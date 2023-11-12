using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public GameObject[,] allTitle = new GameObject[9, 10];
    public List<GameObject> P1 = new List<GameObject>();
    public List<GameObject> P2 = new List<GameObject>();
    public int PlayingTeam = 1, myTeam = 1;
    public AudioClip MoveSound, EatSound;
    public Move LastMove;
    private AudioSource audioSource;
    private int P1Jail = 0, P2Jail = 0;

    //biến UI
    public float TimeLimit = 120;

    private float TimeLeftThisTurn = 120;
    public GameObject ChatBoxTextOutput, ChatBoxTextInput;
    public GameObject RedTimer, RedAvatar, RedName, BlackTimer, BlackAvatar, BlackName;
    public GameObject ChatPanel;

    // Start is called before the first frame update
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.volume = GlobalThings.SoundVolume;
            GameObject.FindGameObjectWithTag("MusicPlayer").GetComponent<AudioSource>().volume = GlobalThings.MusicVolume;
        }
        foreach (GameObject i in P1) { allTitle[(int)i.transform.position.x, (int)i.transform.position.y] = i; i.GetComponent<QuanCo>().LoadSkin(); }
        foreach (GameObject i in P2) { allTitle[(int)i.transform.position.x, (int)i.transform.position.y] = i; i.GetComponent<QuanCo>().LoadSkin(); }
        if (GlobalThings.GameRule == 1) LoadCoUp();
        LoadPanel();
    }

    private void Update()
    {
        TimeLeftThisTurn -= Time.deltaTime;

        if (TimeLeftThisTurn <= 0)
        {
            MakeRandomMove();
        }
        else
        {
            int minutes = (int)(TimeLeftThisTurn / 60);
            int seconds = (int)(TimeLeftThisTurn % 60);
            if (PlayingTeam == 1) RedTimer.GetComponent<InputField>().text = String.Format("{0:D2}:{1:D2}", minutes, seconds);
            else BlackTimer.GetComponent<InputField>().text = String.Format("{0:D2}:{1:D2}", minutes, seconds);
        }
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

    public void DiChuyenQuan(GameObject currentMovingObject, int cot, int hang)
    {
        if(LastMove!=null)LastMove.movingObj.GetComponent<SpriteRenderer>().color = Color.white;
        LastMove = new Move
        {
            movingObj = currentMovingObject,
            oldPos = new Vector3((int)currentMovingObject.transform.position.x, (int)currentMovingObject.transform.position.y, 0),
            targetPos = new Vector3(cot, hang, 0),
            PreMove = LastMove,
            capturedObj = null
        };
        if (allTitle[cot, hang] != null)
        {
            LastMove.capturedObj = allTitle[cot, hang];
            DietQuan(cot, hang);
            audioSource.clip = EatSound;

        }
        else 
        { 
            audioSource.clip = MoveSound;
        }
        audioSource.Play();
        if(currentMovingObject.GetComponent<QuanCo>().Team==1) currentMovingObject.GetComponent<SpriteRenderer>().color = new Color(1, 0.6f, 0.6f);
        else currentMovingObject.GetComponent<SpriteRenderer>().color = new Color(0.6f, 1, 0.6f);
        if (allTitle[(int)currentMovingObject.transform.position.x, (int)currentMovingObject.transform.position.y] == currentMovingObject) allTitle[(int)currentMovingObject.transform.position.x, (int)currentMovingObject.transform.position.y] = null;
        currentMovingObject.transform.position = new Vector3(cot, hang, 0);
        //Update the matrix
        allTitle[cot, hang] = currentMovingObject;
        //Check chiếu hết
        if (PlayingTeam == 2) PlayingTeam = 1;
        else PlayingTeam++;
        if (CheckCheckMate()) Debug.Log("Stop game");
        if (PlayingTeam == 2) PlayingTeam = 1;
        else PlayingTeam++;
        //Dat timer
        TimeLeftThisTurn = TimeLimit;
        //extra
        QuanCo concodangdi = currentMovingObject.GetComponent<QuanCo>();
        if (GlobalThings.GameRule == 1)
            if (concodangdi.TenThatCoUp != null)
            {
                concodangdi.TenQuanCo = concodangdi.TenThatCoUp; concodangdi.TenThatCoUp = null;
                concodangdi.LoadSkin();
            }
    }

    public void DietQuan(int cot, int hang)
    {
        GameObject cp = allTitle[cot, hang];
        QuanCo loaiQuan = cp.GetComponent<QuanCo>();
        allTitle[cot, hang] = null;
        //if (loaiQuan.TenQuanCo == "vua")
        //{
        //    if (loaiQuan.Team == 1) Debug.Log("Player 2 win");
        //    else Debug.Log("Player 1 win");
        //}
        cp.transform.position = new Vector3(0.5f * (loaiQuan.Team == 1 ? P1Jail++ : P2Jail++), loaiQuan.Team == 1 ? 10 : -1, loaiQuan.Team == 1 ? -P1Jail : -P2Jail);
        cp.GetComponent<BoxCollider2D>().enabled = false;
    }

    public void HoanCo()
    {
        if (LastMove != null)
        {
            LastMove.movingObj.GetComponent<SpriteRenderer>().color = Color.white;
            if (LastMove.capturedObj != null)
            {
                LastMove.capturedObj.transform.position = LastMove.targetPos;
                allTitle[(int)LastMove.targetPos.x, (int)LastMove.targetPos.y] = LastMove.capturedObj;
                LastMove.capturedObj.GetComponent<BoxCollider2D>().enabled = true;
                if (LastMove.capturedObj.GetComponent<QuanCo>().Team == 1) P1Jail--;
                else P2Jail--;
            }
            if (allTitle[(int)LastMove.movingObj.transform.position.x, (int)LastMove.movingObj.transform.position.y] == LastMove.movingObj) allTitle[(int)LastMove.movingObj.transform.position.x, (int)LastMove.movingObj.transform.position.y] = null;
            LastMove.movingObj.transform.position = LastMove.oldPos;
            //Update the matrix
            allTitle[(int)LastMove.oldPos.x, (int)LastMove.oldPos.y] = LastMove.movingObj;
            LastMove = LastMove.PreMove;
            NextTurn();
            DestroyMovePlates();
        }
    }

    public void NextTurn()
    {
        if (PlayingTeam == 2) PlayingTeam = 1;
        else PlayingTeam++;
    }

    public bool CheckCheckMate()
    {
        List<Move> allmoves = AllPossibleMove();
        foreach (Move movetocheck in allmoves)
        {
            if (movetocheck.movingObj.GetComponent<QuanCo>().IsMoveSafe(movetocheck)) return false;
        }
        return true;
    }

    public bool CheckEndGame()
    {
        int founded = 0;
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (allTitle[i, j] != null)
                    if (allTitle[i, j].tag == "VuaDen" || allTitle[i, j].tag == "VuaDo")
                    {
                        founded++;
                    }
            }
        }
        return founded != 2;
    }

    public void MakeRandomMove()
    {
        List<Move> allmoves = AllPossibleMove();
        for (int i = allmoves.Count - 1; i >= 0; i--)
        {
            if (!allmoves[i].movingObj.GetComponent<QuanCo>().IsMoveSafe(allmoves[i]))
            {
                allmoves.RemoveAt(i);
            }
        }
        Move randomMove = allmoves[(new System.Random()).Next(allmoves.Count)];
        DiChuyenQuan(randomMove.movingObj, (int)randomMove.targetPos.x, (int)randomMove.targetPos.y);
        NextTurn();
        if (GlobalThings.GameMode == 1) BotPlay();
    }

    public GameObject CheckObjOnTitle(int cot, int hang)
    {
        if (cot < 0 || cot > 8 || hang < 0 || hang > 9) return null;
        return allTitle[cot, hang];
    }

    public void LoadCoUp()
    {
        List<string> names = new List<string> { "si", "tuong", "ma", "xe","phao",
                                                "si", "tuong", "ma", "xe","phao",
                                                "tot","tot","tot","tot","tot",
        };
        for (int i = 1; i < P1.Count; i++)
        {
            int index = UnityEngine.Random.Range(0, names.Count - 1);
            string randomName = names[index];
            P1[i].GetComponent<QuanCo>().TenThatCoUp = randomName;
            P1[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Game/Skin" + GlobalThings.SkinID + "/BlankDo");
            names.RemoveAt(index);
        }
        names = new List<string>{ "si", "tuong", "ma", "xe","phao",
                                "si", "tuong", "ma", "xe","phao",
                                "tot","tot","tot","tot","tot",
        };
        for (int i = 1; i < P2.Count; i++)
        {
            int index = UnityEngine.Random.Range(0, names.Count - 1);
            string randomName = names[index];
            P2[i].GetComponent<QuanCo>().TenThatCoUp = randomName;
            P2[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Game/Skin" + GlobalThings.SkinID + "/BlankDen");
            names.RemoveAt(index);
        }
        if (GlobalThings.GameMode == 2)
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < P1.Count; i++) temp.Add(P1[i].GetComponent<QuanCo>().TenThatCoUp);
            for (int i = 0; i < P2.Count; i++) temp.Add(P2[i].GetComponent<QuanCo>().TenThatCoUp);
            SendCoUpToOnl(temp);
        }
    }

    public void SendCoUpToOnl(List<string> allname)
    {
        string allnameonboard = "";
        foreach (string name in allname) allnameonboard += "|" + name;
        GameClient.instance.GuiDenSV(Encoding.UTF8.GetBytes(GameClient.instance.idDoiPhuong + "|LOADCOUP" + allnameonboard));
    }

    public void LoadCoUpFromOnl(List<string> allname)
    {
        for (int i = 0; i < P1.Count; i++)
        {
            P1[i].GetComponent<QuanCo>().TenThatCoUp = allname[0];
            allname.RemoveAt(0);
            P1[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Game/Skin" + GlobalThings.SkinID + "/BlankDo");
        }
        for (int i = 0; i < P2.Count; i++)
        {
            P2[i].GetComponent<QuanCo>().TenThatCoUp = allname[0];
            allname.RemoveAt(0);
            P2[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Game/Skin" + GlobalThings.SkinID + "/BlankDen");
        }
    }

    public void LoadPanel()
    {
        if (GlobalThings.GameMode == 2)
        {
            ChatPanel.SetActive(true);
            if (myTeam == 1)
            {
                RedName.GetComponent<InputField>().text = GameClient.instance.CurrentAccount.username;
                BlackName.GetComponent<InputField>().text = GameClient.instance.EnemyAccount.username;
            }
            else
            {
                BlackName.GetComponent<InputField>().text = GameClient.instance.CurrentAccount.username;
                RedName.GetComponent<InputField>().text = GameClient.instance.EnemyAccount.username;
            }
        }
        if (GlobalThings.GameMode == 1)
        {
            string botname = "MASTER";
            switch (GlobalThings.BotLevel)
            {
                case 2:
                    botname = "NOOB";
                    break;
                case 3:
                    botname = "SENIOR";
                    break;
                case 4:
                    botname = "MASTER";
                    break;
            }
            if (myTeam == 1)
            {
                BlackName.GetComponent<InputField>().text = "BOT " + botname;
                BlackAvatar.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/UI/AvatarBot" + botname);
            }
            else
            {
                RedName.GetComponent<InputField>().text = "BOT " + botname;
                RedAvatar.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/UI/AvatarBot" + botname);
            }
        }
    }

    #region BOT'S SHITSSSSSSSSS

    public void BotPlay()
    {
        Move botmove = Minimax(GlobalThings.BotLevel, true, int.MinValue, int.MaxValue).Item1;
        DiChuyenQuan(botmove.movingObj, (int)botmove.targetPos.x, (int)botmove.targetPos.y);
        NextTurn();
    }

    public int BoardEvaluation()
    {
        int score = 0;
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (allTitle[i, j] != null)
                {
                    QuanCo piece = allTitle[i, j].GetComponent<QuanCo>();
                    if (piece != null)
                    {
                        int chieudai = 9 - (myTeam != 1 ? 0 : j);
                        int scoretoadd = 0;
                        switch (piece.TenQuanCo)
                        {
                            case "tot":
                                scoretoadd += 30;
                                scoretoadd += GlobalThings.totEvalRed[i, chieudai];
                                if (piece.Team == myTeam) score -= scoretoadd;
                                else score += scoretoadd;
                                break;

                            case "si":
                                scoretoadd += 120 + GlobalThings.siEvalRed[i, chieudai];
                                if (piece.Team == myTeam) score -= scoretoadd;
                                else score += scoretoadd;
                                break;

                            case "tuong":
                                scoretoadd += 120 + GlobalThings.tuongEvalRed[i, chieudai];
                                if (piece.Team == myTeam) score -= scoretoadd;
                                else score += scoretoadd;
                                break;

                            case "ma":
                                scoretoadd += 270 + GlobalThings.maEvalRed[i, chieudai];
                                if (piece.Team == myTeam) score -= scoretoadd;
                                else score += scoretoadd;
                                break;

                            case "phao":
                                scoretoadd += 400 + GlobalThings.phaoEvalRed[i, chieudai];
                                if (piece.Team == myTeam) score -= scoretoadd;
                                else score += scoretoadd;
                                break;

                            case "xe":
                                scoretoadd += 600 + GlobalThings.xeEvalRed[i, chieudai];
                                if (piece.Team == myTeam) score -= scoretoadd;
                                else score += scoretoadd;
                                break;

                            case "vua":
                                scoretoadd += 60000 + GlobalThings.vuaEvalRed[i, chieudai];
                                if (piece.Team == myTeam) score -= scoretoadd;
                                else score += scoretoadd;
                                break;
                        }
                    }
                }
            }
        }
        return score;
    }

    public List<Move> AllPossibleMove()
    {
        List<Move> allMoves = new List<Move>();
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (allTitle[i, j] != null)
                {
                    QuanCo piece = allTitle[i, j].GetComponent<QuanCo>();
                    if (piece != null && piece.Team == PlayingTeam)
                    {
                        // Add these moves to the list of all moves
                        allMoves.AddRange(GetPieceMoves(piece, i, j));
                    }
                }
            }
        }
        return allMoves;
    }

    public List<Move> GetPieceMoves(QuanCo piece, int xTruyen, int yTruyen)
    {
        List<Move> MovesChoQuanNay = new List<Move>();
        switch (piece.TenQuanCo)
        {
            case "vua":
                if (piece.Team == 1) for (int y = yTruyen + 1; y < 10; y++)
                    {
                        if (allTitle[xTruyen, y] != null)
                        {
                            if (allTitle[xTruyen, y].GetComponent<QuanCo>().TenQuanCo == "vua") MovesChoQuanNay.Add(new Move(piece.gameObject, xTruyen, yTruyen, xTruyen, y));
                            break;
                        }
                    }
                else for (int y = yTruyen - 1; y >= 0; y--)
                    {
                        if (allTitle[xTruyen, y] != null)
                        {
                            if (allTitle[xTruyen, y].GetComponent<QuanCo>().TenQuanCo == "vua") MovesChoQuanNay.Add(new Move(piece.gameObject, xTruyen, yTruyen, xTruyen, y));
                            break;
                        }
                    }
                if ((piece.Team == 1 && yTruyen != 2) || piece.Team == 2) MovesChoQuanNay.Add(new Move(piece.gameObject, xTruyen, yTruyen, xTruyen, yTruyen + 1));
                if ((piece.Team == 2 && yTruyen != 7) || piece.Team == 1) MovesChoQuanNay.Add(new Move(piece.gameObject, xTruyen, yTruyen, xTruyen, yTruyen - 1));
                if (xTruyen != 5) MovesChoQuanNay.Add(new Move(piece.gameObject, xTruyen, yTruyen, xTruyen + 1, yTruyen));
                if (xTruyen != 3) MovesChoQuanNay.Add(new Move(piece.gameObject, xTruyen, yTruyen, xTruyen - 1, yTruyen));
                break;

            case "si":
                if (((xTruyen != 5) && ((piece.Team == 1 && yTruyen != 2) || piece.Team == 2)) || (GlobalThings.GameRule == 1 && piece.TenThatCoUp == null)) MovesChoQuanNay.Add(new Move(piece.gameObject, xTruyen, yTruyen, xTruyen + 1, yTruyen + 1));
                if (((xTruyen != 3) && ((piece.Team == 1 && yTruyen != 2) || piece.Team == 2)) || (GlobalThings.GameRule == 1 && piece.TenThatCoUp == null)) MovesChoQuanNay.Add(new Move(piece.gameObject, xTruyen, yTruyen, xTruyen - 1, yTruyen + 1));
                if (((xTruyen != 3) && ((piece.Team == 2 && yTruyen != 7) || piece.Team == 1)) || (GlobalThings.GameRule == 1 && piece.TenThatCoUp == null)) MovesChoQuanNay.Add(new Move(piece.gameObject, xTruyen, yTruyen, xTruyen - 1, yTruyen - 1));
                if (((xTruyen != 5) && ((piece.Team == 2 && yTruyen != 7) || piece.Team == 1)) || (GlobalThings.GameRule == 1 && piece.TenThatCoUp == null)) MovesChoQuanNay.Add(new Move(piece.gameObject, xTruyen, yTruyen, xTruyen + 1, yTruyen - 1));
                break;

            case "tuong":
                if (((piece.Team == 1 && yTruyen < 4) || piece.Team == 2 || GlobalThings.GameRule == 1) && CheckObjOnTitle(xTruyen - 1, yTruyen + 1) == null) MovesChoQuanNay.Add(new Move(piece.gameObject, xTruyen, yTruyen, xTruyen - 2, yTruyen + 2));
                if (((piece.Team == 2 && yTruyen > 5) || piece.Team == 1 || GlobalThings.GameRule == 1) && CheckObjOnTitle(xTruyen - 1, yTruyen - 1) == null) MovesChoQuanNay.Add(new Move(piece.gameObject, xTruyen, yTruyen, xTruyen - 2, yTruyen - 2));
                if (((piece.Team == 2 && yTruyen > 5) || piece.Team == 1 || GlobalThings.GameRule == 1) && CheckObjOnTitle(xTruyen + 1, yTruyen - 1) == null) MovesChoQuanNay.Add(new Move(piece.gameObject, xTruyen, yTruyen, xTruyen + 2, yTruyen - 2));
                if (((piece.Team == 1 && yTruyen < 4) || piece.Team == 2 || GlobalThings.GameRule == 1) && CheckObjOnTitle(xTruyen + 1, yTruyen + 1) == null) MovesChoQuanNay.Add(new Move(piece.gameObject, xTruyen, yTruyen, xTruyen + 2, yTruyen + 2));
                break;

            case "tot":
                if (piece.Team == 1) MovesChoQuanNay.Add(new Move(piece.gameObject, xTruyen, yTruyen, xTruyen, yTruyen + 1));
                else MovesChoQuanNay.Add(new Move(piece.gameObject, xTruyen, yTruyen, xTruyen, yTruyen - 1));
                if ((piece.Team == 1 && yTruyen >= 5) || (piece.Team == 2 && yTruyen <= 4))
                {
                    MovesChoQuanNay.Add(new Move(piece.gameObject, xTruyen, yTruyen, xTruyen + 1, yTruyen));
                    MovesChoQuanNay.Add(new Move(piece.gameObject, xTruyen, yTruyen, xTruyen - 1, yTruyen));
                }
                break;

            case "ma":
                if (CheckObjOnTitle(xTruyen - 1, yTruyen) == null)
                {
                    MovesChoQuanNay.Add(new Move(piece.gameObject, xTruyen, yTruyen, xTruyen - 2, yTruyen - 1));
                    MovesChoQuanNay.Add(new Move(piece.gameObject, xTruyen, yTruyen, xTruyen - 2, yTruyen + 1));
                }
                if (CheckObjOnTitle(xTruyen + 1, yTruyen) == null)
                {
                    MovesChoQuanNay.Add(new Move(piece.gameObject, xTruyen, yTruyen, xTruyen + 2, yTruyen - 1));
                    MovesChoQuanNay.Add(new Move(piece.gameObject, xTruyen, yTruyen, xTruyen + 2, yTruyen + 1));
                }
                if (CheckObjOnTitle(xTruyen, yTruyen - 1) == null)
                {
                    MovesChoQuanNay.Add(new Move(piece.gameObject, xTruyen, yTruyen, xTruyen - 1, yTruyen - 2));
                    MovesChoQuanNay.Add(new Move(piece.gameObject, xTruyen, yTruyen, xTruyen + 1, yTruyen - 2));
                }
                if (CheckObjOnTitle(xTruyen, yTruyen + 1) == null)
                {
                    MovesChoQuanNay.Add(new Move(piece.gameObject, xTruyen, yTruyen, xTruyen - 1, yTruyen + 2));
                    MovesChoQuanNay.Add(new Move(piece.gameObject, xTruyen, yTruyen, xTruyen + 1, yTruyen + 2));
                }
                break;

            case "xe":
                for (int x = xTruyen + 1; x < 9; x++)
                {
                    MovesChoQuanNay.Add(new Move(piece.gameObject, xTruyen, yTruyen, x, yTruyen));
                    if (allTitle[x, yTruyen] != null) break;
                }
                for (int x = (int)xTruyen - 1; x >= 0; x--)
                {
                    MovesChoQuanNay.Add(new Move(piece.gameObject, xTruyen, yTruyen, x, yTruyen));
                    if (allTitle[x, yTruyen] != null) break;
                }
                for (int y = (int)yTruyen + 1; y < 10; y++)
                {
                    MovesChoQuanNay.Add(new Move(piece.gameObject, xTruyen, yTruyen, xTruyen, y));
                    if (allTitle[xTruyen, y] != null) break;
                }
                for (int y = (int)yTruyen - 1; y >= 0; y--)
                {
                    MovesChoQuanNay.Add(new Move(piece.gameObject, xTruyen, yTruyen, xTruyen, y));
                    if (allTitle[xTruyen, y] != null) break;
                }
                break;

            case "phao":
                for (int x = xTruyen + 1; x < 9; x++)
                {
                    if (allTitle[x, yTruyen] != null)
                    {
                        for (int xphay = x + 1; xphay < 9; xphay++)
                        {
                            if (allTitle[xphay, yTruyen] != null)
                            {
                                MovesChoQuanNay.Add(new Move(piece.gameObject, xTruyen, yTruyen, xphay, yTruyen));
                                break;
                            }
                        }
                        break;
                    }
                    MovesChoQuanNay.Add(new Move(piece.gameObject, xTruyen, yTruyen, x, yTruyen));
                }
                for (int x = xTruyen - 1; x >= 0; x--)
                {
                    if (allTitle[x, yTruyen] != null)
                    {
                        for (int xphay = x - 1; xphay >= 0; xphay--)
                        {
                            if (allTitle[xphay, yTruyen] != null)
                            {
                                MovesChoQuanNay.Add(new Move(piece.gameObject, xTruyen, yTruyen, xphay, yTruyen));
                                break;
                            }
                        }
                        break;
                    }
                    MovesChoQuanNay.Add(new Move(piece.gameObject, xTruyen, yTruyen, x, yTruyen));
                }
                for (int y = yTruyen + 1; y < 10; y++)
                {
                    if (allTitle[xTruyen, y] != null)
                    {
                        for (int yphay = y + 1; yphay < 10; yphay++)
                        {
                            if (allTitle[xTruyen, yphay] != null)
                            {
                                MovesChoQuanNay.Add(new Move(piece.gameObject, xTruyen, yTruyen, xTruyen, yphay));
                                break;
                            }
                        }
                        break;
                    }
                    MovesChoQuanNay.Add(new Move(piece.gameObject, xTruyen, yTruyen, xTruyen, y));
                }
                for (int y = yTruyen - 1; y >= 0; y--)
                {
                    if (allTitle[xTruyen, y] != null)
                    {
                        for (int yphay = y - 1; yphay >= 0; yphay--)
                        {
                            if (allTitle[xTruyen, yphay] != null)
                            {
                                MovesChoQuanNay.Add(new Move(piece.gameObject, xTruyen, yTruyen, xTruyen, yphay));
                                break;
                            }
                        }
                        break;
                    }
                    MovesChoQuanNay.Add(new Move(piece.gameObject, xTruyen, yTruyen, xTruyen, y));
                }
                break;
        }
        for (int i = MovesChoQuanNay.Count - 1; i >= 0; i--)
        {
            Move move = MovesChoQuanNay[i];
            if (!(move.targetPos.x >= 0 && move.targetPos.x <= 8 && move.targetPos.y >= 0 && move.targetPos.y <= 9) || (move.capturedObj != null && move.movingObj.GetComponent<QuanCo>().Team == move.capturedObj.GetComponent<QuanCo>().Team))
            {
                MovesChoQuanNay.RemoveAt(i);
            }
        }
        return MovesChoQuanNay;
    }

    /*
                    int targetx = (int)move.targetPos.x;
                    int targety = (int)move.targetPos.y;
                    int oldx = (int)move.oldPos.x;
                    int oldy = (int)move.oldPos.y;
                    GameObject tempcapture = allTitle[targetx, targety];
                    //TEMP MOVE
                    allTitle[oldx, oldy] = null;
                    allTitle[targetx, targety] = move.movingObj;
                    NextTurn();
    */
    /*
                        allTitle[oldx, oldy] = move.movingObj;
                        allTitle[targetx, targety] = tempcapture;
    */

    public (Move, int) Minimax(int depth, bool maximizingPlayer, int alpha, int beta)
    {
        if (depth == 0 || CheckEndGame())
        {
            return (null, BoardEvaluation());
        }

        List<Move> allmoves = AllPossibleMove();
        Move bestMove = allmoves[(new System.Random()).Next(allmoves.Count)];

        if (maximizingPlayer)
        {
            int maxEval = int.MinValue;
            foreach (Move move in allmoves)
            {
                int targetx = (int)move.targetPos.x;
                int targety = (int)move.targetPos.y;
                int oldx = (int)move.oldPos.x;
                int oldy = (int)move.oldPos.y;
                GameObject tempcapture = allTitle[targetx, targety];
                //TEMP MOVE
                allTitle[oldx, oldy] = null;
                allTitle[targetx, targety] = move.movingObj;
                NextTurn();
                var currentEval = Minimax(depth - 1, false, alpha, beta).Item2;
                allTitle[oldx, oldy] = move.movingObj;
                allTitle[targetx, targety] = tempcapture;
                NextTurn();
                if (currentEval > maxEval)
                {
                    maxEval = currentEval;
                    bestMove = move;
                }
                alpha = Math.Max(alpha, currentEval);
                if (beta <= alpha)
                {
                    break;
                }
            }
            return (bestMove, maxEval);
        }
        else
        {
            var minEval = int.MaxValue;
            foreach (Move move in allmoves)
            {
                int targetx = (int)move.targetPos.x;
                int targety = (int)move.targetPos.y;
                int oldx = (int)move.oldPos.x;
                int oldy = (int)move.oldPos.y;
                GameObject tempcapture = allTitle[targetx, targety];
                //TEMP MOVE
                allTitle[oldx, oldy] = null;
                allTitle[targetx, targety] = move.movingObj;
                NextTurn();
                var currentEval = Minimax(depth - 1, true, alpha, beta).Item2;
                allTitle[oldx, oldy] = move.movingObj;
                allTitle[targetx, targety] = tempcapture;
                NextTurn();
                if (currentEval < minEval)
                {
                    minEval = currentEval;
                    bestMove = move;
                }
                beta = Math.Min(beta, currentEval);
                if (beta <= alpha)
                {
                    break;
                }
            }
            return (bestMove, minEval);
        }
    }

    #endregion BOT'S SHITSSSSSSSSS

    #region SE XOA SAU

    //TEMP SE XOA SAU
    public void Chat()
    {
        string content = "";
        if (GameClient.instance.CurrentAccount != null) content += GameClient.instance.CurrentAccount.username + ": ";
        content += ChatBoxTextInput.GetComponent<InputField>().text;
        ChatBoxTextInput.GetComponent<InputField>().text = "";
        GameClient.instance.GuiDenSV(Encoding.UTF8.GetBytes(GameClient.instance.idDoiPhuong + "|CHAT|" + content));
        ChatBoxTextOutput.GetComponent<Text>().text += "\n" + content;
    }

    public void TEMP()
    {
        GlobalThings.GameMode = 2;
        if (GlobalThings.GameRule == 0) GameClient.instance.GuiDenSV(Encoding.UTF8.GetBytes("MATCHMAKING|" + GameClient.instance.idDuocCap));
        else GameClient.instance.GuiDenSV(Encoding.UTF8.GetBytes("MATCHMAKINGCOUP|" + GameClient.instance.idDuocCap));
    }

    public void loadMainMenuToExit()
    {
        Camera.main.orthographic = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex-1);
        Debug.Log("Exit");
    }

    #endregion SE XOA SAU
}

