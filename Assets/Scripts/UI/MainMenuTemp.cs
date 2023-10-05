using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuTemp : MonoBehaviour
{
    public void TuChoiVoiNhauDi()
    {
        SceneManager.LoadScene("ChineseChessGame");
    }
    public void ConnectDenSV()
    {
        GameClient.instance.ConnectDenSV("127.0.0.1", 1006);
    }
    public void Tatgame() {
        Application.Quit();
    }
    public void SendSTH()
    {
        GameClient.instance.GuiDenSV(Encoding.UTF8.GetBytes(GameObject.Find("TextBoxChatWithSV").GetComponent<InputField>().text));
    }
}
