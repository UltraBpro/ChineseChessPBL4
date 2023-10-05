using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public void TuChoiVoiNhauDi()
    {
        SceneManager.LoadScene("ChineseChessGame");
    }
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
            port = "1006";
        }
        GameClient.instance.ConnectDenSV(ip, System.Convert.ToInt32(port));
    }
    public void Tatgame() {
        Application.Quit();
    }
    public void SendSTH()
    {
        GameClient.instance.GuiDenSV(Encoding.UTF8.GetBytes(GameObject.Find("TextBoxChatWithSV").GetComponent<InputField>().text));
    }
}
