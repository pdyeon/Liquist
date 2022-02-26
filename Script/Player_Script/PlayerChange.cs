using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerChange : MonoBehaviour {

    public float eatOKHP;
    float eatdistance;
    bool isEat;

    bool isNinjaAttack;

    public bool isAttack = false;
    bool isTeleport = false;
    bool isHeal = false;

    public float teleportsize = 10.0f;

    public static PlayerChange instance;

    GameObject target;
    Vector2 tpos;

    public GameObject iceDoor;     //아이스문 프리팹 연동
    bool isEnd = false;   //게임이 끝났는지
    private void Awake()
    {
        instance = this;
        eatOKHP = 10f;
        eatdistance = 1f;
        isEat = false;
        
        isNinjaAttack = false;
 
    }

    //2018-11-12 플레이어->브라더스 체인지
    int changeNum = 0;

    // Update is called once per frame
    void Update()
    {
        PlayerState();
        //PlayerToBrothersChange();
    }

    private void FixedUpdate()
    {
        

        if (isNinjaAttack)
        {
            Grab(target);
            StartCoroutine(GrabDelay());
        }
    }

    void PlayerState()
    {
        switch (GameManager.instance.PlayerNum)
        {
            case 0: // 슬라임
                EatRange();
                break;
            case 1: // 라바레이
                LavaShortAttack();
                LavaLongAttack();
                break;
            case 2: // 닌자
                NinjaShortAttack();
                NinjaLongAttack();
                break;
            case 3: // 스니키
                SneeShortAttack();
                break;
            case 4: // 텔레라임
                Teleport();
                break;
            case 5: // 회복라임
                HPBallShortAttack();
                Healling();
                break;
        }
    }

    //근 원거리 공격 코루틴----------------------------
    IEnumerator ShortAttackIE()
    {
        if (!isAttack)
        {
            isAttack = true;
            PlayerControl.instance.animator.SetTrigger("Attack_Short");
            yield return new WaitForSeconds(1.0f);
            isAttack = false;
        }
    }
    IEnumerator LongAttackIE()
    {
        if (!isAttack)
        {
            isAttack = true;
            PlayerControl.instance.animator.SetTrigger("Attack_Long");
            yield return new WaitForSeconds(0.5f);
            isAttack = false;
        }
    }
    //-----------------------------------------------

    //슬라임 상태-------------------------------------
    void EatRange()
    {
        RaycastHit2D hit;

        if (PlayerControl.instance.isRight)
        {

            Debug.DrawRay(FireBullet.instance.bulletPos.transform.position, Vector2.right * eatdistance, Color.red);

            hit = Physics2D.Raycast(FireBullet.instance.bulletPos.transform.position, Vector2.right, eatdistance); 
            if (hit.collider != null)
            {
                //Debug.Log(hit.transform.gameObject.name);
                if (hit.transform.gameObject.tag == "Monster") // 몬스터일때
                {
                    //Debug.Log("먹기실행");
                    hit.transform.SendMessage("MonCurrentHP", SendMessageOptions.DontRequireReceiver);
                    hit.transform.gameObject.GetComponent<MonsterCtrl>().MonCurrentHP();
                    if (MonsterManager.instance.monCurrentHP <= eatOKHP)
                    {
                        if (Input.GetKeyDown(KeyCode.F))
                        {
                            Instantiate(PaticleManager.instance.player_Eatmon.transform.gameObject, PlayerEvent.instance.effpos, PlayerEvent.instance.transform.rotation);
                            StartCoroutine(EatIE());
                            //hit.transform.SendMessage("MonSendIcon", SendMessageOptions.DontRequireReceiver); // 해당몬스터Ctrl에 몬스터 bottle아이콘 보내기
                            hit.transform.gameObject.GetComponent<MonsterCtrl>().MonSendIcon();
                            StartCoroutine(EatDestroyIE(hit.transform.gameObject));
                        }
                    }

                }
                else if (hit.transform.gameObject.tag == "HP")
                {
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        Instantiate(PaticleManager.instance.player_Eathp.transform.gameObject, PlayerEvent.instance.effpos, PlayerEvent.instance.transform.rotation);
                        StartCoroutine(EatIE());
                        GameControl.instance.OpenItemIcon(0);
                        StartCoroutine(EatDestroyIE(hit.transform.gameObject));
                    }
                }
                else if (hit.transform.gameObject.tag == "Star")
                {
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        StartCoroutine(EatIE());
                        GameControl.instance.OpenItemIcon(1);
                        StartCoroutine(EatDestroyIE(hit.transform.gameObject));
                    }
                }
                else if (hit.transform.gameObject.tag == "Twine")
                {
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        StartCoroutine(EatIE());
                        GameControl.instance.OpenItemIcon(2);
                        StartCoroutine(EatDestroyIE(hit.transform.gameObject));
                    }  
                }
                else if (hit.transform.gameObject.tag == "Key") // 키일때
                {
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        StartCoroutine(EatIE());
                        GameControl.instance.OpenItemIcon(3);
                        StartCoroutine(EatDestroyIE(hit.transform.gameObject));
                    }
                }
                else if(hit.transform.gameObject.tag == "Plasma")  //플라즈마 일때   //2018-11-19
                {
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        StartCoroutine(EatIE());
                        GameControl.instance.OpenItemIcon(4);
                        StartCoroutine(EatDestroyIE(hit.transform.gameObject));

                        //다윗 코드 추가 
                        //보틀 먹으면 문열고 엔딩씬 넘어가는 기능
                        iceDoor.GetComponent<Animator>().Play("Open");
                        isEnd = true;
                        
                    }
                }
            }
        }
        else if (!PlayerControl.instance.isRight)
        {

            Debug.DrawRay(FireBullet.instance.bulletPos.transform.position, Vector2.left * eatdistance, Color.red);
            hit = Physics2D.Raycast(FireBullet.instance.bulletPos.transform.position, Vector2.left, eatdistance);
            if (hit.collider != null)
            {
                //Debug.Log(hit.transform.gameObject.name);
                if (hit.transform.gameObject.tag == "Monster")
                {
                    //Debug.Log("먹기실행");
                    hit.transform.gameObject.GetComponent<MonsterCtrl>().MonCurrentHP(); // MonsterCtrl에 몬스터 체력 체크
                    if (MonsterManager.instance.monCurrentHP <= eatOKHP)
                    {
                        if (Input.GetKeyDown(KeyCode.F))
                        {
                            StartCoroutine(EatIE());
                            hit.transform.gameObject.GetComponent<MonsterCtrl>().MonSendIcon(); // 해당몬스터Ctrl에 몬스터 bottle아이콘 보내기
                            StartCoroutine(EatDestroyIE(hit.transform.gameObject));
                        }
                    }

                }
                else if (hit.transform.gameObject.tag == "HP")
                {
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        StartCoroutine(EatIE());
                        GameControl.instance.OpenItemIcon(0);
                        StartCoroutine(EatDestroyIE(hit.transform.gameObject));
                    }
                }
                else if (hit.transform.gameObject.tag == "Star")
                {
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        StartCoroutine(EatIE());
                        GameControl.instance.OpenItemIcon(1);
                        StartCoroutine(EatDestroyIE(hit.transform.gameObject));
                    }
                }
                else if (hit.transform.gameObject.tag == "Twine")
                {
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        StartCoroutine(EatIE());
                        GameControl.instance.OpenItemIcon(2);
                        StartCoroutine(EatDestroyIE(hit.transform.gameObject));
                    }
                }
                else if (hit.transform.gameObject.tag == "Key") // 키일때
                {
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        StartCoroutine(EatIE());
                        GameControl.instance.OpenItemIcon(3);
                        StartCoroutine(EatDestroyIE(hit.transform.gameObject));

                    }
                }
            }
        }

    }
    IEnumerator EatIE()
    {
        if (!isEat)
        {
            isEat = true;
            PlayerControl.instance.animator.SetTrigger("Eat");
            SoundManager.instance.PlaySfx(transform.position, SoundManager.instance.get_item, 0, SoundManager.instance.bgmVolum);
            yield return new WaitForSeconds(2.0f);
            isEat = false;
        }
    }
    IEnumerator EatDestroyIE(GameObject hitObject)
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(hitObject);
    }
    //-----------------------------------------------

    //라바라임 상태-----------------------------------
    void LavaShortAttack()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(Lava_ShortAttackIE());
        }
    }
    IEnumerator Lava_ShortAttackIE()
    {
        if (!isAttack)
        {
            isAttack = true;
            PlayerControl.instance.animator.SetTrigger("Attack_Short");
            SoundManager.instance.PlaySfx(transform.position, SoundManager.instance.lava_attack, 0, SoundManager.instance.bgmVolum);
            yield return new WaitForSeconds(1.0f);
            isAttack = false;
        }
    }
    void LavaLongAttack()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            //PlayerControl.instance.animator.SetTrigger("Attack_Long");
            StartCoroutine(Lava_LongAttackIE());
        }
    }
    IEnumerator Lava_LongAttackIE()
    {
        if (!isAttack)
        {
            isAttack = true;
            PlayerControl.instance.animator.SetTrigger("Attack_Long");
            SoundManager.instance.PlaySfx(transform.position, SoundManager.instance.lava_bullet, 0, SoundManager.instance.bgmVolum);
            yield return new WaitForSeconds(0.5f);
            isAttack = false;
        }
    }
    //-----------------------------------------------

    //닌자라임 상태-----------------------------------

    void NinjaShortAttack()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            //PlayerControl.instance.animator.SetTrigger("Attack_Short");
            StartCoroutine(NinjaShortAttackIE());
            isNinjaAttack = true;
        }
    }
    IEnumerator NinjaShortAttackIE()
    {
        if (!isAttack)
        {
            isAttack = true;
            PlayerControl.instance.animator.SetTrigger("Attack_Short");
            SoundManager.instance.PlaySfx(transform.position, SoundManager.instance.ninja_shortAttack, 0, SoundManager.instance.bgmVolum);
            yield return new WaitForSeconds(1.0f);
            isAttack = false;
            isNinjaAttack = false;
        }
    }
    void NinjaLongAttack()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            //PlayerControl.instance.animator.SetTrigger("Attack_Long");
            StartCoroutine(Ninja_LongAttackIE());
        }
    }
    IEnumerator Ninja_LongAttackIE()
    {
        if (!isAttack)
        {
            isAttack = true;
            PlayerControl.instance.animator.SetTrigger("Attack_Long");
            SoundManager.instance.PlaySfx(transform.position, SoundManager.instance.ninja_shurikan, 0, SoundManager.instance.bgmVolum);
            yield return new WaitForSeconds(0.5f);
            isAttack = false;
        }
    }
    //-----------------------------------------------

    //스니키라임 상태---------------------------------
    void SneeShortAttack()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            //PlayerControl.instance.animator.SetTrigger("Attack_Short");
            StartCoroutine(Snee_ShortAttackIE());
        }
    }
    IEnumerator Snee_ShortAttackIE()
    {
        if (!isAttack)
        {
            isAttack = true;
            PlayerControl.instance.animator.SetTrigger("Attack_Short");
            SoundManager.instance.PlaySfx(transform.position, SoundManager.instance.sneeky_Attack, 0, SoundManager.instance.bgmVolum);
            yield return new WaitForSeconds(1.0f);
            isAttack = false;
        }
    }
    //-----------------------------------------------

    //텔레라임 상태-----------------------------------
    void Teleport()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(TeleportIE());
        }
    }
    void TeleLongAttack()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(Tele_LongAttackIE());
        }
    }
    IEnumerator TeleportIE()
    {
        if (!isTeleport)
        {
            isTeleport = true;
            if (PlayerControl.instance.isRight != false)
            {
                transform.position = new Vector2(transform.position.x + teleportsize, transform.position.y);
            }
            else if (PlayerControl.instance.isRight != true)
            {
                transform.position = new Vector2(transform.position.x - teleportsize, transform.position.y);
            }
            SoundManager.instance.PlaySfx(transform.position, SoundManager.instance.tele_tel, 0, SoundManager.instance.bgmVolum);
            yield return new WaitForSeconds(1f);
            isTeleport = false;
        }
    }

    IEnumerator Tele_LongAttackIE()
    {
        if (!isAttack)
        {
            isAttack = true;
            PlayerControl.instance.animator.SetTrigger("Attack_Long");
            SoundManager.instance.PlaySfx(transform.position, SoundManager.instance.tele_bullet, 0, SoundManager.instance.bgmVolum);
            yield return new WaitForSeconds(0.5f);
            isAttack = false;
        }
    }

    //-----------------------------------------------

    //회복라임 상태-----------------------------------
    void HPBallShortAttack()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(HPBall_ShortAttackIE());
        }
    }
    IEnumerator HPBall_ShortAttackIE()
    {
        if (!isAttack)
        {
            isAttack = true;
            PlayerControl.instance.animator.SetTrigger("Attack_Short");
            SoundManager.instance.PlaySfx(transform.position, SoundManager.instance.hp_attack, 0, SoundManager.instance.bgmVolum);
            yield return new WaitForSeconds(1.0f);
            isAttack = false;
        }
    }
    void Healling()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(HealIE());
        }
    }
    IEnumerator HealIE()
    {
        if (!isHeal)
        {
            isHeal = true;
            if (GameManager.instance.playerHP >= 70)
            {
                GameManager.instance.playerHP = 100;
            }
            else if (GameManager.instance.playerHP < 70)
            {
                GameManager.instance.playerHP += 30;
            }
            yield return new WaitForSeconds(10.0f);
            isHeal = false;
        }
    }

    //-----------------------------------------------
    private void OnTriggerEnter2D(Collider2D other)
    {

        if (isNinjaAttack)
        {
            if (other.gameObject.tag == "Monster")
            {
                target = other.gameObject;
                tpos = new Vector2(this.transform.position.x, other.gameObject.transform.position.y);
                //other.gameObject.transform.position = Vector2.Lerp(other.gameObject.transform.position, this.transform.position, Time.deltaTime * 10.0f);
                
                //isNinjaAttack = false;
            }
        }

        if(other.gameObject.name == "IceDoor")
        {
            //엔딩씬 로딩 코드 추가
            if(isEnd)
            {
                gameObject.GetComponent<SpriteRenderer>().sortingLayerID = 0;
                gameObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
                SceneManager.LoadScene("ResultScene");
            }
           
        }
    }

    void Grab(GameObject tg)
    {
        target.transform.position = Vector2.MoveTowards(target.transform.position, tpos, Time.deltaTime * 5.0f);
    }

    IEnumerator GrabDelay()
    {
        yield return new WaitForSeconds(1.0f);
        isNinjaAttack = false;
    }

    //-----------------------------------------------
    void PlayerToBrothersChange()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            switch (changeNum)
            {
                case 0:
                    {
                        GameObject.Find("Player").GetComponent<PlayerControl>().enabled = false;
                        GameObject.Find("Player_slime_Brother01").GetComponent<PlayerControl>().enabled = true;


                        FollowCam.instance.player = GameObject.Find("Player_slime_Brother01");
                        changeNum++;
                        //GameObject.Find("Player_slime_Brother01").SetActive(true);
                        //GameObject.Find("Player").SetActive(false);
                    }
                    break;
                case 1:
                    {
                        GameObject.Find("Player_slime_Brother01").GetComponent<PlayerControl>().enabled = false;
                        GameObject.Find("Player_slime_Brother02").GetComponent<PlayerControl>().enabled = true;

                        FollowCam.instance.player = GameObject.Find("Player_slime_Brother02");


                        changeNum++;
                        //GameObject.Find("Player_slime_Brother02").SetActive(true);
                        //GameObject.Find("Player_slime_Brother01").SetActive(false);
                    }
                    break;
                case 2:
                    {
                        GameObject.Find("Player_slime_Brother02").GetComponent<PlayerControl>().enabled = false;
                        GameObject.Find("Player").GetComponent<PlayerControl>().enabled = true;
                        FollowCam.instance.player = GameObject.Find("Player");
                        changeNum = 0;
                        //GameObject.Find("Player_slime_Brother2").SetActive(true);
                        //GameObject.Find("Player").SetActive(false);
                    }
                    break;

            }

        }
    }

    void AttackState()
    {

    }

}