public class Move
{
    public Vector3 oldPos;
    public GameObject movingObj;
    public Vector3 targetPos;
    public GameObject capturedObj = null;
    public Move PreMove = null;

    //For AI
    public int evaluation;

    public Move()
    {
    }

    public Move(GameObject moving, int xTruyen, int yTruyen, int x, int y)
    {
        oldPos = new Vector3((int)xTruyen, (int)yTruyen, 0);
        targetPos = new Vector3((int)x, (int)y, 0);
        movingObj = moving;
        capturedObj = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>().CheckObjOnTitle(x, y);
    }
}

public static class GlobalThings
{
    public static int GameMode = 0;
    public static int GameRule = 0;
    public static int BotLevel = 4;
    public static int SkinID = 2;
    public static float MusicVolume = 1;
    public static float SoundVolume = 1;

    #region valuePieces

    public static int[,] totEvalRed = new int[9, 10]
    {
{0, 0, 0, 0, 2, 6, 10, 14, 18, 0},
{0, 0, 0, 0, 0, 12, 20, 26, 36, 3},
{0, 0, 0, -2, 8, 18, 30, 42, 56, 6},
{0, 0, 0, 0, 0, 18, 34, 60, 80, 9},
{0 ,0 ,0 ,4 ,8 ,20 ,40 ,80 ,120 ,12},
{0 ,0 ,0 ,0 ,0 ,18 ,34 ,60 ,80 ,9},
{0 ,0 ,0 ,-2 ,8 ,18 ,30 ,42 ,56 ,6},
{0 ,0 ,0 ,0 ,0 ,12 ,20 ,26 ,36 ,3},
{0 ,0 ,0 ,0 ,2 ,6 ,10 ,14,18 ,0}
    };

