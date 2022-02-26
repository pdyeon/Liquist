using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_RabbitaiCtrl : MonsterCtrl
{

  

    bool canMove = true;          //이동가능한지 (코루틴 제어변수)
    public float turnTerms = 5.0f;    //방향을 바꾸는 주기
    public float speed = 5.0f;        //이동속도
    Vector2 tempSpeed;             //이동속도 임시저장변수 
    Vector2 playerPos;            //플레이어 위치 저장 변수

    public int aniMotionValue; //공격 애니메이션 동작할때 이변수를 수정함으로 레비타이 위치값조정

    bool canAttack = true;      //공격가능한지 (코루틴 제어변수)

    CircleCollider2D circleCollider2D;   //발공격 콜라이더 저장변수
    public float forceToPlayer = 50.0f;   //플레이어에게 공격할시 플레이어에게 가해지는 힘
    private void Awake()
    {
        circleCollider2D = GetComponent<CircleCollider2D>();
        tr = transform;
        animator = GetComponent<Animator>();
        rbody = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
    }


  

    private void FixedUpdate()
    {
        Land();
        switch (monsterState)
        {
            case MONSTERSTATE.IDLE:
                {
                    animator.SetFloat("Move", 0);

                    monsterState = MONSTERSTATE.MOVE;
                    break;
                }
            case MONSTERSTATE.MOVE:
                {

                    animator.SetFloat("Move", 1);



              if(IsGround())
                    {
                        if (Sense(tr) == 0)
                        {

                            if (canMove) StartCoroutine(RabbiMove());
                        }
                        else if (Sense(tr) == (int)traceRange)
                        {
                            LookAtPlayer(0, 0, true);
                            rbody.velocity = new Vector2(-tr.localScale.x, 0) * traceSpeed;
                        }
                        else if (Sense(tr) == (int)attackRange)
                        {
                            animator.SetFloat("Move", 0);
                            monsterState = MONSTERSTATE.ATTACK;
                        }
                    }
                    else
                    {
                        rbody.velocity = gravity;
                    }
                    break;
                }
            case MONSTERSTATE.ATTACK:
                {
                    //RabbiAniMotionFunc();   //애니메이션 조정으로 변경, 추후 x방향 값만 코드로 수정할것.!!
                 
                    animator.SetTrigger("Attack");

                    rbody.velocity = new Vector2(0, 0);

                    LookAtPlayer(0, 0, true);

                 
                    if (Sense(tr) != (int)attackRange)
                    { 
                        monsterState = MONSTERSTATE.MOVE;
                    }
                    break;



                   
                }
            case MONSTERSTATE.DAMAGED:
                {
                    animator.SetTrigger("Damage");
                    SoundManager.instance.PlaySfx(tr.position, SoundManager.instance.rabbit_Damage, 0, SoundManager.instance.bgmVolum);

                    Invoke("InvokeToMove", damageDelay);
                    break;
                }
            case MONSTERSTATE.DEAD:
                {
                    animator.SetTrigger("Die");
                    SoundManager.instance.PlaySfx(tr.position, SoundManager.instance.rabbit_Die, 0, SoundManager.instance.bgmVolum);

                    break;
                }
        }



    }


    public void RabbiAttackSound(int i)
    {
        if(i==0)
        {
            SoundManager.instance.PlaySfx(tr.position, SoundManager.instance.rabbit_Kick1, 0, SoundManager.instance.bgmVolum);

        }
        
        else {
            SoundManager.instance.PlaySfx(tr.position, SoundManager.instance.rabbit_Kick2, 0, SoundManager.instance.bgmVolum);
        }


    }

    //땅에 있을시 y값고정해서 떨어지지않게
    void Land()
    {
        if (IsGround())
        {
            rbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            rbody.gravityScale = 0;
        }
        else
        {
            rbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            rbody.gravityScale = 20;
        }
    }

    public float rayCastLength = 5.0f;   //IsGround 체크할때 레이케스트를 쏘는 거리d

    public override bool IsGround()
    {
        if (Physics2D.Raycast(rayPoint_C.transform.position, Vector2.down, rayCastLength, groundLayer.value) ||
            Physics2D.Raycast(rayPoint_L.transform.position, Vector2.down, rayCastLength, groundLayer.value) ||
            Physics2D.Raycast(rayPoint_R.transform.position, Vector2.down, rayCastLength, groundLayer.value))
        {
            return true;
        }
        else
        {
            return false;
        }
    }



    //텔레볼 이동 함수
    IEnumerator RabbiMove()
    {
        canMove = false;
        rbody.velocity = (-tr.localScale.x) * new Vector2(speed, 0);
        yield return new WaitForSeconds(turnTerms);   //5초동안 x방향으로 이동
        rbody.velocity = new Vector2(0, 0);                                        //5초되면 정지
        yield return new WaitForSeconds(1.5f);            //1.5초 기다렸다가 

        tr.localScale = new Vector2(-tr.localScale.x, tr.localScale.y);     //바라보는 방향 수정
        rbody.velocity = (-tr.localScale.x) * new Vector2(speed, 0);    //방향바꿔서 이동

        canMove = true;

    }

   
    IEnumerator Attack()
    {
        canAttack = false;
        yield return new WaitForSeconds(this.atkSpeed);
        canAttack = true;
    }

       private new void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("OnT active!");
        if(circleCollider2D.IsTouching(collision) && collision.gameObject.tag == "Player")
        {
            PlayerControl.instance.ForceAdded(new Vector2(-tr.localScale.x * forceToPlayer, 0));

        }

        base.OnTriggerEnter2D(collision);

    }

    public float jumpForce = 20.0f;
    public float dashForce = 15.0f;

    public void RabbitaiAtkMotion(int i)
    {
        if (i == 1)
        {
            rbody.AddForce(new Vector2(0, 1) * jumpForce);
        }else if(i == 2)
        {
            rbody.AddForce(new Vector2(-tr.localScale.x, 0) * dashForce);
        }
    }

    public float dashLength = 5.0f;    //레비타이 대쉬 거리


    //레비타이 공격 애니메이션 작동시 마지막 컷에 이 함수로 레비타이가 바라보는 방향
    //즉 플레이어가 위치한 방향으로 포지션값을 이동해준다.
    public void RabbitaiAtkMove()   
    {
        tr.position = new Vector2(tr.position.x + (-tr.localScale.x) * dashLength,tr.position.y);
    }

   

}
	


