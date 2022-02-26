using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_SneekyCtrl : MonsterCtrl {



    bool canMove = true;          //이동가능한지 (코루틴 제어변수)
    public float turnTerms = 5.0f;    //방향을 바꾸는 주기
    public float speed = 5.0f;        //이동속도
    Vector2 tempSpeed;             //이동속도 임시저장변수 
    Vector2 playerPos;            //플레이어 위치 저장 변수

    private void Awake()
    {
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




                    if (Sense(tr) == 0)
                    {

                        if (canMove) StartCoroutine(SneekyMove());
                    }
                    else if (Sense(tr) == (int)traceRange)
                    {
                        playerPos = PlayerControl.instance.playerPos.position;
                        LookAtPlayer(3, 3, true);
                        rbody.velocity = new Vector2(-tr.localScale.x, 0) * traceSpeed;
                    }
                  


                    break;
                }
            case MONSTERSTATE.ATTACK:
                {



                    
                    break;
                }
            case MONSTERSTATE.DAMAGED:
                {
                    animator.SetTrigger("Damage");
                    SoundManager.instance.PlaySfx(tr.position, SoundManager.instance.sneeky_Attack, 0, SoundManager.instance.bgmVolum);

                    Invoke("InvokeToMove", damageDelay);
                    break;
                }
            case MONSTERSTATE.DEAD:
                {
                    animator.SetTrigger("Die");
                    SoundManager.instance.PlaySfx(tr.position, SoundManager.instance.sneeky_Die, 0, SoundManager.instance.bgmVolum);

                    break;
                }
        }



    }


  

    //땅에 있을시 y값고정해서 떨어지지않게
    void Land()
    {
        if (IsGround())
        {
            rbody.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            rbody.gravityScale = 0;
        }
        else
        {
            rbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            rbody.gravityScale = 10;
        }
    }





    //텔레볼 이동 함수
    IEnumerator SneekyMove()
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

    //플레이어랑 거리 감지 함수 재정의
   public override int Sense(Transform tr)
    {
        playerPos = PlayerControl.instance.playerPos.position;
        if (Vector2.Distance(playerPos, tr.position) <= traceRange)
        {
            return (int)traceRange;
        }
        return 0;
    }


   
}