    public static int[,] phaoEvalRed = new int[9, 10]
    {
{ 0, 0, 4, 0, -2, 0, 0, 2, 2, 6 },
{ 0, 2, 0, 0, 0, 0, 0, 2, 2, 4 },
{ 2, 4, 8, 0, 4, 0, -2, 0, 0, 0 },
{ 6, 6, 6, 2, 2, 2, 4, -10, -4, -10 },
{ 6, 6, 10, 4, 6, 8, 10, -8, -14, -12 },
{ 6, 6, 6, 2, 2, 2, 4, -10, -4, -10 },
{ 2, 4, 8, 0, 4, 0, -2, 0, 0, 0 },
{ 0, 2, 0, 0, 0, 0, 0, 2, 2, 4 },
{ 0, 0, 4, 0, -2, 0, 0, 2, 2, 6 }
    };

    public static int[,] xeEvalRed = new int[9, 10]
    {
{ -2, 8, 4, 6, 12,12 , 12, 12, 16, 14 },
{ 10, 4, 8, 10, 16, 14, 18,12, 20, 14 },
{ 6, 8, 6, 8, 14, 12, 16,12, 18, 12 },
{ 14, 16, 14, 14, 20, 18, 22,18, 24, 18 },
{ 12, 8, 12, 14, 20, 18, 22,18, 26, 16 },
{ 14, 16, 14, 14, 20, 18, 22,18, 24, 18 },
{ 6, 8, 6, 8, 14, 12, 16,12, 18, 12 },
{ 10, 4, 8, 10, 16, 14, 18,12, 20, 14 },
{ -2, 8, 4, 6, 12,12 , 12, 12, 16, 14 }
    };

