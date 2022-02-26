using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_PrizmaMonCtrl : MonsterCtrl{


   public new enum MONSTERSTATE
    {
        IDLE = 0,
        MOVE,
        SWORDATTACK,
        EQATTACK,
        KNEEATTACK,
        LONGATTACK,
        DAMAGED,
        DEAD

    }

    public GameObject prizmaBullet;   //프리즈마가 발사하는 투사체 프리펩
    public Transform firePos;         //프리즈마 투사체 발사되는곳
    public float bulletForce = 10.0f;
    public float predictShotDist = 5.0f;  //프리즈마의 총알 예측샷 거리
   


    public MONSTERSTATE monState = MONSTERSTATE.IDLE;

    bool canMove = true;          //이동가능한지 (코루틴 제어변수)
    public float turnTerms = 5.0f;    //방향을 바꾸는 주기
    public float speed = 5.0f;        //이동속도
    Vector2 tempSpeed;             //이동속도 임시저장변수 
    Vector2 playerPos;            //플레이어 위치 저장 변수

    public float dashForce = 50.0f;     //대쉬하기위한 힘 수치

    public float closeAttackRange = 5.0f;    //근접공격 범위
    public float gunAttackRange = 15.0f;   //원거리공격 범위

    bool canAttack = true;      //공격가능한지 (코루틴 제어변수)

    public float dashTime = 2.0f;    //프라즈마가 플레이어를 향해 대쉬해 올때 걸리는 시간


    public bool isAniQuit = false;

    public static Mon_PrizmaMonCtrl instance = null;

    private void Awake()
    {
        instance = this;
        tr = transform;
        animator = GetComponent<Animator>();
        rbody = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        collider2d = GetComponent<Collider2D>();
    }

    private void Start()
    {
        hpBarBackGround.SetActive(false);
        currentHp = hp;
    }

    private new void Update()
    {

        if(monState == MONSTERSTATE.DEAD)
        {
            SlowDelete();
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            Damaged();
        }
    }

    private void FixedUpdate()
    {
        Land();
        switch (monState)
        {
            case MONSTERSTATE.IDLE:
                {
                    animator.SetFloat("Move", 0);

                    monState = MONSTERSTATE.MOVE;
                    break;
                }
            case MONSTERSTATE.MOVE:
                {
                    //move상태에 올때 지금 취하고있는
                    //공격모션들을 전부 취소한다.
                    animator.SetFloat("Move", 1);


                    animator.ResetTrigger("LongAttack");
                    animator.ResetTrigger("SwordAttack");
                    animator.ResetTrigger("EQAttack");
                    animator.ResetTrigger("Knee");


                  

                    if (IsGround())
                    {
                        LookAtPlayer(3, 3, false);
                        rbody.velocity = new Vector2(tr.localScale.x, 0) * speed;

                        if(Sense(tr) >= (int)gunAttackRange)
                        {
                            animator.SetFloat("Move", 0);
                            monState = MONSTERSTATE.SWORDATTACK;
                        }
                        else if(Sense(tr) <(int)gunAttackRange && Sense(tr) >= (int)gunAttackRange*0.5)
                        {
                            animator.SetFloat("Move", 0);
                            if (Random.Range(1, 100) % 8 != 0)     //확률적으로 원거리공격 범위일때 대시어택을 한다
                            {
                                monState = MONSTERSTATE.SWORDATTACK;
                            }else monState = MONSTERSTATE.LONGATTACK;

                        }
                        else if(Sense(tr) <= (int)closeAttackRange)
                        {
                            animator.SetFloat("Move", 0);
                            monState = Random.Range(1,100) <50  ? MONSTERSTATE.EQATTACK : MONSTERSTATE.KNEEATTACK;
                        }
                      
                    }
                    else
                    {
                        rbody.velocity = gravity;
                    }




                    break;
                }
            case MONSTERSTATE.LONGATTACK:
                {
                    
                    if (Random.Range(1, 100) % 3 == 0)     //확률적으로 원거리공격시전할때 대시어택을 한다
                    {
                       animator.SetTrigger("SwordAttack");
                    }else animator.SetTrigger("LongAttack");




                    if (isAniQuit)
                    {
                        animator.SetFloat("Move", 1);
                        
                        monState = MONSTERSTATE.MOVE;
        
                    }else if(!isAniQuit && animator.GetFloat("Move") == 1)
                    {
                        monState = MONSTERSTATE.MOVE;
                    }
                    break;
                }
            case MONSTERSTATE.SWORDATTACK:
                {
                   if(canAttack)
                    {
                        StartCoroutine(PrizmaDashAttack());
                    }
                    if (isAniQuit)
                    {
                        animator.SetFloat("Move", 1);
                        monState = MONSTERSTATE.MOVE;
                    
                    }

                    break;
                }
            case MONSTERSTATE.EQATTACK:
                {
                    rbody.velocity = new Vector2(0, 0);
                    animator.SetTrigger("EQAttack");
                    SoundManager.instance.PlaySfx(tr.position, SoundManager.instance.pri1_charge, 0, SoundManager.instance.bgmVolum);

                    if (isAniQuit)
                    {
                        animator.SetFloat("Move", 1);
                        monState = MONSTERSTATE.MOVE;
                   
                    }
                    break;
                }
            case MONSTERSTATE.KNEEATTACK:
                {
                    rbody.velocity = new Vector2(0, 0);
                    animator.SetTrigger("Knee");
                    SoundManager.instance.PlaySfx(tr.position, SoundManager.instance.pri1_groundHit, 0, SoundManager.instance.bgmVolum);

                    if (isAniQuit)
                    {
                        animator.SetFloat("Move", 1);
                        monState = MONSTERSTATE.MOVE;
                     
                    }
                    break;
                        
                }
     
            case MONSTERSTATE.DAMAGED:
                {
                    animator.SetTrigger("Damage");
                    SoundManager.instance.PlaySfx(tr.position, SoundManager.instance.pri1_Damage, 0, SoundManager.instance.bgmVolum);

                    Invoke("InvokeToMove", damageDelay);
                    break;
                }
            case MONSTERSTATE.DEAD:
                {

                    animator.SetTrigger("Die");
                    SoundManager.instance.PlaySfx(tr.position, SoundManager.instance.pri1_Die, 0, SoundManager.instance.bgmVolum);

                    break;
                }
        }



    }

    

    protected override void SlowDelete()
    {
        Color temp = renderer.color;
        lerpProcess += Time.deltaTime / slowDelTime;
        temp = Color.Lerp(new Color(temp.r, temp.g, temp.b, 1), new Color(temp.r, temp.g, temp.b, 0), lerpProcess);
        renderer.color = temp;
        if (renderer.color.a == 0)
        {
            Destroy(this.gameObject);
            //문삭제코드추가할것.
            Destroy(GameObject.Find("door2"));
            Mon_Dragon_Ctrl.instance.Able(true);
        }
    }

    //프리즈마 상태 변경 함수  1:Move 2:Damage 3:Dead
    protected override void MonsterStateChange(int stateNum)
    {
        if(stateNum == 1)
        {
            monState = MONSTERSTATE.MOVE;
        }else if(stateNum == 2)
        {
            monState = MONSTERSTATE.DAMAGED;
        }else if(stateNum ==3)
        {
            monState = MONSTERSTATE.DEAD;
        }
    }


    IEnumerator PrizmaGunAttack()
    {
        canAttack = false;
        rbody.velocity = new Vector2(0, 0);
        animator.SetTrigger("LongAttack");
        Fire();
        yield return new WaitForSeconds(atkSpeed);
        canAttack = true;
    }


   public void Fire()
    {
        Vector2 playerPos = PlayerControl.instance.playerPos.position;
        GameObject temp = Instantiate(prizmaBullet);
        temp.transform.position = firePos.position;
        float x = playerPos.x - firePos.position.x;
        float y = playerPos.y - firePos.position.y;

        //벡터의 tan값을 구해서 arctan으로 각도를 구하고 Rad2Deg상수를 곱해서 도수로 변환
        //그래서 총알이 항상 플레이어를 향하는 각도로 기울어져있게.
        temp.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(y, x) * Mathf.Rad2Deg);


        if (PlayerControl.instance.PlayerMovingDir()*x>0 )
        {
            x += predictShotDist * PlayerControl.instance.PlayerMovingDir();
        }
  
        temp.GetComponent<Rigidbody2D>().AddForce(new Vector2(x,y)*bulletForce);               //플레이어 방향으로 총알발사.
        SoundManager.instance.PlaySfx(tr.position, SoundManager.instance.pri1_bullet, 0, SoundManager.instance.bgmVolum);



    }

    IEnumerator PrizmaDashAttack()
    {
        canAttack = false;

        rbody.velocity = new Vector2(0, 0);
      //  rbody.AddForce(new Vector2(tr.localScale.x,0) * dashForce,ForceMode2D.Force);      //일단 단방향힘을 주는 방법, 움직이는 모션이 안보이는 단점 그냥 점멸같은 느낌. 
        animator.SetTrigger("SwordAttack");
        // tr.rotation = Quaternion.Euler(0, 0, tr.localScale.x / Mathf.Abs(tr.localScale.x) * -45);

        yield return new WaitForSeconds(dashTime);

       
        
      
      //  tr.rotation = Quaternion.Euler(0, 0, 0);
        canAttack = true;
    }


   public void DashForce()
    {
        rbody.AddForce(new Vector2(tr.localScale.x, 0) * dashForce, ForceMode2D.Force);
        SoundManager.instance.PlaySfx(tr.position, SoundManager.instance.pri1_splash, 0, SoundManager.instance.bgmVolum);

    }



    //땅에 있을시 y값고정해서 떨어지지않게
    void Land()
    {
        if (IsGround())
        {
            //rbody.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            rbody.gravityScale = 0;
        }
        else
        {
           // rbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            rbody.gravityScale = 1;
        }
    }
    

    //플레이어 바라보는 함수 수정 
    //프리즈마의 localScale값이 (2,2)으로 인함.
    public override void LookAtPlayer(float x1, float x2, bool isReversed)
    {
        Vector2 playerPos = PlayerControl.instance.playerPos.position;
        if (playerPos.x + x1 < tr.position.x)
        {
            tr.localScale = new Vector2(isReversed ? 2 : -2, 2);

        }
        else if (playerPos.x - x2 >= tr.position.x)
        {
            tr.localScale = new Vector2(isReversed ? -2 : 2, 2);

        }
    }


    //프리즈마 4가지 공격 패턴에 따른 범위 감지 함수 기능 변화
    //거리 측정용 기능으로 재정의
    public override int Sense(Transform tr)
    {
        return (int)Mathf.Abs(tr.position.x - PlayerControl.instance.playerPos.position.x);   
    }



    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            
               if (PlayerControl.instance.playerPos.position.y > tr.position.y && PlayerControl.instance.isLanding && GameManager.instance.PlayerNum == 0)
                {
                    if (currentHp > 0)
                    {
                        other.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.up * 30.0f;

                    }
                    Damaged();
                }

                 
            if (PlayerChange.instance.isAttack && GameManager.instance.PlayerNum != 0)
            {
                Damaged();
            }

        }
        else if (other.gameObject.tag == "PlayerBullet")
        {
            Damaged(BulletManager.instance.BulletDamage[GameManager.instance.PlayerNum]);

        }
    }


}
