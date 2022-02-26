using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_TeleCtrl : MonsterCtrl {



    bool canMove = true;          //이동가능한지 (코루틴 제어변수)
   public float teleTurnTerms = 5.0f;    //텔레볼이 방향을 바꾸는 주기
    public float teleSpeed = 5.0f;        //텔레볼 이동속도
    Vector2 tempSpeed;             //텔레볼 이동속도 임시저장변수

    bool canTeleport = true;       //텔레포트 가능한지 (코루틴 제어변수)
  
    public float teleHeight = 8.0f;           //텔레포트 높이,
    public float fallPower = 10.0f;           //떨어지는 힘
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
                    animator.SetBool("isIdle", true);
                    animator.SetFloat("Move", 0);
                    
                    monsterState = MONSTERSTATE.MOVE;
                    break;
                }
            case MONSTERSTATE.MOVE:
                {
                

                    animator.ResetTrigger("Attack");
                    animator.SetFloat("Move",1);

                  if(!IsGround())
                    {
                        rbody.velocity = gravity;
                    }
                    else
                    {

                        if (Sense(tr) == 0)
                        {

                            if (canMove) StartCoroutine(TeleMove());
                        }
                        else if (Sense(tr) == (int)traceRange)
                        {
                            LookAtPlayer(0, 0, false);
                            rbody.velocity = new Vector2(tr.localScale.x, 0) * traceSpeed;
                        }
                        else if (Sense(tr) == (int)attackRange)
                        {
                            StopCoroutine(TeleMove());
                           
                            monsterState = MONSTERSTATE.ATTACK;
                        }
                    }


                    
                   
                    break;
                }
            case MONSTERSTATE.ATTACK:
                {
                  

                  


                   
                    animator.SetFloat("Move", 0);

                    rbody.velocity = new Vector2(0, 0);
                    //flash to player upside 
                    if (canTeleport) StartCoroutine(TelePort());

                  
                    if(IsGround() && Sense(tr)!=(int)attackRange)
                    {

                        StopCoroutine(TelePort());
                       

                        monsterState = MONSTERSTATE.MOVE;
                    }
                    break;
                }
            case MONSTERSTATE.DAMAGED:
                {
                    animator.SetTrigger("Damage");
                    SoundManager.instance.PlaySfx(tr.position, SoundManager.instance.tele_Damage, 0, SoundManager.instance.bgmVolum);

                    Invoke("InvokeToMove", damageDelay);
                    break;
                }
            case MONSTERSTATE.DEAD:
                {
                    animator.SetTrigger("Die");
                    SoundManager.instance.PlaySfx(tr.position, SoundManager.instance.tele_Die, 0, SoundManager.instance.bgmVolum);
 
                    break;
                }
        }



    }


    //텔레볼 텔레포트 함수
    IEnumerator TelePort()
    {
        canTeleport = false;
        playerPos = PlayerControl.instance.playerPos.position;
        tr.position = new Vector2(playerPos.x, playerPos.y + teleHeight);
        animator.SetTrigger("Attack");
        SoundManager.instance.PlaySfx(tr.position, SoundManager.instance.tele_tel, 0, SoundManager.instance.bgmVolum);

        // rbody.AddForce(new Vector2(0, -1) * fallPower);
        yield return new WaitForSeconds(this.atkSpeed);
        
        canTeleport = true;

      
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
            rbody.gravityScale = 20;
        }
    }

    
   


    //텔레볼 이동 함수
    IEnumerator TeleMove()
    {
        canMove = false;
        rbody.velocity = (tr.localScale.x) * new Vector2(teleSpeed, 0);
        yield return new WaitForSeconds(teleTurnTerms);   //5초동안 x방향으로 이동
        rbody.velocity = new Vector2(0, 0);                                        //5초되면 정지
        yield return new WaitForSeconds(1.5f);            //1.5초 기다렸다가 

        tr.localScale = new Vector2(-tr.localScale.x, tr.localScale.y);     //바라보는 방향 수정
        rbody.velocity = (tr.localScale.x) * new Vector2(teleSpeed, 0);    //방향바꿔서 이동

        /*
        Stop();
        Debug.Log("Stop");
        yield return new WaitForSeconds(1.5f);   //텔레볼이 방향을 바꾸기전 멈춰있는 시간
        Turn();
        Debug.Log("Turn");
        yield return new WaitForSeconds(teleTurnTerms);
    */    
    canMove = true;

    }

   

    /*  private void OnTriggerEnter2D(Collider2D coll)
      {
          if(coll.gameObject.tag == "Player")
          {
              //Damage(damage);    //플레이어가 데미지를 입는 함수
          }else if(coll.gameObject.layer == LayerMask.NameToLayer("Ground")&&tr.position.y>0)     //텔레볼이 땅을 제외한 그라운드에 닿았을시
          {
              tr.position = new Vector2(tr.position.x, tr.position.y - 5.0f);
          }
      }
      */

   
}
