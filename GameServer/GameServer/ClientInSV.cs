using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    class ClientInSV
    {
        public static int BufferSize = 4096;
        public readonly int id;
        public TcpClient ketnoiTCPdenSV;
        private byte[] buffer;
        private NetworkStream stream;
        public ClientInSV(int idClient)
        {
            id = idClient;
        }
        public void KhoiTaoClient()
        {
            ketnoiTCPdenSV.ReceiveBufferSize = BufferSize;
            ketnoiTCPdenSV.SendBufferSize = BufferSize;
            stream =ketnoiTCPdenSV.GetStream();
            buffer = new byte[BufferSize];
            stream.BeginRead(buffer,0,BufferSize,new AsyncCallback(NhanStream),null);
        }
        public void NhanStream(IAsyncResult thongtin)
        {
            try
            {
                int dodaidaybyte = stream.EndRead(thongtin);
                if (dodaidaybyte <= 0) Console.WriteLine("Đéo đọc được mẹ gì");
                else
                {
                    byte[] data = new byte[dodaidaybyte];
                    Array.Copy(buffer, data, dodaidaybyte);
                    //Xử lý thông tin nhận được
                    stream.BeginRead(buffer, 0, BufferSize, new AsyncCallback(NhanStream), null);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Đéo đọc được gì, lỗi cmnr");
            }
        }
    }
}
