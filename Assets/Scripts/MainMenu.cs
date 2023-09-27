using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void ConnectDenSV()
    {
        GameClient.instance.ConnectDenSV("127.0.0.1", 1006);
    }
}
