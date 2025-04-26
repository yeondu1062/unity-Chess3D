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
using System.Threading;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance { get; private set; }

    private const int PORT = 51062;
    private bool isServerRunning = false;
    private UdpClient udpClient;
    private UdpClient udpServer;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        instance = this;

        udpClient = new UdpClient();
        udpClient.EnableBroadcast = true;

        StartServer();
    }

    private void StartServer()
    {
        if (isServerRunning) return;
        isServerRunning = true;

        Thread thread = new Thread(ServerLoop);
        thread.IsBackground = true;
        thread.Start();
    }

    private void ServerLoop()
    {
        udpServer = new UdpClient(PORT);
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

        while (isServerRunning)
        {
            if (udpServer.Available > 0 && Encoding.UTF8.GetString(udpServer.Receive(ref remoteEP)) == "C1062;Discovery")
            {
                byte[] res = Encoding.UTF8.GetBytes("C1062;DiscoveryRes");
                udpServer.Send(res, res.Length, remoteEP);
            }
        }
    }

    private void StopServer()
    {
        isServerRunning = false;

        udpServer.Close();
        udpServer.Dispose();
        udpServer = null;
    }

    public async Task<List<IPEndPoint>> FindServers()
    {
        byte[] discovery = Encoding.UTF8.GetBytes("C1062;Discovery");
        udpClient.Send(discovery, discovery.Length, new IPEndPoint(IPAddress.Broadcast, PORT));
        
        return await WaitForResponses(2000);
    }

    private async Task<List<IPEndPoint>> WaitForResponses(int timeout)
    {
        List<IPEndPoint> servers = new List<IPEndPoint>();
        var startTime = DateTime.UtcNow;

        while ((DateTime.UtcNow - startTime).TotalMilliseconds < timeout)
        {
            if (udpClient != null && udpClient.Available > 0)
            {
                UdpReceiveResult result = await udpClient.ReceiveAsync();
                string msg = Encoding.UTF8.GetString(result.Buffer);

                if (msg == "C1062;DiscoveryRes" && !servers.Contains(result.RemoteEndPoint))
                {
                    servers.Add(result.RemoteEndPoint);
                }
            }
            await Task.Delay(10);
        }

        if (servers.Count > 0) servers.RemoveAt(0); //Remove local IP
        return servers;
    }

    private void OnApplicationQuit()
    {
        StopServer();
        udpClient.Close();
        udpClient.Dispose();
    }
}