    public static int[,] maEvalRed = new int[9, 10]
    {
{ 0,   0,   4,   2,   4,   6,   8,  12,   4,   4},
{-4,   2,   2,   6,  12,  16,  24,  14,   10,   8},
{0 ,4 ,8 ,8 ,16 ,14 ,18 ,16 ,28 ,16 },
{0 ,4 ,8 ,6 ,14 ,18 ,24 ,20 ,16 ,12 },
{0 , -2 ,4 ,10 ,12 ,16 ,20 ,18 ,8 ,4 },
{0 ,4 ,8 ,6 ,14 ,18 ,24 ,20 ,16 ,12 },
{0 ,4 ,8 ,8 ,16 ,14 ,18 ,16 ,28 ,16 },
{-4,   2,   2,   6,  12,  16,  24,  14,   10,   8},
{ 0,   0,   4,   2,   4,   6,   8,  12,   4,   4}
    };

    public static int[,] tuongEvalRed = new int[9, 10]
    {
{0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
{0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
{0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
{0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
{0, 0, 2, 0, 0, 0, 0, 0, 0, 0},
{0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
{0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
{0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
{0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
    };

    public static int[,] siEvalRed = new int[9, 10]
    {
{0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
{0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
{0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
{0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
{0, 2, 0, 0, 0, 0, 0, 0, 0, 0},
{0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
{0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
{0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
{0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
    };

    public static int[,] vuaEvalRed = new int[9, 10]
    {
{0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
{0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
{0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
{0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
{4, 2, 0, 0, 0, 0, 0, 0, 0, 0},
{0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
{0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
{0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
{0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
    };

    #endregion valuePieces
}