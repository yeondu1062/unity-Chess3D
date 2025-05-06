/* ____ _                   _____ ____  
 / ___| |__   ___  ___ ___|___ /|  _ \ 
| |   | '_ \ / _ \/ __/ __| |_ \| | | |
| |___| | | |  __/\__ \__ \___) | |_| |
 \____|_| |_|\___||___/___/____/|____/ 
   written by @yeondu1062.
*/

using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance { get; private set; }

    private const int PORT = 51062;
    private const string MAGIC = "C1062;";

    private UdpClient udpClient;

    private readonly Queue<Action> mainThreadQueue = new();

    private ServerLoop serverLooper;
    private ClientLoop clientLooper;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        instance = this;

        serverLooper = new ServerLoop(ScheduleMainThread, PORT, MAGIC);
        clientLooper = new ClientLoop(ScheduleMainThread, PORT, MAGIC);

        udpClient = new UdpClient();
        udpClient.EnableBroadcast = true;

        serverLooper.Start();
    }

    private void Update()
    {
        lock (mainThreadQueue) 
        {
            while (mainThreadQueue.Count > 0) mainThreadQueue.Dequeue().Invoke();
        }
    }

    private void ScheduleMainThread(Action action)
    {
        lock (mainThreadQueue) mainThreadQueue.Enqueue(action);
    }

    public async Task<List<IPEndPoint>> FindServers()
    {
        byte[] discovery = Encoding.UTF8.GetBytes(MAGIC + "Discovery");
        udpClient.Send(discovery, discovery.Length, new IPEndPoint(IPAddress.Broadcast, PORT));

        return await clientLooper.WaitForResponses(udpClient, 1500);
    }

    public void JoinServer(string ip)
    {
        byte[] join = Encoding.UTF8.GetBytes(MAGIC + "Join");
        udpClient.Send(join, join.Length, new IPEndPoint(IPAddress.Parse(ip), PORT));

        ChessManager.instance.InitGame();
        ChessManager.instance.playerType = 2;
        FindFirstObjectByType<JoinMsgUi>().Show(ip);

        serverLooper.Stop();
        clientLooper.Start(ip, udpClient);
    }

    public void MoveDataToServer(float fromX, float fromZ, float toX, float toZ)
    {
        byte[] move = Encoding.UTF8.GetBytes(MAGIC + $"Move;{fromX};{fromZ};{toX};{toZ};");
        clientLooper.SendToServer(move);
    }

    public void MoveDataToClient(float fromX, float fromZ, float toX, float toZ)
    {
        byte[] move = Encoding.UTF8.GetBytes(MAGIC + $"Move;{fromX};{fromZ};{toX};{toZ};");
        serverLooper.SendToClient(move);
    }

    private void OnApplicationQuit()
    {
        serverLooper.Stop();
        clientLooper.Stop();
    }
}
