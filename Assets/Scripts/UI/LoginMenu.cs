using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LoginMenu : MonoBehaviour
{
    public GameObject LoginPanel;
    public GameObject RegisterPanel;
    public GameObject textboxUsernameRegister;
    public GameObject textboxPasswordRegister;
    public GameObject textboxPasswordRegisterDoubleCheck;
    public GameObject textboxUsernameLogin;
    public GameObject textboxPasswordLogin;
    public GameObject MessageLogin;
    public GameObject MessageRegister;

    public void openRegisterPanel()
    {
        RegisterPanel.SetActive(true);
        LoginPanel.SetActive(false);
        ResetContent();
    }

    public void openLoginPanel()
    {
        RegisterPanel.SetActive(false);
        LoginPanel.SetActive(true);
        ResetContent();
    }

    public void HiddenConnect()
    {
        if (GameClient.instance.idDuocCap == -1)
        {
            GameClient.instance.WaitingForServer = true;
            GameClient.instance.ConnectDenSV("127.0.0.1", 1006);
            float startTime = Time.realtimeSinceStartup;
            while (GameClient.instance.WaitingForServer)
            {
                if (Time.realtimeSinceStartup - startTime > 5)
                {
                    GameClient.instance.WaitingForServer = false;
                    break;
                }
                if(!GameClient.instance.WaitingForServer)this.gameObject.GetComponent<MainMenu>().openLoginPanel();
            }
        }
    }

    public void ResetContent()
    {
        textboxUsernameRegister.GetComponent<InputField>().text = "";
        textboxPasswordRegister.GetComponent<InputField>().text = "";
        textboxPasswordRegisterDoubleCheck.GetComponent<InputField>().text = "";
        textboxUsernameLogin.GetComponent<InputField>().text = "";
        textboxPasswordLogin.GetComponent<InputField>().text = "";
        MessageLogin.GetComponent<Text>().text = "";
        MessageRegister.GetComponent<Text>().text = "";
    }

    #region DangKy

    public void Register()
    {
        if (textboxPasswordRegister.GetComponent<InputField>().text == textboxPasswordRegisterDoubleCheck.GetComponent<InputField>().text)
        {
            string salt = CreateSalt(64);
            string username = textboxUsernameRegister.GetComponent<InputField>().text;
            string password = textboxPasswordRegister.GetComponent<InputField>().text;
            string passwordWithSalt = password + salt;

            // Tạo một instance của lớp MD5
            using (MD5 md5 = MD5.Create())
            {
                // Chuyển chuỗi input thành mảng byte
                byte[] inputBytes = Encoding.ASCII.GetBytes(passwordWithSalt);

                // Tính toán hash
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Chuyển mảng byte thành chuỗi hex
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                GameClient.instance.GuiDenSV(Encoding.UTF8.GetBytes("REGISTER|" + username + "|" + sb.ToString() + "|" + salt));

            }
        }
        else MessageRegister.GetComponent<Text>().text = "Password nhập hai lần phải trùng khớp.";
    }

    public static string CreateSalt(int size)
    {
        // Tạo một instance của lớp RNGCryptoServiceProvider
        using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
        {
            byte[] buff = new byte[size / 2];
            rng.GetBytes(buff);

            // Chuyển mảng byte thành chuỗi hex
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < buff.Length; i++)
            {
                sb.Append(buff[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }

    #endregion DangKy

    #region DangNhap

    public void Login()
    {
        string username = textboxUsernameLogin.GetComponent<InputField>().text;
        string password = textboxPasswordLogin.GetComponent<InputField>().text;
        GameClient.instance.GuiDenSV(Encoding.UTF8.GetBytes("LOGIN|" + username + "|" + password + "|" + GameClient.instance.idDuocCap));
    }

    #endregion DangNhap
}

public class player
{
    public int id { get; set; }
    public string username { get; set; }
    public int score { get; set; }
}