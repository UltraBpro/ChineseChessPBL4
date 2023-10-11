using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public GameObject[,] allTitle=new GameObject[9,10];
    public List<GameObject> P1 = new List<GameObject>();
    public List<GameObject> P2 = new List<GameObject>();
    public int PlayingTeam = 1,myTeam=1;
    public AudioClip MoveSound, EatSound;
    private AudioSource audioSource;
    // Start is called before the first frame update
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        foreach (GameObject i in P1) { allTitle[(int)i.transform.position.x, (int)i.transform.position.y] = i; i.GetComponent<QuanCo>().LoadSkin(); }
        foreach (GameObject i in P2) { allTitle[(int)i.transform.position.x, (int)i.transform.position.y] = i; i.GetComponent<QuanCo>().LoadSkin(); }
    }
    public void DestroyMovePlates()
    {
        //Destroy old MovePlates
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        for (int i = 0; i < movePlates.Length; i++)
        {
            Destroy(movePlates[i]);
        }
    }
    public void RemoveMovePlates()
    {
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        for (int i = 0; i < movePlates.Length; i++)
        {
            movePlates[i].GetComponent<SpriteRenderer>().enabled = false;
        }
    }
    public void DiChuyenQuan(GameObject currentMovingObject, int cot, int hang, bool attack)
    {
        if (attack)
        {
            DietQuan(cot, hang);
            audioSource.clip = EatSound;
        }
        else audioSource.clip = MoveSound;
        audioSource.Play();
        allTitle[(int)currentMovingObject.transform.position.x, (int)currentMovingObject.transform.position.y] = null;
        currentMovingObject.transform.position = new Vector3(cot, hang);
        //Update the matrix
        allTitle[cot, hang] = currentMovingObject;
        //extra
        QuanCo concodangdi = currentMovingObject.GetComponent<QuanCo>();
        if (GlobalThings.GameRule == 1)
            if (concodangdi.TenThatCoUp != null) { concodangdi.TenQuanCo = concodangdi.TenThatCoUp;concodangdi.LoadSkin(); concodangdi.TenThatCoUp = null; }
        
        if (currentMovingObject.GetComponent<QuanCo>().TenQuanCo == "tot")
        {
            if (concodangdi.Team == 1 && concodangdi.transform.position.y >= 5) concodangdi.TenQuanCo += "sangxong";
            if (concodangdi.Team == 2 && concodangdi.transform.position.y <= 4) concodangdi.TenQuanCo += "sangxong";
        }
    }
    public void DietQuan(int cot,int hang)
    {
        GameObject cp = allTitle[cot, hang];
        QuanCo loaiQuan = cp.GetComponent<QuanCo>();
        if (loaiQuan.Team == 1) P1.Remove(cp);
        else P2.Remove(cp);
        allTitle[cot, hang] = null;
        if (loaiQuan.TenQuanCo == "vua")
        {
            if (loaiQuan.Team == 1) Debug.Log("Player 2 win");
            else Debug.Log("Player 1 win");
        }
        Destroy(cp);
    }
    public void NextTurn()
    {
        if (PlayingTeam == 2) PlayingTeam = 1;
        else PlayingTeam ++;
        if (GlobalThings.GameMode == 1) BotPlay();
    }
    public GameObject CheckObjOnTitle(int cot,int hang)
    {
        if (cot < 0 || cot > 8 || hang < 0 || hang > 9) return null;
        return allTitle[cot, hang];
    }
    private void BotPlay()
    {
        //BOT PLAY, DUHHH
    }
    public void LoadCoUp()
    {
        GlobalThings.GameRule = 1;
        List<string> names = new List<string> { "si", "tuong", "ma", "xe","phao",
                                                "si", "tuong", "ma", "xe","phao",
                                                "tot","tot","tot","tot","tot",
        };
        for(int i = 1; i < P1.Count; i++)
        {
            int index = Random.Range(0, names.Count - 1);
            string randomName = names[index];
            P1[i].GetComponent<QuanCo>().TenThatCoUp = randomName;
            P1[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Game/Skin" + GlobalThings.SkinID + "/BlankDo");
            names.RemoveAt(index);
        }
        names=new List<string>{ "si", "tuong", "ma", "xe","phao",
                                "si", "tuong", "ma", "xe","phao",
                                "tot","tot","tot","tot","tot",
        };
        for (int i = 1; i < P2.Count; i++)
        {
            int index = Random.Range(0, names.Count - 1);
            string randomName = names[index];
            P2[i].GetComponent<QuanCo>().TenThatCoUp = randomName;
            P2[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Game/Skin" + GlobalThings.SkinID + "/BlankDen");
            names.RemoveAt(index);
        }
    }
    //TEMP SE XOA SAU
    public void Chat()
    {
        ThreadManager.ExecuteOnMainThread(() =>
        {
        string content = GameObject.Find("ChatBoxTextInput").GetComponent<InputField>().text;
        GameObject.Find("ChatBoxTextInput").GetComponent<InputField>().text = "";
        GameClient.instance.GuiDenSV(Encoding.UTF8.GetBytes(GameClient.instance.idDoiPhuong + "|CHAT|" + content));
        GameObject.Find("ChatBoxTextOutput").GetComponent<Text>().text += "\n" + content;
        });
    }
    public void ConnectDenSV()
    {
        ThreadManager.ExecuteOnMainThread(() =>
        {
            GameClient.instance.ConnectDenSV(GameObject.Find("TextBoxIPTEMP").GetComponent<InputField>().text, 1006);
        });
    }
    public void TEMP()
    {
        GlobalThings.GameMode = 2;
        GameClient.instance.GuiDenSV(Encoding.UTF8.GetBytes("MATCHMAKING|"+GameClient.instance.idDuocCap));
    }
}
public static class GlobalThings
{
    public static int GameMode=0;
    public static int GameRule = 0;
    public static int SkinID = 0;
}