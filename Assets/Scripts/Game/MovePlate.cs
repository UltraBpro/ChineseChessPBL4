using UnityEngine;

public class MovePlate : MonoBehaviour
{
    public GameObject controller;
    public GameObject currentMovingObject = null;
    public bool attack = false;

    private void Start()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        if (attack)
        {
            this.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    public void OnMouseDown()
    {
        Game controlScript = controller.GetComponent<Game>();
        controlScript.DiChuyenQuan(currentMovingObject, (int)this.transform.position.x, (int)this.transform.position.y);
        controlScript.DestroyMovePlates();
        controlScript.NextTurn();
        if (GlobalThings.GameMode == 1) controlScript.BotPlay();
        string CMD = GameClient.instance.idDoiPhuong + "|MOVE|" + currentMovingObject.name + "|" + (int)this.transform.position.x + "|" + (int)this.transform.position.y;
        if (GlobalThings.GameMode == 2 && controlScript.PlayingTeam != controlScript.myTeam) GameClient.instance.GuiDenSV(System.Text.Encoding.UTF8.GetBytes(CMD));
        //if (GlobalThings.GameRule == 1)
        //{
        //    QuanCo quancoDangDiChuyen=currentMovingObject.GetComponent<QuanCo>();
        //    if (quancoDangDiChuyen.TenThatCoUp != null)
        //    {
        //        quancoDangDiChuyen.TenQuanCo = quancoDangDiChuyen.TenThatCoUp;
        //        quancoDangDiChuyen.TenThatCoUp = null;
        //    }
        //}
    }
}