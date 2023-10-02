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
                    Console.WriteLine(Encoding.UTF8.GetString(data));
                    ReactToClient(Encoding.UTF8.GetString(data));
                    stream.BeginRead(buffer, 0, BufferSize, new AsyncCallback(NhanStream), null);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Đéo đọc được gì, lỗi cmnr");
            }
        }
        public void GuiDenClient(byte[] data)
        {
            try
            {
                // Bắt đầu hoạt động gửi không đồng bộ
                stream.BeginWrite(data, 0, data.Length, new AsyncCallback(DaGuiXongRoi), stream);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Có lỗi xảy ra: " + ex.Message);
            }
        }

        private void DaGuiXongRoi(IAsyncResult ketqua)
        {
            try
            {
                stream.EndWrite(ketqua);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Có lỗi xảy ra: " + ex.Message);
            }
        }
        private void ReactToClient(string command)
        {
            try
            {
                // Tách chuỗi thành các phần thông tin
                string[] info = command.Split('/');

                // Kiểm tra nếu info1 là "hello"
                switch (info[0])
                {
                    case "Hello":
                        GuiDenClient(Encoding.UTF8.GetBytes("Chào mày, id của mày là: " + id));
                        break;
                }
                // Thêm các trường hợp xử lý khác tại đây dựa trên giá trị của info1, info2, v.v...
            }
            catch (Exception ex)
            {
                Console.WriteLine("Có lỗi xảy ra: " + ex.Message);
            }
        }
    }
}
