using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
    public GameObject controller;
    public GameObject currentMovingObject = null;
    public bool attack = false;
    public int hang, cot;
    public AudioClip MoveSound,EatSound;
    private AudioSource audioSource;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        controller = GameObject.FindGameObjectWithTag("GameController");
        if (attack) 
        {
            this.GetComponent<SpriteRenderer>().color = Color.red;
            audioSource.clip = EatSound;
        }
        else audioSource.clip = MoveSound;
    }
    public void OnMouseDown()
    {
        audioSource.Play();
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
        controlScript.RemoveMovePlates();
        //Destroy the move plates including self after the sound has played
        StartCoroutine(WaitAndDestroy(audioSource.clip.length, controlScript));
        controlScript.NextTurn();
    }
    IEnumerator WaitAndDestroy(float waitTime,Game controlScript)
    {
        yield return new WaitForSeconds(waitTime); // Chờ cho đến khi clip âm thanh kết thúc
        controlScript.DestroyMovePlates();
    }
}
