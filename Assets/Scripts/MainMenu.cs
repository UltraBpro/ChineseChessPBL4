using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public void ConnectDenSV()
    {
        string[] diachi=GameObject.Find("TextBoxIP").GetComponent<InputField>().text.Split(':');
        string ip = diachi[0];
        string port = diachi.Length > 1 ? diachi[1] : "80";
        System.Net.IPAddress ipAddress;
        if (!System.Net.IPAddress.TryParse(ip, out ipAddress))
        {
            ip = "127.0.0.1";
        }
        int portNumber;
        if (!int.TryParse(port, out portNumber))
        {
            port = "80";
        }
        GameClient.instance.ConnectDenSV(ip, System.Convert.ToInt32(port));
    }
}
