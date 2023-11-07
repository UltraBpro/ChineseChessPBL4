﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameClient : MonoBehaviour
{
    public static GameClient instance { get; private set; }
    public static int BufferSize = 4096;
    private string IP; private int Port;
    public int idDuocCap=-1;
    public int idDoiPhuong;
    public TcpClient ketnoiTCPdenSV;
    private byte[] buffer;
    private NetworkStream stream;
    public player CurrentAccount = null;

    //Ket qua tra ve
    public bool WaitingForServer = false;

    public void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(this);
    }

    public void ConnectDenSV(string IPkn, int Portkn)
    {
        instance.IP = IPkn; instance.Port = Portkn;
        ketnoiTCPdenSV = new TcpClient
        {
            ReceiveBufferSize = BufferSize,
            SendBufferSize = BufferSize
        };
        buffer = new byte[BufferSize];
        ketnoiTCPdenSV.BeginConnect(instance.IP, instance.Port, NhanKetNoi, ketnoiTCPdenSV);
    }

    public void NhanKetNoi(IAsyncResult ketnoi)
    {
        ketnoiTCPdenSV.EndConnect(ketnoi);
        if (ketnoiTCPdenSV.Connected)
        {
            stream = ketnoiTCPdenSV.GetStream();
            stream.BeginRead(buffer, 0, BufferSize, new AsyncCallback(NhanStream), null);
        }
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
                ReactToServer(Encoding.UTF8.GetString(data));
                stream.BeginRead(buffer, 0, BufferSize, new AsyncCallback(NhanStream), null);
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Có lỗi xảy ra khi nhận kết nối: " + ex.Message);
        }
    }

    public void GuiDenSV(byte[] data)
    {
        try
        {
            stream.BeginWrite(data, 0, data.Length, new AsyncCallback(DaGuiXongRoi), stream);
        }
        catch (Exception e)
        {
            Debug.Log("Có lỗi xảy ra khi gửi đến server: " + e.Message);
        }
    }

    public void DaGuiXongRoi(IAsyncResult ketquagui)
    {
        try
        {
            stream.EndWrite(ketquagui);
        }
        catch (Exception e)
        {
            Debug.Log("Có lỗi xảy ra khi hoàn thành gửi: " + e.Message);
        }
    }

    private void ReactToServer(string command)
    {
        try
        {
            // Tách chuỗi thành các phần thông tin
            string[] info = command.Split('|');

            // Kiểm tra nếu info1 là "hello"
            switch (info[0])
            {
                case "HELLO":
                    idDuocCap = System.Convert.ToInt32(info[1]);
                    WaitingForServer = false;
                    break;

                case "LOGIN":
                    CurrentAccount = new player { id = int.Parse(info[1]), username = info[2], score = int.Parse(info[3]) };
                    Debug.Log("Sucess");
                    break;

                case "CHAT":
                    ThreadManager.ExecuteOnMainThread(() => GameObject.Find("ChatBoxTextOutput").GetComponent<Text>().text += "\n" + info[1]);
                    break;

                case "MATCH":
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        Game controllerscript = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
                        controllerscript.myTeam = System.Convert.ToInt32(info[1]);
                        idDoiPhuong = System.Convert.ToInt32(info[2]);
                    });
                    break;

                case "LOADCOUP":
                    GlobalThings.GameRule = 1;//Se xoa sau;
                    List<string> nametosave = new List<string>();
                    for (int i = 1; i < info.Length; i++) { nametosave.Add(info[i]); }
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>().LoadCoUpFromOnl(nametosave);
                    });
                    break;

                case "MOVE":
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        Game controllerscript = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
                        controllerscript.DiChuyenQuan(GameObject.Find(info[1]), System.Convert.ToInt32(info[2]), System.Convert.ToInt32(info[3]));
                        controllerscript.NextTurn();

                        //GameObject movingobj=GameObject.Find(info[1]);
                        //GameObject mp = Instantiate(movingobj.GetComponent<QuanCo>().movePlate, new Vector3(System.Convert.ToInt32(info[2]), System.Convert.ToInt32(info[3]), 0), Quaternion.identity);
                        //MovePlate mpScript = mp.GetComponent<MovePlate>();
                        //if (System.Convert.ToBoolean(info[4])) mpScript.attack = true;
                        //mpScript.currentMovingObject = movingobj.gameObject;
                        //mp.SendMessage("OnMouseDown");
                    });
                    break;
            }
            // Thêm các trường hợp xử lý khác tại đây dựa trên giá trị của info1, info2, v.v...
        }
        catch (Exception ex)
        {
            Console.WriteLine("Có lỗi xảy ra khi phản ứng server: " + ex.Message);
        }
    }
}