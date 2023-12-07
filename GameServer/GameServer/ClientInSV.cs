﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    internal class ClientInSV
    {
        public static int BufferSize = 4096;
        public readonly int id;
        public TcpClient ketnoiTCPdenSV;
        private byte[] buffer;
        private NetworkStream stream;
        public player ThisPlayer = null;
        public ClientInSV(int idClient)
        {
            id = idClient;
        }

        public void KhoiTaoClient()
        {
            ketnoiTCPdenSV.ReceiveBufferSize = BufferSize;
            ketnoiTCPdenSV.SendBufferSize = BufferSize;
            stream = ketnoiTCPdenSV.GetStream();
            buffer = new byte[BufferSize];
            stream.BeginRead(buffer, 0, BufferSize, new AsyncCallback(NhanStream), null);
        }

        public void NhanStream(IAsyncResult thongtin)
        {
            try
            {
                int dodaidaybyte = stream.EndRead(thongtin);
                if (dodaidaybyte <= 0) {
                    if (ThisPlayer != null)
                    {
                        using (PBL4Entities db = new PBL4Entities())
                        {
                            player targetAcc = db.players.Find(System.Convert.ToInt32(ThisPlayer.id));
                            targetAcc.online = false;
                            db.SaveChanges();
                        }
                    }
                    ketnoiTCPdenSV.Dispose();
                    ketnoiTCPdenSV = null;
                    ThisPlayer = null;
                    Console.WriteLine("Kết nối đã bị ngắt."); }
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
            catch (ObjectDisposedException)
            {
                Console.WriteLine("TcpClient đã được Dispose");
            }
            catch (IOException ex)
            {
                Console.WriteLine("Kết nối đã bị ngắt: " + ex.Message);
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
                string[] info = command.Split(new char[] { '|' }, 2);
                if (int.TryParse(info[0], out int idtarget))
                {
                    ClientInSV Target = Server.DSClient[Convert.ToInt32(info[0])];
                    Target.GuiDenClient(Encoding.UTF8.GetBytes(info[1]));
                }
                else
                {
                    info = command.Split('|');
                    switch (info[0])
                    {
                        case "REGISTER":
                            using (PBL4Entities db = new PBL4Entities())
                            {
                                player newplayer = new player
                                {
                                    username = info[1],
                                    password = info[2],
                                    salt = info[3]
                                };
                                db.players.Add(newplayer);
                                db.SaveChanges();
                                //Thong bao dang ky thanh cong
                            }
                            break;

                        case "LOGIN":
                            using (PBL4Entities db = new PBL4Entities())
                            {
                                string usernametocheck = info[1];
                                player targetAcc = db.players.Where(p => p.username == usernametocheck).FirstOrDefault();
                                if (targetAcc != null)
                                {
                                    if (VerifyAccount(info[2], targetAcc.salt, targetAcc.password))
                                    {
                                        if (targetAcc.online != true)
                                        {
                                            Server.DSClient[int.Parse(info[3])].GuiDenClient(Encoding.UTF8.GetBytes("LOGIN|" + targetAcc.id + "|" + targetAcc.username + "|" + targetAcc.score));
                                            ThisPlayer = targetAcc;
                                            targetAcc.online = true;
                                            db.SaveChanges();
                                        }
                                        else Server.DSClient[int.Parse(info[3])].GuiDenClient(Encoding.UTF8.GetBytes("ERROR|Tài khoản đã được đăng nhập ở nơi khác"));
                                    }
                                    else
                                    {
                                        Server.DSClient[int.Parse(info[3])].GuiDenClient(Encoding.UTF8.GetBytes("ERROR|Sai mật khẩu"));
                                    }
                                }
                                else
                                {
                                    Server.DSClient[int.Parse(info[3])].GuiDenClient(Encoding.UTF8.GetBytes("ERROR|Tài khoản không tồn tại"));
                                }
                            }
                            break;
                        case "LOGOUT":
                            if (info[1] != "-1")
                            { 
                                using (PBL4Entities db = new PBL4Entities())
                                {
                                    player targetAcc = db.players.Find(System.Convert.ToInt32(info[1]));
                                    targetAcc.online = false;
                                    db.SaveChanges();
                                } 
                            }
                            byte[] data=(Encoding.UTF8.GetBytes("LOGOUT"));
                            IAsyncResult result = stream.BeginWrite(data, 0, data.Length, null, null);
                            stream.EndWrite(result);
                            ThisPlayer = null;
                            ketnoiTCPdenSV.Dispose();
                            ketnoiTCPdenSV = null;
                            break;

                        case "MATCHMAKING":
                            Server.AddToMMQueue(Convert.ToInt32(info[1]));
                            break;

                        case "MATCHMAKINGCOUP":
                            Server.AddToMMQueueCoUp(Convert.ToInt32(info[1]));
                            break;
                        case "SAVESCORE":
                            using (PBL4Entities db = new PBL4Entities())
                            {
                                player targetAcc=db.players.Find(System.Convert.ToInt32(info[1]));
                                targetAcc.score = System.Convert.ToInt32(info[2]);
                                db.SaveChanges();
                            }
                                break;
                        case "ALLSCORE":
                            {
                                using (PBL4Entities db = new PBL4Entities())
                                {
                                    var players = db.players
                                        .OrderByDescending(p => p.score)
                                        .ToList();

                                    int rank = 1;
                                    int lastScore = players[0].score;
                                    StringBuilder output = new StringBuilder();
                                    List<string> outputs = new List<string>();

                                    foreach (var player in players)
                                    {
                                        if (player.score < lastScore)
                                        {
                                            rank++;
                                            lastScore = player.score;
                                        }

                                        string playerString = $"{rank}|{player.username}|{player.score}|";

                                        // Check if adding the next player would exceed the buffer size
                                        if ((output.Length + playerString.Length) * sizeof(char) > BufferSize-20)
                                        {
                                            // Remove the last "|"
                                            output.Remove(output.Length - 1, 1);
                                            outputs.Add("ALLSCORE|" + output.ToString());
                                            output.Clear();
                                        }

                                        output.Append(playerString);
                                    }

                                    // Add the remaining players
                                    if (output.Length > 0)
                                    {
                                        // Remove the last "|"
                                        output.Remove(output.Length - 1, 1);
                                        outputs.Add("ALLSCORE|" + output.ToString());
                                    }

                                    foreach (var outStr in outputs)
                                    {
                                        Console.WriteLine(outStr);
                                        GuiDenClient(Encoding.UTF8.GetBytes(outStr));
                                    }
                                }
                                break;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Có lỗi xảy ra phản ứng client: " + ex.Message);
            }
        }

        public bool VerifyAccount(string password, string salt, string hashtoverify)
        {
            using (MD5 md5 = MD5.Create())
            {
                // Chuyển chuỗi input thành mảng byte
                byte[] inputBytes = Encoding.ASCII.GetBytes(password + salt);

                // Tính toán hash
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Chuyển mảng byte thành chuỗi hex
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return hashtoverify.Trim() == sb.ToString();
            }
        }
    }
}