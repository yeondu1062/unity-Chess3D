using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

public class ClientLoop
{
    private readonly string magic;
    private readonly int port;

    private bool isRunning = false;
    private UdpClient udpClient;
    private Thread clientThread;
    private IPEndPoint remoteEP;
    private string serverIp;

    private readonly Action<Action> runOnMainThread;

    public ClientLoop(Action<Action> runOnMainThread, int port, string magic)
    {
        this.runOnMainThread = runOnMainThread; this.port = port; this.magic = magic;
    }

    public void Start(string ip, UdpClient client)
    {
        if (isRunning) return;
        isRunning = true;

        serverIp = ip; udpClient = client;
        remoteEP = new IPEndPoint(IPAddress.Any, 0);

        clientThread = new Thread(Loop);
        clientThread.IsBackground = true;
        clientThread.Start();
    }

    private void Loop()
    {
        while (isRunning)
        {
            if (udpClient.Available == 0) { Thread.Sleep(10); continue; }

            byte[] data = udpClient.Receive(ref remoteEP);
            string msg = Encoding.UTF8.GetString(data);

            if (msg.StartsWith(magic + "Move;"))
            {
                string[] parts = msg.Split(';');
                if (
                    float.TryParse(parts[2], out float fromX) && float.TryParse(parts[3], out float fromZ) &&
                    float.TryParse(parts[4], out float toX) && float.TryParse(parts[5], out float toZ))
                {
                    runOnMainThread(() =>
                    {
                        ChessPiece piece = ChessManager.GetPieceAtPos(toX, toZ);
                        Transform selectedPieceT = ChessManager.GetPieceAtPos(fromX, fromZ).transform;

                        if (piece != null)
                        {
                            if (piece.isWhite) ChessManager.instance.aliveWhite--;
                            else ChessManager.instance.aliveBlack--;
                            UnityEngine.Object.Destroy(piece.gameObject);
                        }

                        selectedPieceT.position = new UnityEngine.Vector3(toX, 0, toZ);
                        ChessManager.instance.trunChange();
                    });
                }
            }
        }
    }

    public async Task<List<IPEndPoint>> WaitForResponses(UdpClient client, int timeout)
    {
        List<IPEndPoint> servers = new List<IPEndPoint>();
        var startTime = DateTime.UtcNow;

        while ((DateTime.UtcNow - startTime).TotalMilliseconds < timeout)
        {
            if (client.Available > 0)
            {
                UdpReceiveResult result = await client.ReceiveAsync();
                string msg = Encoding.UTF8.GetString(result.Buffer);

                if (msg == magic + "DiscoveryRes" && !servers.Contains(result.RemoteEndPoint))
                {
                    servers.Add(result.RemoteEndPoint);
                }
            }
            await Task.Delay(10);
        }

        return servers;
    }

    public void SendToServer(byte[] data)
    {
        udpClient.Send(data, data.Length, new IPEndPoint(IPAddress.Parse(serverIp), port));
    }

    public void Stop()
    {
        if (!isRunning) return;
        isRunning = false;

        udpClient.Close();
        udpClient.Dispose();
        udpClient = null;

        clientThread.Join();
    }
}
