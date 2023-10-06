﻿using System;
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
                if (dodaidaybyte <= 0) Console.WriteLine("BLANK");
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
            catch (Exception ex)
            {
                Console.WriteLine("Có lỗi xảy ra khi nhận kết nối: " + ex.Message);
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
                Console.WriteLine("Có lỗi xảy ra khi gửi đến client: " + ex.Message);
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
                Console.WriteLine("Có lỗi xảy ra khi hoàn thành gửi: " + ex.Message);
            }
        }
        private void ReactToClient(string command)
        {
            try
            {
                // Tách chuỗi thành các phần thông tin ""TARGET"|CMD"|"DATA"|...
                string[] info = command.Split(new char[] { '|' }, 2) ;
                Console.WriteLine(info[0]);
                Console.WriteLine(info[1]);
                ClientInSV Target = Server.DSClient[Convert.ToInt32(info[0])];
                Target.GuiDenClient(Encoding.UTF8.GetBytes(info[1]));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Có lỗi xảy ra phản ứng client: " + ex.Message);
            }
        }
    }
}
