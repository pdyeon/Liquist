using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public bool isTel_Item = false;

    //public float monCurrentHP = 0;

    public int PlayerNum = 0;
    public float playerHP;
    //public int firePoint = 0;
    //public int icePoint = 0;
    public float playerAtkDmg = 30.0f;   //플레이어 기본 엉덩방아 데미지

    public Transform startPoint;

    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        switch(PlayerNum)
        {
            case 0:
                playerAtkDmg = 30.0f;
                break;
            case 1:
                playerAtkDmg = 100.0f;
                break;
            case 2:
                playerAtkDmg = 70.0f;
                break;
            case 3:
                playerAtkDmg = 200.0f;
                break;
        }
    }

    private void Start()
    {
        SoundManager.instance.PlayBGM(SoundManager.instance.prisonBgm, 0, true);
        StartCoroutine(GameStartIE());
    }

    //플레이어가 데미지를 입는함수
    //인자값으로 입는 데미지 값을 입력
    public void Damaged(int dmg)
    {
        //플레이어 스테이트값 "Damage"로 변경
        playerHP -= dmg;
    }

    IEnumerator GameStartIE()
    {
        GameObject.FindWithTag("Player").GetComponent<SpriteRenderer>().enabled = false;
        GameObject.FindWithTag("Player").GetComponent<PlayerControl>().enabled = false;
        GameObject.Find("Player_Start").GetComponent<Animator>().SetTrigger("GameStart");
        GameObject.Find("Player").transform.position = startPoint.transform.position;
        yield return new WaitForSeconds(1.3f);
        GameObject.Find("Player_Start").GetComponent<Animator>().SetTrigger("Idle");
        GameObject.FindWithTag("Player").GetComponent<SpriteRenderer>().enabled = true;
        GameObject.FindWithTag("Player").GetComponent<PlayerControl>().enabled = true;
    }

}
