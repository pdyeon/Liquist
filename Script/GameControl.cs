using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour {

    public static GameControl instance;

    public GameObject miniMap;
    bool miniMapSwitch = false;

    bool delayBottle = false;

    //2018-11-16 플레이어->브라더스 체인지
    int changeNum = 1;
    public bool isKeyOpen = false;
    bool isPlayerChange = false;
    public bool isEndingDoor = false;
    bool isChangePlayer = false;

    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update ()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            miniMapSwitch = !miniMapSwitch;
            miniMap.SetActive(miniMapSwitch);
        }
        MakeCanvas();
        //Inventory();
        Menu();
        Setting();
        CloseUI();

        ChangePlayer();
        UseItem();

        HPUpdate();
        ////////////브라더스 1~2키/////////////////
        
        if (Input.GetKeyDown("1") && isPlayerChange)
        {
            ChangePlayerReference();
            changeNum = 1;
        }
        else if (Input.GetKeyDown("2") && isPlayerChange)
        {
            ChangePlayerReference();
            changeNum = 2;
        }

        if (Input.GetKeyDown(KeyCode.E) && isChangePlayer) //플레이어로 돌아가는 함수
        {           
            BackToPlayer();
        }
        
        /////////////////////////////////////////////
    }

    void UseItem()
    {
        
        if (UIManager.instance.isFullbottle3)
        {
            if (Input.GetKeyDown(KeyCode.E))// && UIadmin.instance.playerState)  //2018-11-19
            {
                UIadmin.instance.inbottle3[UIManager.instance.bottle3Num].GetComponent<Animator>().SetTrigger("Use");
                SoundManager.instance.PlaySfx(transform.position, SoundManager.instance.itemUse, 0, SoundManager.instance.bgmVolum);
                Instantiate(PaticleManager.instance.player_itemuse.transform.gameObject, PlayerEvent.instance.effpos, PlayerEvent.instance.transform.rotation);
                switch (UIManager.instance.bottle3Num)
                {
                    case 0:
                        SoundManager.instance.PlaySfx(transform.position, SoundManager.instance.hpHeal, 0, SoundManager.instance.bgmVolum);
                        SoundManager.instance.PlaySfx(transform.position, SoundManager.instance.hpUp, 0, SoundManager.instance.bgmVolum);
                        GameManager.instance.playerHP = 130;

                        break;
                    case 1:

                        //PlayerControl.instance.animator.SetTrigger("SuperArmor");
                        StartCoroutine(InvinIE());

                        break;
                    case 2: //브라더스 보틀
                        Instantiate(PaticleManager.instance.player_brother_change.transform.gameObject, PlayerEvent.instance.effpos, PlayerEvent.instance.transform.rotation);
                        StartCoroutine(BrotherCutScene());  //--분열 확인
                        isPlayerChange = true;

                        break;
                    case 3: //키보틀

                        isKeyOpen = true; //PlayerEvent에서 문에 닿으면 문이 사라짐

                        break;
                    case 4:  //플라즈마 보틀

                        isEndingDoor = true;

                        break;
                }
                StartCoroutine(DelayDestroyBottleIE());
                UIManager.instance.isFullbottle3 = false;
            }
        }
        
    }
    
    IEnumerator InvinIE()
    {
        PlayerControl.instance.playermaterial.color = PaticleManager.instance.targetcolor;//new Color(255, 0, 0, 255);
        UIManager.instance.isInvin = true;

        if (UIManager.instance.isInvin)
        {
            yield return new WaitForSeconds(5.0f);
            PlayerControl.instance.playermaterial.color = PaticleManager.instance.initcolor; //new Color(255, 255, 255, 255);
            UIManager.instance.isInvin = false;
        }
    }
    
    IEnumerator DelayDestroyBottleIE()
    {
        yield return new WaitForSeconds(1.0f);
        delayBottle = true;
        if (delayBottle)
        {
            UIadmin.instance.inbottle3[UIManager.instance.bottle3Num].SetActive(false);
            UIManager.instance.bottle3Num = -1;
            delayBottle = false;
        }
    }

    public void OpenItemIcon(int num)
    {
        if (UIManager.instance.isFullbottle3) // 아이템 공간이 차있을때
        {
            UIadmin.instance.inbottle3[UIManager.instance.bottle3Num].SetActive(false);
        }
        UIManager.instance.bottle3Num = num;
        UIadmin.instance.inbottle3[num].SetActive(true);
        UIadmin.instance.inbottle3[num].GetComponent<Animator>().SetTrigger("Full");
        UIManager.instance.isFullbottle3 = true; 
    }

    void HPUpdate()
    {
        if (100 < GameManager.instance.playerHP)
        {
            UIadmin.instance.hpImg[0].gameObject.SetActive(false);
            UIadmin.instance.hpImg[1].gameObject.SetActive(false);
            UIadmin.instance.hpImg[2].gameObject.SetActive(false);
            UIadmin.instance.hpImg[3].gameObject.SetActive(true);
        }
        else if (GameManager.instance.playerHP <= 100)
        {
            UIadmin.instance.hpImg[0].gameObject.SetActive(true);
            UIadmin.instance.hpImg[1].gameObject.SetActive(false);
            UIadmin.instance.hpImg[2].gameObject.SetActive(false);
            UIadmin.instance.hpImg[3].gameObject.SetActive(false);

            UIadmin.instance.hpImg[0].fillAmount = GameManager.instance.playerHP / 100;

            if (GameManager.instance.playerHP <= 70)
            {
                UIadmin.instance.hpImg[0].gameObject.SetActive(false);
                UIadmin.instance.hpImg[1].gameObject.SetActive(true);
                UIadmin.instance.hpImg[2].gameObject.SetActive(false);
                UIadmin.instance.hpImg[3].gameObject.SetActive(false);

                UIadmin.instance.hpImg[1].fillAmount = GameManager.instance.playerHP / 100;

                if (GameManager.instance.playerHP <= 30)
                {
                    UIadmin.instance.hpImg[0].gameObject.SetActive(false);
                    UIadmin.instance.hpImg[1].gameObject.SetActive(false);
                    UIadmin.instance.hpImg[2].gameObject.SetActive(true);
                    UIadmin.instance.hpImg[3].gameObject.SetActive(false);

                    UIadmin.instance.hpImg[2].fillAmount = GameManager.instance.playerHP / 100;
                }
            }
        }

    }
    /*
    void Inventory()
    {
        if (Input.GetKeyDown(KeyCode.I) && UIManager.instance.isInvenOpen == false)
        {
            UIManager.instance.isInvenOpen = true;
            UIadmin.instance.inventory.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.I) && UIManager.instance.isInvenOpen == true)
        {           
            UIManager.instance.isInvenOpen = false;
            UIadmin.instance.inventory.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && UIManager.instance.isInvenOpen == true)
        {
            UIManager.instance.isInvenOpen = false;

            UIadmin.instance.inventory.SetActive(false);
        }
    }
    */

    void MakeCanvas()
    {
        if (UIManager.instance.isUIMake == false)
        {
            Instantiate(UIManager.instance.saveAllUI);
            UIManager.instance.isUIMake = true;
        }
    }

    void Menu()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && UIManager.instance.isUIAllClose == true)
        {
            SoundManager.instance.PlaySfx(transform.position, SoundManager.instance.button, 0, SoundManager.instance.bgmVolum);
            UIManager.instance.isMenuOpen = true;
            UIadmin.instance.menu.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && UIManager.instance.isMenuOpen == true)
        {
            SoundManager.instance.PlaySfx(transform.position, SoundManager.instance.button, 0, SoundManager.instance.bgmVolum);
            UIManager.instance.isMenuOpen = false;

            UIadmin.instance.menu.SetActive(false);
        }
    }

    void Setting()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && UIManager.instance.isSettingOpen == true)
        {
            SoundManager.instance.PlaySfx(transform.position, SoundManager.instance.button, 0, SoundManager.instance.bgmVolum);
            UIManager.instance.isSettingOpen = false;

            UIadmin.instance.setting.SetActive(false);
        }
    }

    void CloseUI()
    {
        if (UIManager.instance.isMenuOpen != true && UIManager.instance.isSettingOpen != true)//UIManager.instance.isInvenOpen != true && 
        {
            Time.timeScale = 1;
            UIManager.instance.isUIAllClose = true; 
        }

        else if (UIManager.instance.isMenuOpen != false || UIManager.instance.isSettingOpen != false)//UIManager.instance.isInvenOpen != false || 
        {
            Time.timeScale = 0;
            UIManager.instance.isUIAllClose = false;
        }
    }

    public void OpenMonIcon(int Num)
    {
        if (!UIManager.instance.isFullbottle1) // 처음것이 열려있을때
        {
            UIManager.instance.bottle1Num = Num - 1;
            UIadmin.instance.inbottle1[UIManager.instance.bottle1Num].SetActive(true);
            UIadmin.instance.inbottle1[UIManager.instance.bottle1Num].GetComponent<Animator>().SetTrigger("Full");
            UIManager.instance.isFullbottle1 = true;
        }
        else if (UIManager.instance.isFullbottle1) // 처음것이 차있을때
        {
            UIManager.instance.bottle2Num = UIManager.instance.bottle1Num;
            UIManager.instance.isFullbottle2 = UIManager.instance.isFullbottle1;
            UIManager.instance.isEmptybottle2 = UIManager.instance.isEmptybottle1;
            if (UIManager.instance.isFullbottle2 && !UIManager.instance.isEmptybottle2)
            {
                UIadmin.instance.inbottle2[UIManager.instance.bottle2Num].SetActive(true);
                UIadmin.instance.inbottle2[UIManager.instance.bottle2Num].GetComponent<Animator>().SetTrigger("Full");
            }
            UIManager.instance.bottle1Num = Num - 1;
            UIadmin.instance.inbottle1[UIManager.instance.bottle1Num].SetActive(true);
            UIadmin.instance.inbottle1[UIManager.instance.bottle1Num].GetComponent<Animator>().SetTrigger("Full");
            UIManager.instance.isFullbottle1 = true;
            // 기존것을 뒤로 옮기고 처음자리에 넣는다

        }
        
    }

    void ChangePlayer()
    {
        if (Input.GetKeyDown(KeyCode.Q)) // Q키를 눌렀을 때
        {
            if (UIManager.instance.bottle1Num != -1)
            {
                SoundManager.instance.PlaySfx(transform.position, SoundManager.instance.itemUse, 0, SoundManager.instance.bgmVolum);
                // Q를 다시 눌렀을때
                if (!UIManager.instance.isFullbottle1 && UIManager.instance.isEmptybottle1)
                {
                    UIManager.instance.isEmptybottle1 = false;
                    UIadmin.instance.inbottle1[UIManager.instance.bottle1Num].SetActive(false);
                    //PlayerControl.instance.animator.runtimeAnimatorController = PlayerEvent.instance.playeranimator[0]; //슬라임으로 변신
                    //GameManager.instance.PlayerNum = 0; //관련 스크립트실행
                    UIManager.instance.bottle1Num = -1; //데이터를 없애고 슬라임으로 돌린다.
                }

                else if (UIManager.instance.bottle2Num != -1)
                {
                    // W를 누른 상태일때
                    if (!UIManager.instance.isFullbottle2 && UIManager.instance.isEmptybottle2)
                    {
                        UIManager.instance.isEmptybottle2 = false;
                        //W칸 이미지 끄기
                        UIadmin.instance.inbottle2[UIManager.instance.bottle2Num].SetActive(false);
                        //W칸 데이터 삭제
                        UIManager.instance.bottle2Num = -1;
                    }
                }
                if (UIManager.instance.isFullbottle1 && !UIManager.instance.isEmptybottle1) // 처음 눌렀을때
                {
                    UIManager.instance.isFullbottle1 = false;
                    UIManager.instance.isEmptybottle1 = true;
                    UIadmin.instance.inbottle1[UIManager.instance.bottle1Num].GetComponent<Animator>().SetTrigger("Use");
                }
                //애니메이션 변신
                PlayerControl.instance.animator.runtimeAnimatorController = PlayerEvent.instance.playeranimator[UIManager.instance.bottle1Num + 1];
                //이팩트
                Instantiate(PaticleManager.instance.player_Change.transform.gameObject, PlayerEvent.instance.effpos, PlayerEvent.instance.transform.rotation);
                //변신 이미지 파티클 실행
                ChangePaticle.instance.ShowChangePaticle(UIManager.instance.bottle1Num + 1);
                //관련 스크립트 실행
                GameManager.instance.PlayerNum = UIManager.instance.bottle1Num + 1;
                //총알 변경
                BulletManager.instance.bulletNum = UIManager.instance.bottle1Num + 1;
            }
           
        }

        if (Input.GetKeyDown(KeyCode.W)) // W키를 눌렀을 때
        {
            if (UIManager.instance.bottle2Num != -1) 
            {
                SoundManager.instance.PlaySfx(transform.position, SoundManager.instance.itemUse, 0, SoundManager.instance.bgmVolum);
                // W를 다시 눌렀을때
                if (!UIManager.instance.isFullbottle2 && UIManager.instance.isEmptybottle2)
                {
                    UIManager.instance.isEmptybottle2 = false;
                    UIadmin.instance.inbottle2[UIManager.instance.bottle2Num].SetActive(false);
                    //PlayerControl.instance.animator.runtimeAnimatorController = PlayerEvent.instance.playeranimator[0]; //슬라임으로 변신
                    //GameManager.instance.PlayerNum = 0; //관련 스크립트실행

                    UIManager.instance.bottle2Num = -1;
                    //데이터를 없애고 슬라임으로 돌린다.
                }

                else if (UIManager.instance.bottle1Num != -1)
                {
                    // Q를 누른 상태일때
                    if (!UIManager.instance.isFullbottle1 && UIManager.instance.isEmptybottle1)
                    {
                        UIManager.instance.isEmptybottle1 = false;
                        //Q칸 이미지 끄기
                        UIadmin.instance.inbottle1[UIManager.instance.bottle1Num].SetActive(false);
                        //Q칸 데이터 삭제
                        UIManager.instance.bottle1Num = -1;
                    }
                }
                if (UIManager.instance.isFullbottle2 && !UIManager.instance.isEmptybottle2) // 처음 눌렀을때
                {
                    UIManager.instance.isFullbottle2 = false;
                    UIManager.instance.isEmptybottle2 = true;
                    UIadmin.instance.inbottle2[UIManager.instance.bottle2Num].GetComponent<Animator>().SetTrigger("Use");
                }
                //애니메이션 변신
                PlayerControl.instance.animator.runtimeAnimatorController = PlayerEvent.instance.playeranimator[UIManager.instance.bottle2Num + 1];
                //이팩트
                Instantiate(PaticleManager.instance.player_Change.transform.gameObject, PlayerEvent.instance.effpos, PlayerEvent.instance.transform.rotation);
                //변신 이미지 파티클 실행
                ChangePaticle.instance.ShowChangePaticle(UIManager.instance.bottle1Num + 1);
                //관련 스크립트 실행
                GameManager.instance.PlayerNum = UIManager.instance.bottle2Num + 1;
                //총알 변경
                BulletManager.instance.bulletNum = UIManager.instance.bottle2Num + 1;

            }
           
        }
    }

    void PlayerToBrothersChange()
    {
        //1.분열 이미지
        //2.원래 플레이어 setactive(false)
        //3-1.changeNum가 1번일시 주황 플레이어 
        //3-2.changeNum가 2번일시 초록 플레이어
        //4.카메라가 플레이어를 새로 지정
        //5.플레이    
        GameObject.Find("PlayerS").transform.GetChild(1).position
            = new Vector2(GameObject.Find("PlayerS").transform.GetChild(0).transform.position.x-5,
                          GameObject.Find("PlayerS").transform.GetChild(0).transform.position.y + 5);

        GameObject.Find("PlayerS").transform.GetChild(2).position
            = new Vector2(GameObject.Find("PlayerS").transform.GetChild(0).transform.position.x+5,
                          GameObject.Find("PlayerS").transform.GetChild(0).transform.position.y + 5);
        GameObject.Find("PlayerS").transform.GetChild(1).gameObject.SetActive(true);
        GameObject.Find("PlayerS").transform.GetChild(2).gameObject.SetActive(true);
        ChangePlayerReference();
    }

    IEnumerator BrotherCutScene()
    {
        GameObject.Find("Player").SetActive(false);
        GameObject.Find("PlayerS").transform.GetChild(3).gameObject.SetActive(true);
        GameObject.Find("PlayerS").transform.GetChild(3).transform.position =
                           new Vector2( GameObject.Find("PlayerS").transform.GetChild(0).transform.position.x,
                                        GameObject.Find("PlayerS").transform.GetChild(0).transform.position.y + 3);
        GameObject.Find("PlayerS").transform.GetChild(3).GetComponent<Animator>().SetTrigger("Play");
        yield return new WaitForSeconds(0.5f);
        GameObject.Find("PlayerS").transform.GetChild(3).gameObject.SetActive(false);
        PlayerToBrothersChange();
    }

    void ChangePlayerReference()
    {
        if(changeNum == 1)
        {
            FollowCam.instance.pl = GameObject.Find("PlayerS").transform.GetChild(1).transform;
            FollowCam.instance.player = GameObject.Find("Player_slime_Brother01");
            GameObject.Find("PlayerS").transform.GetChild(1).GetComponent<PlayerControl>().enabled = true;
            GameObject.Find("PlayerS").transform.GetChild(2).GetComponent<PlayerControl>().enabled = false;
        }
        else if(changeNum == 2)
        {
            FollowCam.instance.pl = GameObject.Find("PlayerS").transform.GetChild(2).transform;
            FollowCam.instance.player = GameObject.Find("Player_slime_Brother02");
            GameObject.Find("PlayerS").transform.GetChild(1).GetComponent<PlayerControl>().enabled = false;
            GameObject.Find("PlayerS").transform.GetChild(2).GetComponent<PlayerControl>().enabled = true;
        }
        isChangePlayer = true;
    }
 
    void BackToPlayer()
    {
        GameObject.Find("PlayerS").transform.GetChild(1).gameObject.SetActive(false);
        GameObject.Find("PlayerS").transform.GetChild(2).gameObject.SetActive(false);
        GameObject.Find("PlayerS").transform.GetChild(0).gameObject.SetActive(true);
        FollowCam.instance.pl = GameObject.Find("PlayerS").transform.GetChild(0).transform;
        FollowCam.instance.player = GameObject.Find("Player");
    }

}
