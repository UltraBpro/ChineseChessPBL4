using UnityEngine;

public class CursorController : MonoBehaviour
{
    public Texture2D mouse0;
    public Texture2D mouse1;
    public bool useCursor = true;
    public static CursorController instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this) Destroy(this);
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        if (useCursor)
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
        else
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        } 
            
    }

    public void NoCursor()
    {
        if (!useCursor)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
        useCursor = !useCursor;
    }
}