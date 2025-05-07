using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class ServerLoop
{
    private readonly string magic;
    private readonly int port;

    private bool isRunning = false;
    private UdpClient udpServer;
    private Thread serverThread;
    private IPEndPoint remoteEP;
    private IPEndPoint clientEP;

    private readonly Action<Action> runOnMainThread;

    public ServerLoop(Action<Action> runOnMainThread, int port, string magic)
    {
        this.runOnMainThread = runOnMainThread; this.port = port; this.magic = magic;
    }

    public void Start()
    {
        if (isRunning) return;
        isRunning = true;

        udpServer = new UdpClient(port);
        remoteEP = new IPEndPoint(IPAddress.Any, 0);

        serverThread = new Thread(Loop);
        serverThread.IsBackground = true;
        serverThread.Start();
    }

    private void Loop()
    {
        while (isRunning)
        {
            if (udpServer.Available == 0) { Thread.Sleep(10); continue; }

            byte[] data = udpServer.Receive(ref remoteEP);
            string msg = Encoding.UTF8.GetString(data);

            if (msg == magic + "Discovery" && clientEP == null)
            {
                byte[] res = Encoding.UTF8.GetBytes(magic + "DiscoveryRes");
                udpServer.Send(res, res.Length, remoteEP);
            }
            else if (msg == magic + "Join")
            {
                clientEP = remoteEP;
                ChessManager.instance.playerType = 1;

                runOnMainThread(() =>
                {
                    UnityEngine.Object.FindFirstObjectByType<JoinMsgUi>().Show(remoteEP.Address.ToString());
                    ChessManager.instance.InitGame();
                });
            }
            else if (msg.StartsWith(magic + "Move;"))
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

    public void SendToClient(byte[] data)
    {
        udpServer.Send(data, data.Length, clientEP);
    }

    public void Stop()
    {
        if (!isRunning) return;
        isRunning = false;

        udpServer.Close();
        udpServer.Dispose();
        udpServer = null;

        serverThread.Join();
    }
}
