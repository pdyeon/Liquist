using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIadmin : MonoBehaviour {

    //public GameObject inventory;
    public GameObject menu;
    public GameObject setting;
    public GameObject gameover;

    public Image[] hpImg;

    public GameObject playerState;
    public GameObject hpBar;
    public GameObject[] inbottle1;
    public GameObject[] inbottle2;
    public GameObject[] inbottle3;

    public GameObject[] soundLV;

    public static UIadmin instance;

    private void Awake()
    {
        instance = this;

        //inventory.SetActive(false);
        menu.SetActive(false);
        setting.SetActive(false);
        
        playerState.SetActive(false);   //false로 변경후 PlayerEventPlus->NPC에서 true
        hpBar.SetActive(true);

        hpImg[0].gameObject.SetActive(true);
    }

}
