using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OnlineMenu : MonoBehaviour
{
    public AudioClip ClickSound;
    public AudioClip HoverSound;
    public GameObject AccountName;
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        LoadAccountPanel();
    }
    public void playHoverClip()
    {
        audioSource.clip = HoverSound;
        audioSource.Play();
    }

    public void playClickSound()
    {
        audioSource.clip = ClickSound;
        audioSource.Play();
    }
    public void ChangeGameRule()
    {
        GlobalThings.GameRule = GlobalThings.GameRule == 0 ? 1 : 0;
    }
    public void loadMainMenuToExit()
    {
        GameClient.instance.Reset();
        LoadingThings.LoadingTarget = 0;
        SceneManager.LoadScene(2);
    }
    public void FindMatch()
    {
        GlobalThings.GameMode = 2;
        if (GlobalThings.GameRule == 0) GameClient.instance.GuiDenSV(Encoding.UTF8.GetBytes("MATCHMAKING|" + GameClient.instance.idDuocCap));
        else GameClient.instance.GuiDenSV(Encoding.UTF8.GetBytes("MATCHMAKINGCOUP|" + GameClient.instance.idDuocCap));
        GameClient.instance.WaitingForServer = true;
        while (GameClient.instance.WaitingForServer)
        {
        }
        LoadingThings.LoadingTarget = 1;
        SceneManager.LoadScene(2);
    }
    public void LoadAccountPanel()
    {
        AccountName.GetComponent<InputField>().text = GameClient.instance.CurrentAccount.username;
    }


}
