using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_NinjaCtrl : MonsterCtrl
{

  

    bool canMove = true;          //이동가능한지 (코루틴 제어변수)
    public float turnTerms = 5.0f;    //방향을 바꾸는 주기
    public float speed = 5.0f;        //이동속도
    Vector2 tempSpeed;             //이동속도 임시저장변수
  
    bool canAttack = true;       //공격 가능한지 (코루틴 제어변수)

    public GameObject atkPoint;  //플레이어를 당겨오는 공격 포인트

    Vector2 playerPos;            //플레이어 위치 저장 변수

    public static Mon_NinjaCtrl instance = null;
    public CircleCollider2D cc2d;

    private void Awake()
    {
        instance = this;
        tr = transform;
        rbody = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        renderer = GetComponent<SpriteRenderer>();
        cc2d = GetComponent<CircleCollider2D>();
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


                    if (!IsGround())
                    {
                        rbody.velocity = gravity;
                    }
                    else
                    {
                        if (Sense(tr) == 0)
                        {

                            if (canMove) StartCoroutine(NinjaMove());
                        }
                        else if (Sense(tr) == (int)traceRange)
                        {
                            LookAtPlayer(3,3,false);

                            rbody.velocity = new Vector2(tr.localScale.x, 0) * traceSpeed;
                        }
                        else if (Sense(tr) == (int)attackRange)
                        {
                            StopCoroutine(NinjaMove());
                            monsterState = MONSTERSTATE.ATTACK;
                        }
                    }
                   

                    break;
                }
            case MONSTERSTATE.ATTACK:
                {
                    LookAtPlayer(3,3,false);
                    animator.SetTrigger("Attack");

                    if(IsGround()) rbody.velocity = new Vector2(0, 0);
                    else
                    {
                        rbody.velocity = gravity;
                        break;
                    }


                    if (canAttack)
                    {
                       
                        StartCoroutine(Attack());
                    }

                    if (Sense(tr) != (int)attackRange)
                    {
                        StopCoroutine(Attack());

                        monsterState = MONSTERSTATE.MOVE;
                    }
                    break;
                }
            case MONSTERSTATE.DAMAGED:
                {
                    animator.SetTrigger("Damage");
                    SoundManager.instance.PlaySfx(tr.position, SoundManager.instance.ninja_Damage, 0, SoundManager.instance.bgmVolum);

                    Invoke("InvokeToMove", damageDelay);
                    break;
                }
            case MONSTERSTATE.DEAD:
                {   
                    animator.SetTrigger("Die");
                    SoundManager.instance.PlaySfx(tr.position, SoundManager.instance.ninja_Die, 0, SoundManager.instance.bgmVolum);

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


    

    protected override void OnTriggerEnter2D(Collider2D coll)
    {
        if(cc2d.IsTouching(coll) && coll.gameObject.tag == "Player")
        {
            coll.SendMessage("AttractToNinja", SendMessageOptions.DontRequireReceiver);
            SoundManager.instance.PlaySfx(tr.position, SoundManager.instance.ninja_shortAttack, 0, SoundManager.instance.bgmVolum);

        }

        base.OnTriggerEnter2D(coll);
       
    }

    
    private void OnTriggerExit2D(Collider2D coll)
    {
        if (cc2d.IsTouching(coll) && coll.gameObject.tag == "Player")
        {
            GameObject.Find("Player").transform.SetParent(null);
        }
    }

    //이동 함수
    IEnumerator NinjaMove()
    {
        canMove = false;
        rbody.velocity = (tr.localScale.x) * new Vector2(speed, 0);
        yield return new WaitForSeconds(turnTerms);   //5초동안 x방향으로 이동
        rbody.velocity = new Vector2(0, 0);                                  //5초되면 정지
        yield return new WaitForSeconds(1.5f);            //1.5초 기다렸다가 
        tr.localScale = new Vector2(-tr.localScale.x, tr.localScale.y);     //바라보는 방향 수정
        rbody.velocity = (tr.localScale.x) * new Vector2(speed, 0);          //방향바꿔서 이동

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
            rbody.gravityScale = 1;
        }
    }

   

    public void MonSendIcon() // 몬스터 bottle 실행시키기
    {
      
    }

}
