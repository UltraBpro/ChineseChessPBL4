using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicMenu : MonoBehaviour
{
    public GameObject FullScreen;
    public GameObject Resolutions;
    public GameObject VSync;
    public GameObject AntiAliasing;
    // Start is called before the first frame update
    void Start()
    {
        Resolution[] resolutions = Screen.resolutions;
        Resolutions.GetComponent<Dropdown>().ClearOptions();
        List<string> options = new List<string>();
        for (int i = 8; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
        }
        Resolutions.GetComponent<Dropdown>().AddOptions(options);
        Resolutions.GetComponent<Dropdown>().value = resolutions.Length - 7;
    }
    public void SaveSetting()
    {
        Resolution resolution = Screen.resolutions[Resolutions.GetComponent<Dropdown>().value + 8];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        if (FullScreen.GetComponent<Text>().text == "ON") Screen.fullScreen = true;
        else Screen.fullScreen = false;
        if (VSync.GetComponent<Text>().text == "ON") QualitySettings.vSyncCount = 1;
        else QualitySettings.vSyncCount = 0;
        QualitySettings.antiAliasing = (System.Convert.ToInt32(AntiAliasing.GetComponent<Dropdown>().options[AntiAliasing.GetComponent<Dropdown>().value].text));
        
    }
    public void ChangeTextMenu()
    {
        FullScreen.GetComponent<Text>().text=(FullScreen.GetComponent<Text>().text == "ON")? "OFF" : "ON";
    }
    public void ChangeTextVsync()
    {
        VSync.GetComponent<Text>().text = (VSync.GetComponent<Text>().text == "ON") ? "OFF" : "ON";
    }
}
