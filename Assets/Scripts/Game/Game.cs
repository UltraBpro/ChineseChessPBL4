using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class Game : MonoBehaviour
{
    public bool ChoiVoiBot = false;
    public GameObject[,] allTitle=new GameObject[9,10];
    public List<GameObject> P1 = new List<GameObject>();
    public List<GameObject> P2 = new List<GameObject>();
    public int PlayingTeam = 1;
    // Start is called before the first frame update
    private void Start()
    {
        foreach (GameObject i in P1) { allTitle[(int)i.transform.position.x, (int)i.transform.position.y] = i; }
        foreach (GameObject i in P2) { allTitle[(int)i.transform.position.x, (int)i.transform.position.y] = i; }
        
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
    public void RemoveMovePlates()
    {
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        for (int i = 0; i < movePlates.Length; i++)
        {
            movePlates[i].GetComponent<SpriteRenderer>().enabled = false;
        }
    }
    public void DiChuyenQuan(GameObject currentMovingObject, int cot,int hang,bool attack)
    {
        if (attack)
        {
            DietQuan(cot,hang);
        }
        allTitle[(int)currentMovingObject.transform.position.x, (int)currentMovingObject.transform.position.y] = null;
        currentMovingObject.transform.position = new Vector3(cot, hang);
        //Update the matrix
        allTitle[cot, hang] = currentMovingObject;
        if (currentMovingObject.GetComponent<QuanCo>().TenQuanCo == "tot")
        {
            QuanCo conTot = currentMovingObject.GetComponent<QuanCo>();
            if (conTot.Team == 1 && conTot.transform.position.y >= 5) conTot.TenQuanCo = "totsangxong";
            if (conTot.Team == 2 && conTot.transform.position.y <= 4) conTot.TenQuanCo = "totsangxong";
        }
    }
    public void DietQuan(int cot,int hang)
    {
        GameObject cp = allTitle[cot, hang];
        QuanCo loaiQuan = cp.GetComponent<QuanCo>();
        if (loaiQuan.Team == 1) P1.Remove(cp);
        else P2.Remove(cp);
        allTitle[cot, hang] = null;
        if (loaiQuan.TenQuanCo == "vua")
        {
            if (loaiQuan.Team == 1) Debug.Log("Player 2 win");
            else Debug.Log("Player 1 win");
        }
        Destroy(cp);
    }
    public void NextTurn()
    {
        if (PlayingTeam == 2) PlayingTeam = 1;
        else PlayingTeam ++;
    }
    public GameObject CheckObjOnTitle(int cot,int hang)
    {
        if (cot < 0 || cot > 8 || hang < 0 || hang > 9) return null;
        return allTitle[cot, hang];
    }
}
public static class GlobalFunctions
{
    
}