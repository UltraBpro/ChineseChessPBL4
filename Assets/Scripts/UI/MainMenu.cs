﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    private Animator anim;

    public int quickSaveSlotID;

    [Header("Options Panel")]
    public GameObject MainOptionsPanel;

    public GameObject StartGameOptionsPanel;
    public GameObject GamePanel;
    public GameObject ControlsPanel;
    public GameObject GfxPanel;
    public GameObject LoginPanel;
    public GameObject SkinPanel;
    public GameObject BotLevelSelector;
        
    public AudioClip ClickSound;
    public AudioClip HoverSound;
    private AudioSource audioSource;

    // Use this for initialization
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        anim = GetComponent<Animator>();
    }

    #region Open Different panels

    public void openOptions()
    {
        //enable respective panel
        MainOptionsPanel.SetActive(true);
        StartGameOptionsPanel.SetActive(false);

        //play anim for opening main options panel
        anim.Play("buttonTweenAnims_on");

        //play click sfx
        playClickSound();

        //enable BLUR
        //Camera.main.GetComponent<Animator>().Play("BlurOn");
    }

    public void openStartGameOptions()
    {
        //enable respective panel
        MainOptionsPanel.SetActive(false);
        StartGameOptionsPanel.SetActive(true);

        //play anim for opening main options panel
        anim.Play("buttonTweenAnims_on");

        //play click sfx
        playClickSound();

        //enable BLUR
        //Camera.main.GetComponent<Animator>().Play("BlurOn");
    }

    public void openOptions_Game()
    {
        //enable respective panel
        GamePanel.SetActive(true);
        ControlsPanel.SetActive(false);
        GfxPanel.SetActive(false);
        LoginPanel.SetActive(false);
        SkinPanel.SetActive(false);

        //play anim for opening game options panel
        anim.Play("OptTweenAnim_on");

        //play click sfx
        playClickSound();
    }

    public void openOptions_Controls()
    {
        //enable respective panel
        GamePanel.SetActive(false);
        ControlsPanel.SetActive(true);
        GfxPanel.SetActive(false);
        LoginPanel.SetActive(false);
        SkinPanel.SetActive(false);

        //play anim for opening game options panel
        anim.Play("OptTweenAnim_on");

        //play click sfx
        playClickSound();
    }

    public void openOptions_Gfx()
    {
        //enable respective panel
        GamePanel.SetActive(false);
        ControlsPanel.SetActive(false);
        GfxPanel.SetActive(true);
        LoginPanel.SetActive(false);
        SkinPanel.SetActive(false);

        //play anim for opening game options panel
        anim.Play("OptTweenAnim_on");

        //play click sfx
        playClickSound();
    }

    public void openLoginPanel()
    {
        //enable respective panel
        GamePanel.SetActive(false);
        ControlsPanel.SetActive(false);
        GfxPanel.SetActive(false);
        LoginPanel.SetActive(true);
        SkinPanel.SetActive(false);

        //play anim for opening game options panel
        anim.Play("OptTweenAnim_on");

        //play click sfx
        playClickSound();
    }

    public void openSkinPanel()
    {
        //enable respective panel
        GamePanel.SetActive(false);
        ControlsPanel.SetActive(false);
        GfxPanel.SetActive(false);
        LoginPanel.SetActive(false);
        SkinPanel.SetActive(true);

        //play anim for opening game options panel
        anim.Play("OptTweenAnim_on");

        //play click sfx
        playClickSound();
    }

    public void SkinSelected(int skinid)
    {
        GlobalThings.SkinID = skinid;
    }

    public void newGame(int GameModeCreate)
    {
        GlobalThings.GameMode = GameModeCreate;
        LoadingThings.LoadingTarget = 1;
        SceneManager.LoadScene(2);
    }

    #endregion Open Different panels

    #region Back Buttons

    public void back_options()
    {
        //simply play anim for CLOSING main options panel
        anim.Play("buttonTweenAnims_off");

        //disable BLUR
        // Camera.main.GetComponent<Animator>().Play("BlurOff");

        //play click sfx
        playClickSound();
    }

    public void back_options_panels()
    {
        this.GetComponent<LoginMenu>().ResetContent();
        //simply play anim for CLOSING main options panel
        SkinPanel.SetActive(false);
        anim.Play("OptTweenAnim_off");
        //play click sfx
        playClickSound();
    }

    public void Quit()
    {
        Application.Quit();
    }

    #endregion Back Buttons

    #region Sounds

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

    public void SaveMusic(float volume)
    {
        GlobalThings.MusicVolume = volume;
    }

    public void SaveSound(float volume)
    {
        GlobalThings.SoundVolume = volume;
        audioSource.volume = volume;
    }

    #endregion Sounds
    public void ChangeGameRule()
    {
        GlobalThings.GameRule = GlobalThings.GameRule == 0 ? 1 : 0;
    }
    public void ChangeDoKho()
    {
        switch (BotLevelSelector.GetComponent<Dropdown>().options[BotLevelSelector.GetComponent<Dropdown>().value].text)
        {
            case "Cực khó":
                GlobalThings.BotLevel = 4;
                break;
            case "Khó":
                GlobalThings.BotLevel = 3;
                break;
            case "Dễ":
                GlobalThings.BotLevel = 2;
                break;
        }
    }
}