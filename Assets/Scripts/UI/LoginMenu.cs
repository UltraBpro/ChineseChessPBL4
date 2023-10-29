using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LoginMenu : MonoBehaviour
{
    public GameObject textboxUsernameRegister;
    public GameObject textboxPasswordRegister;
    public GameObject textboxUsernameLogin;
    public GameObject textboxPasswordLogin;
    #region DangKy
    public void Register()
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
            GameClient.instance.GuiDenSV(Encoding.UTF8.GetBytes("REGISTER|" + username +"|"+sb.ToString()+"|"+salt));
        }
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
    #endregion


    #region DangNhap
    public void Login()
    {
        string username = textboxUsernameLogin.GetComponent<InputField>().text;
        string password = textboxPasswordLogin.GetComponent<InputField>().text;
        GameClient.instance.GuiDenSV(Encoding.UTF8.GetBytes("LOGIN|"+username+"|"+password+"|"+GameClient.instance.idDuocCap));
    }
    #endregion
}
public class player
{
    public int id { get; set; }
    public string username { get; set; }
    public int score { get; set; }
}
