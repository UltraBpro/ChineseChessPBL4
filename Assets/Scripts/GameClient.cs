using System.Collections;
using System.Collections.Generic;
using System.Net;
using System;
using System.Text;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class GameClient : MonoBehaviour
{
    public static GameClient instance;
    public static int BufferSize = 4096;
    string IP;int Port;
    private int idDuocCap;
    public TcpClient ketnoiTCPdenSV;
    private byte[] buffer;
    private NetworkStream stream;
    public void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(this);
    }
    private void Start()
    {
        
    }
    public void ConnectDenSV(string IPkn,int Portkn)
    {
        instance.IP = IPkn;instance.Port = Portkn;
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
            if (dodaidaybyte <= 0) Console.WriteLine("Đéo đọc được mẹ gì");
            else
            {
                byte[] data = new byte[dodaidaybyte];
                Array.Copy(buffer, data, dodaidaybyte);
                //Xử lý thông tin nhận được
                ThreadManager.ExecuteOnMainThread(() => { GameObject.Find("TextFieldFromSV").GetComponent<Text>().text += Encoding.UTF8.GetString(data) + "\n"; });
                stream.BeginRead(buffer, 0, BufferSize, new AsyncCallback(NhanStream), null);
            }
        }
        catch (Exception)
        {
            Console.WriteLine("Đéo đọc được gì, lỗi cmnr");
        }
    }
    public void GuiDenSV(byte[] data)
    {
        try
        {
            stream.BeginWrite(data, 0, data.Length, new AsyncCallback(DaGuiXongRoi), stream);
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
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
            Debug.Log(e.Message);
        }
    }
}
