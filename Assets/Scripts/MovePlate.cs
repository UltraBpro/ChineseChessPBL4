using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
    public GameObject controller;
    public GameObject currentMovingObject = null;
    public bool attack = false;
    public int hang, cot;
    private void Start()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        if (attack) this.GetComponent<SpriteRenderer>().color = Color.red;
    }
    public void OnMouseDown()
    {
        Game controlScript = controller.GetComponent<Game>();
        if (attack)
        {
            controlScript.DietQuan((int)this.transform.position.x, (int)this.transform.position.y);
        }
        controlScript.allTitle[(int)currentMovingObject.transform.position.x, (int)currentMovingObject.transform.position.y] = null;
        currentMovingObject.transform.position= this.transform.position;
        //Update the matrix
        controlScript.allTitle[(int)this.transform.position.x, (int)this.transform.position.y] = currentMovingObject;
        if (currentMovingObject.GetComponent<QuanCo>().TenQuanCo == "tot")
        {
            QuanCo conTot = currentMovingObject.GetComponent<QuanCo>();
            if (conTot.Team == 1 && conTot.transform.position.y >= 5) conTot.TenQuanCo = "totsangxong";
            if (conTot.Team == 2 && conTot.transform.position.y <= 4) conTot.TenQuanCo = "totsangxong";
        }
        //Destroy the move plates including self
        controlScript.DestroyMovePlates();
        controlScript.NextTurn();
    }
}
