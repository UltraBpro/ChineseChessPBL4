﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
    public GameObject controller;
    public GameObject currentMovingObject = null;
    public bool attack = false;
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
        controlScript.DiChuyenQuan(currentMovingObject, (int)this.transform.position.x, (int)this.transform.position.y, attack);
        controlScript.RemoveMovePlates();
        //Destroy the move plates including self after the sound has played
        StartCoroutine(WaitAndDestroy(audioSource.clip.length, controlScript));
        controlScript.NextTurn();
        string CMD = GameClient.instance.idDoiPhuong + "|MOVE|" + currentMovingObject.name + "|" + (int)this.transform.position.x + "|" + (int)this.transform.position.y+"|"+(attack?1:0);
        if (GlobalThings.GameMode == 2 && controlScript.PlayingTeam!=controlScript.myTeam) GameClient.instance.GuiDenSV(System.Text.Encoding.UTF8.GetBytes(CMD));
    }
    IEnumerator WaitAndDestroy(float waitTime,Game controlScript)
    {
        yield return new WaitForSeconds(waitTime); // Chờ cho đến khi clip âm thanh kết thúc
        controlScript.DestroyMovePlates();
    }
}