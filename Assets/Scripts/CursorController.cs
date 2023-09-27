using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    public Texture2D mouse0;
    public Texture2D mouse1;
    // Start is called before the first frame update
    private void Start()
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
            Cursor.SetCursor(mouse0, new Vector2(10, 0), CursorMode.Auto);
        }
        
    }
    
}
