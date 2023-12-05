using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace GameServer
{
    internal static class Server
    {
        public static int MaxConnections = 20;
        public static int Port = 1006;
        public static Dictionary<int, ClientInSV> DSClient = new Dictionary<int, ClientInSV>();
        public static Queue<int> MatchmakingQueue = new Queue<int>();
        public static Queue<int> MatchmakingQueueCoUp = new Queue<int>();
        public static TcpListener ServerTcpListener;

        public static void Chay(string IPtoHost)
        {
            if (IPtoHost == "") IPtoHost = "127.0.0.1";
            for (int i = 0; i < MaxConnections; i++) DSClient.Add(i, new ClientInSV(i));
            ServerTcpListener = new TcpListener(IPAddress.Parse(IPtoHost), Port);
            ServerTcpListener.Start();
            ServerTcpListener.BeginAcceptTcpClient(new AsyncCallback(NhanKetNoi), null);
            Console.WriteLine("Tao SV o dia chi: "+IPtoHost+":"+Port);
        }

        private static void NhanKetNoi(IAsyncResult ketqua)
        {
            TcpClient clientketnoi = ServerTcpListener.EndAcceptTcpClient(ketqua);
            ServerTcpListener.BeginAcceptTcpClient(new AsyncCallback(NhanKetNoi), null);
            Console.WriteLine("Da nhan 1 ket noi den tu " + clientketnoi.Client.RemoteEndPoint.ToString());
            for (int i = 0; i < MaxConnections; i++)
            {
                if (DSClient[i].ketnoiTCPdenSV == null)
                {
                    DSClient[i].ketnoiTCPdenSV = clientketnoi;
                    DSClient[i].KhoiTaoClient();
                    DSClient[i].GuiDenClient(Encoding.UTF8.GetBytes("HELLO|" + DSClient[i].id));
                    break;
                }
            }
        }

        public static void AddToMMQueue(int idadd)
        {
            MatchmakingQueue.Enqueue(idadd);
            if (MatchmakingQueue.Count >= 2)
            {
                TaoMatch(MatchmakingQueue.Dequeue(), MatchmakingQueue.Dequeue());
            }
        }

        public static void AddToMMQueueCoUp(int idadd)
        {
            MatchmakingQueueCoUp.Enqueue(idadd);
            if (MatchmakingQueueCoUp.Count >= 2)
            {
                TaoMatch(MatchmakingQueueCoUp.Dequeue(), MatchmakingQueueCoUp.Dequeue());
            }
        }

        public static void TaoMatch(int id1, int id2)
        {
            Console.WriteLine("Tien hanh ghep tran giua client" + id1 + " va client" + id2);
            byte[] data = Encoding.UTF8.GetBytes("MATCH|1|" + id2 + "|" + DSClient[id2].ThisPlayer.id + "|" + DSClient[id2].ThisPlayer.username + "|" + DSClient[id2].ThisPlayer.score);
            DSClient[id1].GuiDenClient(data);

            // Gửi ID của client1 tới client2
            byte[] data2 = Encoding.UTF8.GetBytes("MATCH|2|" + id1 + "|" + DSClient[id1].ThisPlayer.id + "|" + DSClient[id1].ThisPlayer.username + "|" + DSClient[id1].ThisPlayer.score);
            DSClient[id2].GuiDenClient(data2);
        }

        public static void GuiThongTinAccount(int idclienttarget, int idaccount)
        {
            //byte[] data2 = Encoding.UTF8.GetBytes("ACCOUNT|2|" + id1);
            //DSClient[idclienttarget].GuiDenClient(data2);
        }
    }
}