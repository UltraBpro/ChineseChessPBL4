using UnityEngine;

public class QuanCo : MonoBehaviour
{
    //public Sprite Goc,Up;
    public string TenQuanCo;

    public string TenThatCoUp;
    public int Team;
    public GameObject controller;
    public GameObject movePlate;
    public AudioClip ClickSound;
    private AudioSource audioSource;

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.volume = GlobalThings.SoundVolume;
        }
        controller = GameObject.FindGameObjectWithTag("GameController");
    }

    public void OnMouseDown()
    {
        if (GlobalThings.GameMode == 0 || controller.GetComponent<Game>().PlayingTeam == controller.GetComponent<Game>().myTeam)
            if (Team == controller.GetComponent<Game>().PlayingTeam)
            {
                audioSource.clip = ClickSound;
                audioSource.Play();
                controller.GetComponent<Game>().DestroyMovePlates();
                CreateMovePlates();
            }
    }

    public void MovePlateSpawn(int cotMovePl, int hangMovePl, bool attack = false)
    {
        Move tempmove = new Move(this.gameObject, (int)this.transform.position.x, (int)this.transform.position.y, cotMovePl, hangMovePl);
        if (IsMoveSafe(tempmove))
        {
            GameObject mp = Instantiate(movePlate, new Vector3(cotMovePl, hangMovePl, 0), Quaternion.identity);
            MovePlate mpScript = mp.GetComponent<MovePlate>();
            if (attack) mpScript.attack = true;
            mpScript.currentMovingObject = gameObject;
        }
    }

    public bool IsMoveSafe(Move move)
    {
        Game controllerScript = controller.GetComponent<Game>();
        int TempTeam = controllerScript.myTeam;
        controllerScript.myTeam = this.Team == 1 ? 2 : 1;
        int targetx = (int)move.targetPos.x;
        int targety = (int)move.targetPos.y;
        int oldx = (int)move.oldPos.x;
        int oldy = (int)move.oldPos.y;
        GameObject tempcapture = controllerScript.allTitle[targetx, targety];
        //TEMP MOVE
        controllerScript.allTitle[oldx, oldy] = null;
        controllerScript.allTitle[targetx, targety] = move.movingObj;
        controllerScript.NextTurn();
        // Call the minimax function
        var eval = controllerScript.Minimax(1, false, int.MinValue, int.MaxValue).Item2;
        controllerScript.allTitle[oldx, oldy] = move.movingObj;
        controllerScript.allTitle[targetx, targety] = tempcapture;
        controllerScript.NextTurn();
        controllerScript.myTeam = TempTeam;
        // If the evaluation is greater than a certain threshold, the move is not safe
        if (eval < -30000)
        {
            return false;
        }

        // Otherwise, the move is safe
        return true;
    }

    public void PointMovePlate(int cot, int hang)
    {
        if (cot >= 0 && cot <= 8 && hang >= 0 && hang <= 9)
        {
            Game controlscript = controller.GetComponent<Game>();

            if (controlscript.allTitle[cot, hang] != null)
            {
                if (controlscript.allTitle[cot, hang].GetComponent<QuanCo>().Team != this.Team)
                    MovePlateSpawn(cot, hang, true);
            }
            else MovePlateSpawn(cot, hang);
        }
    }

    public void CreateMovePlates()
    {
        switch (TenQuanCo)
        {
            case "vua":
                if (Team == 1) for (int y = (int)this.transform.position.y + 1; y < 10; y++)
                    {
                        if (controller.GetComponent<Game>().allTitle[(int)this.transform.position.x, y] != null)
                        {
                            if (controller.GetComponent<Game>().allTitle[(int)this.transform.position.x, y].GetComponent<QuanCo>().TenQuanCo == "vua") PointMovePlate((int)this.transform.position.x, y);
                            break;
                        }
                    }
                else for (int y = (int)this.transform.position.y - 1; y >= 0; y--)
                    {
                        if (controller.GetComponent<Game>().allTitle[(int)this.transform.position.x, y] != null)
                        {
                            if (controller.GetComponent<Game>().allTitle[(int)this.transform.position.x, y].GetComponent<QuanCo>().TenQuanCo == "vua") PointMovePlate((int)this.transform.position.x, y);
                            break;
                        }
                    }
                if ((Team == 1 && this.transform.position.y != 2) || Team == 2) PointMovePlate((int)this.transform.position.x, (int)this.transform.position.y + 1);
                if ((Team == 2 && this.transform.position.y != 7) || Team == 1) PointMovePlate((int)this.transform.position.x, (int)this.transform.position.y - 1);
                if (this.transform.position.x != 5) PointMovePlate((int)this.transform.position.x + 1, (int)this.transform.position.y);
                if (this.transform.position.x != 3) PointMovePlate((int)this.transform.position.x - 1, (int)this.transform.position.y);
                break;

            case "si":
                if (((this.transform.position.x != 5) && ((Team == 1 && this.transform.position.y != 2) || Team == 2)) || (GlobalThings.GameRule == 1 && TenThatCoUp == null)) PointMovePlate((int)this.transform.position.x + 1, (int)this.transform.position.y + 1);
                if (((this.transform.position.x != 3) && ((Team == 1 && this.transform.position.y != 2) || Team == 2)) || (GlobalThings.GameRule == 1 && TenThatCoUp == null)) PointMovePlate((int)this.transform.position.x - 1, (int)this.transform.position.y + 1);
                if (((this.transform.position.x != 3) && ((Team == 2 && this.transform.position.y != 7) || Team == 1)) || (GlobalThings.GameRule == 1 && TenThatCoUp == null)) PointMovePlate((int)this.transform.position.x - 1, (int)this.transform.position.y - 1);
                if (((this.transform.position.x != 5) && ((Team == 2 && this.transform.position.y != 7) || Team == 1)) || (GlobalThings.GameRule == 1 && TenThatCoUp == null)) PointMovePlate((int)this.transform.position.x + 1, (int)this.transform.position.y - 1);
                break;

            case "tuong":
                if (((Team == 1 && this.transform.position.y < 4) || Team == 2 || GlobalThings.GameRule == 1) && controller.GetComponent<Game>().CheckObjOnTitle((int)this.transform.position.x - 1, (int)this.transform.position.y + 1) == null) PointMovePlate((int)this.transform.position.x - 2, (int)this.transform.position.y + 2);
                if (((Team == 2 && this.transform.position.y > 5) || Team == 1 || GlobalThings.GameRule == 1) && controller.GetComponent<Game>().CheckObjOnTitle((int)this.transform.position.x - 1, (int)this.transform.position.y - 1) == null) PointMovePlate((int)this.transform.position.x - 2, (int)this.transform.position.y - 2);
                if (((Team == 2 && this.transform.position.y > 5) || Team == 1 || GlobalThings.GameRule == 1) && controller.GetComponent<Game>().CheckObjOnTitle((int)this.transform.position.x + 1, (int)this.transform.position.y - 1) == null) PointMovePlate((int)this.transform.position.x + 2, (int)this.transform.position.y - 2);
                if (((Team == 1 && this.transform.position.y < 4) || Team == 2 || GlobalThings.GameRule == 1) && controller.GetComponent<Game>().CheckObjOnTitle((int)this.transform.position.x + 1, (int)this.transform.position.y + 1) == null) PointMovePlate((int)this.transform.position.x + 2, (int)this.transform.position.y + 2);
                break;

            case "ma":
                if (controller.GetComponent<Game>().CheckObjOnTitle((int)this.transform.position.x - 1, (int)this.transform.position.y) == null)
                {
                    PointMovePlate((int)this.transform.position.x - 2, (int)this.transform.position.y - 1);
                    PointMovePlate((int)this.transform.position.x - 2, (int)this.transform.position.y + 1);
                }
                if (controller.GetComponent<Game>().CheckObjOnTitle((int)this.transform.position.x + 1, (int)this.transform.position.y) == null)
                {
                    PointMovePlate((int)this.transform.position.x + 2, (int)this.transform.position.y - 1);
                    PointMovePlate((int)this.transform.position.x + 2, (int)this.transform.position.y + 1);
                }
                if (controller.GetComponent<Game>().CheckObjOnTitle((int)this.transform.position.x, (int)this.transform.position.y - 1) == null)
                {
                    PointMovePlate((int)this.transform.position.x - 1, (int)this.transform.position.y - 2);
                    PointMovePlate((int)this.transform.position.x + 1, (int)this.transform.position.y - 2);
                }
                if (controller.GetComponent<Game>().CheckObjOnTitle((int)this.transform.position.x, (int)this.transform.position.y + 1) == null)
                {
                    PointMovePlate((int)this.transform.position.x - 1, (int)this.transform.position.y + 2);
                    PointMovePlate((int)this.transform.position.x + 1, (int)this.transform.position.y + 2);
                }
                break;

            case "xe":
                for (int x = (int)this.transform.position.x + 1; x < 9; x++)
                {
                    PointMovePlate(x, (int)this.transform.position.y);
                    if (controller.GetComponent<Game>().allTitle[x, (int)this.transform.position.y] != null) break;
                }
                for (int x = (int)this.transform.position.x - 1; x >= 0; x--)
                {
                    PointMovePlate(x, (int)this.transform.position.y);
                    if (controller.GetComponent<Game>().allTitle[x, (int)this.transform.position.y] != null) break;
                }
                for (int y = (int)this.transform.position.y + 1; y < 10; y++)
                {
                    PointMovePlate((int)this.transform.position.x, y);
                    if (controller.GetComponent<Game>().allTitle[(int)this.transform.position.x, y] != null) break;
                }
                for (int y = (int)this.transform.position.y - 1; y >= 0; y--)
                {
                    PointMovePlate((int)this.transform.position.x, y);
                    if (controller.GetComponent<Game>().allTitle[(int)this.transform.position.x, y] != null) break;
                }
                break;

            case "phao":
                for (int x = (int)this.transform.position.x + 1; x < 9; x++)
                {
                    if (controller.GetComponent<Game>().allTitle[x, (int)this.transform.position.y] != null)
                    {
                        for (int xphay = x + 1; xphay < 9; xphay++)
                        {
                            if (controller.GetComponent<Game>().allTitle[xphay, (int)this.transform.position.y] != null)
                            {
                                PointMovePlate(xphay, (int)this.transform.position.y);
                                break;
                            }
                        }
                        break;
                    }
                    PointMovePlate(x, (int)this.transform.position.y);
                }
                for (int x = (int)this.transform.position.x - 1; x >= 0; x--)
                {
                    if (controller.GetComponent<Game>().allTitle[x, (int)this.transform.position.y] != null)
                    {
                        for (int xphay = x - 1; xphay >= 0; xphay--)
                        {
                            if (controller.GetComponent<Game>().allTitle[xphay, (int)this.transform.position.y] != null)
                            {
                                PointMovePlate(xphay, (int)this.transform.position.y);
                                break;
                            }
                        }
                        break;
                    }
                    PointMovePlate(x, (int)this.transform.position.y);
                }
                for (int y = (int)this.transform.position.y + 1; y < 10; y++)
                {
                    if (controller.GetComponent<Game>().allTitle[(int)this.transform.position.x, y] != null)
                    {
                        for (int yphay = y + 1; yphay < 10; yphay++)
                        {
                            if (controller.GetComponent<Game>().allTitle[(int)this.transform.position.x, yphay] != null)
                            {
                                PointMovePlate((int)this.transform.position.x, yphay);
                                break;
                            }
                        }
                        break;
                    }
                    PointMovePlate((int)this.transform.position.x, y);
                }
                for (int y = (int)this.transform.position.y - 1; y >= 0; y--)
                {
                    if (controller.GetComponent<Game>().allTitle[(int)this.transform.position.x, y] != null)
                    {
                        for (int yphay = y - 1; yphay >= 0; yphay--)
                        {
                            if (controller.GetComponent<Game>().allTitle[(int)this.transform.position.x, yphay] != null)
                            {
                                PointMovePlate((int)this.transform.position.x, yphay);
                                break;
                            }
                        }
                        break;
                    }
                    PointMovePlate((int)this.transform.position.x, y);
                }
                break;

            case "tot":
                if (Team == 1) PointMovePlate((int)this.transform.position.x, (int)this.transform.position.y + 1);
                else PointMovePlate((int)this.transform.position.x, (int)this.transform.position.y - 1);
                if ((Team == 1 && transform.position.y >= 5) || (Team == 2 && transform.position.y <= 4))
                {
                    PointMovePlate((int)this.transform.position.x + 1, (int)this.transform.position.y);
                    PointMovePlate((int)this.transform.position.x - 1, (int)this.transform.position.y);
                }
                break;
        }
    }

    public void LoadSkin()
    {
        SpriteRenderer SpriteQuanCo = GetComponent<SpriteRenderer>();

        switch (TenQuanCo)
        {
            case "vua":
                SpriteQuanCo.sprite = Resources.Load<Sprite>("Sprites/Game/Skin" + GlobalThings.SkinID + "/0Vua" + (Team == 1 ? "Do" : "Den"));
                break;

            case "si":
                SpriteQuanCo.sprite = Resources.Load<Sprite>("Sprites/Game/Skin" + GlobalThings.SkinID + "/1Si" + (Team == 1 ? "Do" : "Den"));
                break;

            case "tuong":
                SpriteQuanCo.sprite = Resources.Load<Sprite>("Sprites/Game/Skin" + GlobalThings.SkinID + "/2Tuong" + (Team == 1 ? "Do" : "Den"));
                break;

            case "ma":
                SpriteQuanCo.sprite = Resources.Load<Sprite>("Sprites/Game/Skin" + GlobalThings.SkinID + "/3Ma" + (Team == 1 ? "Do" : "Den"));
                break;

            case "xe":
                SpriteQuanCo.sprite = Resources.Load<Sprite>("Sprites/Game/Skin" + GlobalThings.SkinID + "/4Xe" + (Team == 1 ? "Do" : "Den"));
                break;

            case "phao":
                SpriteQuanCo.sprite = Resources.Load<Sprite>("Sprites/Game/Skin" + GlobalThings.SkinID + "/5Phao" + (Team == 1 ? "Do" : "Den"));
                break;

            case "tot":
                SpriteQuanCo.sprite = Resources.Load<Sprite>("Sprites/Game/Skin" + GlobalThings.SkinID + "/6Tot" + (Team == 1 ? "Do" : "Den"));
                break;
        }
    }
}