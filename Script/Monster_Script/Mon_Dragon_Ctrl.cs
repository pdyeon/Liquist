using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mon_Dragon_Ctrl : MonsterCtrl {

	public new enum MONSTERSTATE
    {
        IDLE =0,
        ATTACK1,
        BRESS,
        DIE
    }

    public GameObject bottle;  //죽으면 드랍하는 보틀 오브젝트

    public GameObject atkObj;  //얼음기둥 오브젝트 프리팹
    public bool atkObjGen = false;
    public float objYValue = 194.3f;    //오브젝트 y값
    public int attack1Count = 0;        //오브젝트 수량 (제한할것);
    public int attack1Max = 4;          //오브젝트 최대 수량 제한

    public float atkDelay = 2.0f;
    bool canAttack = true;
    bool isBressing = false;


    public float idleWaitTime = 3.0f;   //idle 상태에서 머무르는 시간

    public MONSTERSTATE dragonState = MONSTERSTATE.IDLE;

    public static Mon_Dragon_Ctrl instance = null;

    PolygonCollider2D pc2d;
    CapsuleCollider2D cc2d;

    //  서서히 사라지는 진행도

    private void Awake()
    {
        instance = this;
        tr = transform;
        animator = this.GetComponent<Animator>();
        renderer = this.GetComponent<SpriteRenderer>();
        pc2d = GetComponent<PolygonCollider2D>();
        cc2d = GetComponent<CapsuleCollider2D>();
      //  this.gameObject.SetActive(false);
      
    }

    protected override void SlowDelete()
    {
        Color temp = renderer.color;
        lerpProcess += Time.deltaTime / slowDelTime;
        temp = Color.Lerp(new Color(temp.r, temp.g, temp.b, 1), new Color(temp.r, temp.g, temp.b, 0), lerpProcess);
        renderer.color = temp;
        if (renderer.color.a == 0)
        {
            GameObject p = Instantiate(bottle);
            p.transform.position = tr.position;
            Destroy(this.gameObject);
            

            // 프리즈마 보틀 드랍 코드추가
        }
    }

  
    private new void Update()
    {
        if(dragonState == MONSTERSTATE.DIE)
        {
            SlowDelete();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Damaged();
        }
    }


   
    private void FixedUpdate()
    {
        switch (dragonState)
        {

            case MONSTERSTATE.IDLE:
                {
                    animator.SetTrigger("Idle");
                    animator.ResetTrigger("Bress");
                    animator.ResetTrigger("Attack1");
                
                    Invoke("InvokeIdle", idleWaitTime); //Idle에서 지정한 시간만큼 대기한다
                    break;
                }
            case MONSTERSTATE.ATTACK1:
                {
                    animator.ResetTrigger("Idle");
                    animator.ResetTrigger("Bress");
                    animator.SetTrigger("Attack1");
                   
                    if(Random.Range(101,300)>120)
                    {
                        dragonState = MONSTERSTATE.BRESS;
                    }
                    else
                    {
                        dragonState = MONSTERSTATE.IDLE;
                    }
                    
                    break;

                }
            case MONSTERSTATE.BRESS:
                {
                    animator.ResetTrigger("Idle");
                    animator.SetTrigger("Bress");
                    animator.ResetTrigger("Attack1");

                    SoundManager.instance.PlaySfx(tr.position, SoundManager.instance.pri2_attack, 0, SoundManager.instance.bgmVolum);

                    if (Random.Range(0,199)>150)
                    {
                        dragonState = MONSTERSTATE.IDLE;
                    }else
                    {
                        dragonState = MONSTERSTATE.ATTACK1;
                    }
                 

                    break;

                }
            case MONSTERSTATE.DIE:
                {
                    animator.ResetTrigger("Idle");
                    animator.ResetTrigger("Bress");
                    animator.ResetTrigger("Attack1");
                    animator.SetTrigger("Die");
                    SoundManager.instance.PlaySfx(tr.position, SoundManager.instance.pri2_Die1, 0, SoundManager.instance.bgmVolum);

                    break;
                }
        }

    }

    public void Able(bool b)
    {
        this.gameObject.SetActive(b);
    }

    void InvokeIdle()
    {
        if(Vector2.Distance(tr.position, PlayerControl.instance.playerPos.position) < attackRange)
        {
            if (Random.Range(0, 100) >= 90 && attack1Count <= attack1Max)
            {
                dragonState = MONSTERSTATE.ATTACK1;
            }
            else if (Random.Range(5, 155) > 130)
            {
                dragonState = MONSTERSTATE.BRESS;
            }
        }
      
    }


    //몬스터 상태값 변경 함수
    //열거형 상태값이 다른 몬스터 상태변경 편이를 위해 정의
    protected override void MonsterStateChange(int stateNum)   //2:IDLE 3:DIE 
    {
        if(stateNum == 2)
        {
            dragonState = MONSTERSTATE.IDLE;
        }else if(stateNum == 3)
        {
            dragonState = MONSTERSTATE.DIE;
        }
    }


    //데미지 입는 함수 변경
    protected override void Damaged()
    {
        if (isDamaged == false)    //처음 데미지를 입을시만 아래코드를 실행하여 체력바를 활성화 시킨다.
        {
            hpBarBackGround.SetActive(true);
        }
        currentHp = currentHp - GameManager.instance.playerAtkDmg;
        isDamaged = true;
      
                    SoundManager.instance.PlaySfx(tr.position, SoundManager.instance.pri2_Damage, 0, SoundManager.instance.bgmVolum);


        float hpPer;
        hpPer = currentHp / hp;
        if (hpPer < hp2)
        {
            hpBar.color = new Color(1, 0, 0, 1);
        }
        else if (hpPer < hp1)
        {
            hpBar.color = new Color(1, 1, 0, 1);
        }
        else { hpBar.color = new Color(0, 1, 0, 1); }
        hpBar.fillAmount = hpPer;


        // Debug.Log(this.name + " damaged");
        if (currentHp <= 0)
        {
            CancelInvoke();
            MonsterStateChange(3);    //DIE상태값으로 변경
            animator.Play("Die");
            hpBarBackGround.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            return;
        }
        else
            MonsterStateChange(2);     //IDLE상태값으로 변경
    }
    //얼음기둥들이 플레이어 위치 부근에 생성되는 함수
    public void Attack1()
    {
        if (attack1Count <= attack1Max)
        {
            GameObject temp = Instantiate(atkObj);
            temp.transform.position = new Vector2(PlayerControl.instance.playerPos.position.x, objYValue);
            attack1Count++;
            SoundManager.instance.PlaySfx(tr.position, SoundManager.instance.pri2_attack, 0, SoundManager.instance.bgmVolum);

        }
    }


    //폴리곤 콜라이더 설정을 위한 좌표 배열들
    Vector2[] bress_case1 = { new Vector2(8.1f, 6.3f), new Vector2(3.3f, -13.7f), new Vector2(-2.4f, -16.7f), new Vector2(-19.1f, -15.4f), new Vector2(-20.5f, -12.8f) };
    Vector2[] bress_case2 = { new Vector2(12.9f, 6.1f), new Vector2(3.9f, -15.2f), new Vector2(-4.7f, -17.4f), new Vector2(-24.7f, -14.2f), new Vector2(-25.6f, -8.3f), new Vector2(-18.3f, -10.3f) };
    Vector2[] bress_case3 = { new Vector2(18.4f, 6.8f), new Vector2(-25.5f, -12.1f), new Vector2(-26.5f, -8.0f), new Vector2(-31.1f, -13.1f), new Vector2(-28.1f, -17.2f), new Vector2(1.1f, -16.2f), new Vector2(2.0f, -10.2f) };
    Vector2[] bress_case4 = { new Vector2(29.7f, 3.0f), new Vector2(24.4f, 17.9f), new Vector2(8.1f, 11.3f), new Vector2(8.5f, 6.4f), new Vector2(16.8f, -4.1f) };




    public void DragonBressPolygonCollider(int i)
    {
        if(i == 1)
        {
            pc2d.SetPath(0, bress_case1); 
        }
        else if (i == 2)
        {
            pc2d.SetPath(0, bress_case2);
        }
        else if (i == 3)
        {
            pc2d.SetPath(0, bress_case3);
        }
       else if(i == 4)
        {
            pc2d.SetPath(0, bress_case4);
        }
    }

    //, new Vector2 

    //브레스 공격할시 머리부분의 콜라이더 설정을 위한 좌표배열
    Vector2[] bressHead_case1 = { new Vector2(-12.3f, 4.6f), new Vector2(-9.3f, -3.4f), new Vector2(0.2f, -10.9f), new Vector2(8.9f, 1.7f), new Vector2(2.5f, 13.2f) };
    Vector2[] bressHead_case2 = { new Vector2(-13.4f, 3.7f), new Vector2(-10.8f, -4.4f), new Vector2(-2.4f, -11.7f), new Vector2(8.1f, 0.9f), new Vector2(1.1f, 11.8f) };
    Vector2[] bressHead_case3 = { new Vector2(0.3f, -10.8f), new Vector2(9.1f, 1.1f), new Vector2(-1.4f, 11.0f), new Vector2(-12.8f, -2.0f), new Vector2(-6.8f, -8.9f) };
    Vector2[] bressHead_case4 = { new Vector2(7.8f, 18.4f), new Vector2 (16.7f, 4.8f), new Vector2 (11.5f, -5.4f) , new Vector2(5.3f, -4.2f) , new Vector2(10.0f, 5.2f) , new Vector2(8.3f, 8.6f)
            , new Vector2(-0.3f, -0.9f), new Vector2 (-5.2f, 7.6f)};
    Vector2[] bressHead_case5 = {new Vector2(3.6f, 1.1f), new Vector2(14.4f, 9.1f), new Vector2(14.9f, 4.6f), new Vector2(9.2f, -3.1f), new Vector2(15.8f, -5.7f), new Vector2(23.2f, 0.7f),
        new Vector2(14.1f, 18.4f), new Vector2 (0.7f, 8.4f) };
    Vector2[] bressHead_case6 = {new Vector2(6.3f, 3.4f), new Vector2(4.6f, 10.8f), new Vector2(20.5f, 18.0f), new Vector2(26.1f, -0.4f), new Vector2(18.6f, -5.5f)
            , new Vector2 (12.2f, -2.2f), new Vector2 (18.8f, 4.5f), new Vector2 (18.2f, 8.2f) };
    Vector2[] bressHead_case7 = { new Vector2(29.4f, 4.5f), new Vector2(16.5f, -3.3f), new Vector2(10.4f, 2.8f), new Vector2(8.6f, 11.6f), new Vector2(23.3f, 18.8f) };

    public void BressHeadPolygonCollider(int i)
    {
        if (i == 1)
        {
            pc2d.SetPath(0, bressHead_case1);
        }
        else if (i == 2)
        {
            pc2d.SetPath(0, bressHead_case2);
        }
        else if (i == 3)
        {
            pc2d.SetPath(0, bressHead_case3);
        }
        else if (i == 4)
        {
            pc2d.SetPath(0, bressHead_case4);
        }
        else if (i == 5)
        {
            pc2d.SetPath(0, bressHead_case5);
        }
        else if (i == 6)
        {
            pc2d.SetPath(0, bressHead_case6);
        }
        else if (i == 7)
        {
            pc2d.SetPath(0, bressHead_case7);
        }

    }

    //어택모션 취할때 콜라이더 좌표배열
    Vector2[] atk_case1 = { new Vector2(-11.0f, -2.5f), new Vector2(-13.3f, 4.6f), new Vector2(-10.3f, 12.2f), new Vector2(7.5f, 10.1f), new Vector2(4.1f, -7.6f), new Vector2(-7.6f, -9.4f) };
    Vector2[] atk_case2 = { new Vector2(-13.1f, 0.1f), new Vector2(-12.0f, -4.6f), new Vector2(-4.5f, -10.8f), new Vector2(8.7f, -2.4f), new Vector2(0.3f, 10.3f) };
    Vector2[] atk_case3 = { new Vector2(-6.7f, -13.9f), new Vector2(0.4f, -16.0f), new Vector2(8.5f, -3.0f), new Vector2(-2.7f, 6.8f), new Vector2(-12.3f, -7.7f) };

    public void AtkPolygonCollider(int i)
    {
        if(i == 1)
        {
            pc2d.SetPath(0, atk_case1);
        }else if(i ==2)
        {
            pc2d.SetPath(0, atk_case2);
        }else if(i == 3)
        {
            pc2d.SetPath(0, atk_case3);
        }
    }
    protected override void OnTriggerEnter2D(Collider2D other)
    {


        if (other.gameObject.tag == "Player")
        {
            if (pc2d.IsTouching(other))
            {
                if (dragonState == MONSTERSTATE.BRESS)
                {
                    //bress damage or skill effect code add
                    Debug.Log("Damage in Bress");
                }
                else if (PlayerControl.instance.playerPos.position.y > tr.position.y && PlayerControl.instance.isLanding && GameManager.instance.PlayerNum == 0)
                {
                    if (currentHp > 0)
                    {
                        other.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.up * 30.0f;

                    }
                    Damaged();
                }

            }

            if(cc2d.IsTouching(other) && PlayerControl.instance.playerPos.position.y > tr.position.y && PlayerControl.instance.isLanding && GameManager.instance.PlayerNum == 0)
            {
                Damaged();
            }
            if (PlayerChange.instance.isAttack && GameManager.instance.PlayerNum != 0)
            {
                Damaged();
            }
        
        }else if(other.gameObject.tag == "PlayerBullet")
        {
            Damaged(BulletManager.instance.BulletDamage[GameManager.instance.PlayerNum]);

        }
       
   


    }




}
