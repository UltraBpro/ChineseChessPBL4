using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    public Texture2D mouse0;
    public Texture2D mouse1;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor(mouse0, new Vector2(10, 0), CursorMode.Auto);
    }
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Cursor.SetCursor(mouse1, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            Thread.Sleep(100);
            Cursor.SetCursor(mouse0, new Vector2(10, 0), CursorMode.Auto);
        }
        
    }
    
}
