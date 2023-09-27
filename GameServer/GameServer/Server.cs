using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace GameServer
{
    class Server
    {
        public static int MaxConnections=10;
        public static int Port=1006;
        public static Dictionary<int, ClientInSV> DSClient = new Dictionary<int, ClientInSV>();
        public static TcpListener ServerTcpListener;
        public static void Chay()
        {
            for (int i = 0; i < MaxConnections; i++) DSClient.Add(i, new ClientInSV(i));
            ServerTcpListener = new TcpListener(IPAddress.Any, Port);
            ServerTcpListener.Start();
            ServerTcpListener.BeginAcceptTcpClient(new AsyncCallback(NhanKetNoi), null);
            Console.WriteLine("Tao SV o Port: " + Port);
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
                    DSClient[i].ketnoiTCPdenSV=clientketnoi;
                }
            }
        }
    }
}
