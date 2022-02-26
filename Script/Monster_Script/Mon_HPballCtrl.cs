using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_HPballCtrl : MonsterCtrl {

   
    
    bool canMove = true;          //이동가능한지 (코루틴 제어변수)
    public float turnTerms = 5.0f;    //방향을 바꾸는 주기
    public float speed = 5.0f;        //이동속도
    Vector2 tempSpeed;             //이동속도 임시저장변수

    bool canAttack = true;       //공격 가능한지 (코루틴 제어변수)
   


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
                    animator.ResetTrigger("Attack");
                    animator.SetFloat("Move", 1);



                    if (IsGround())
                    {
                        if (Sense(tr) == 0)
                        {
                            rbody.velocity = (tr.localScale.x) * new Vector2(speed, 0);
                            if (canMove) StartCoroutine(HpMove());
                        }
                        else if (Sense(tr) == (int)traceRange)
                        {
                            LookAtPlayer(0, 0, false);

                            rbody.velocity = new Vector2(tr.localScale.x, 0) * traceSpeed;
                        }
                        else if (Sense(tr) == (int)attackRange)
                        {
                            StopCoroutine(HpMove());
                            monsterState = MONSTERSTATE.ATTACK;
                        }
                    }else
                    {
                        rbody.velocity = gravity;
                    }
                    


                    break;
                }
            case MONSTERSTATE.ATTACK:
                {

                    LookAtPlayer(0, 0, false);
                    animator.SetTrigger("Attack");
                    SoundManager.instance.PlaySfx(tr.position, SoundManager.instance.hp_attack, 0, SoundManager.instance.bgmVolum);
                    rbody.velocity = new Vector2(0, 0);
                    
                    if (canAttack) StartCoroutine(Attack());

                    if (tr.position.y <= -0.9 && Sense(tr) != (int)attackRange)
                    {
                        StopCoroutine(Attack());

                        monsterState = MONSTERSTATE.MOVE;
                    }
                    break;
                }
            case MONSTERSTATE.DAMAGED:
                {
                    animator.SetTrigger("Damage");
                    SoundManager.instance.PlaySfx(tr.position, SoundManager.instance.hp_Damage, 0, SoundManager.instance.bgmVolum);

                    Invoke("InvokeToMove",damageDelay);
                    break;
                }
            case MONSTERSTATE.DEAD:
                {
                    animator.SetTrigger("Die");
                    SoundManager.instance.PlaySfx(tr.position, SoundManager.instance.hp_die, 0, SoundManager.instance.bgmVolum);

                    break;
                }
        }



    }


    IEnumerator Attack()
    {
        canAttack = false;
        
        yield return new WaitForSeconds(this.atkSpeed);
        canAttack = true;
    }

    //이동 멈추는 함수
    void Stop()
    {
        tempSpeed = rbody.velocity;
        rbody.velocity = new Vector2(0, 0);
    }


    //이동 함수
    IEnumerator HpMove()
    {
        canMove = false;
        yield return new WaitForSeconds(turnTerms);   //5초동안 x방향으로 이동
        Stop();                                           //5초되면 정지
        yield return new WaitForSeconds(1.5f);            //1.5초 기다렸다가 
        rbody.velocity = -tempSpeed;                      //방향바꿔서 이동
        tr.localScale = new Vector2(-tr.localScale.x, tr.localScale.y);     //바라보는 방향 수정
                                                                            /*
                                                                            Stop();
                                                                            Debug.Log("Stop");
                                                                            yield return new WaitForSeconds(1.5f);   //방향을 바꾸기전 멈춰있는 시간
                                                                            Turn();
                                                                            Debug.Log("Turn");
                                                                            yield return new WaitForSeconds(turnTerms);
                                                                        */
        canMove = true;

    }


   

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
            rbody.gravityScale = 1;
            rbody.AddForce(new Vector2(0, -50));
        }
    }


  

}
