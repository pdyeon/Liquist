using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_LavaCtrl : MonsterCtrl
{

   

  
    float currentDistance;
    public GameObject lavaFire;
    public Transform firePos;

    public float fireBallSpd = 10.0f;    //투사체 속도

    bool canMove = true;          //이동가능한지 (코루틴 제어변수)
    public float turnTerms = 5.0f;    //방향을 바꾸는 주기
    public float speed = 5.0f;        //이동속도

    public float runSpeed = 1.0f;    //도망가는 속도

    bool canAttack = true;       //공격 가능한지 (코루틴 제어변수)

    Vector2 playerPos;            //플레이어 위치 저장 변수


    public static Mon_LavaCtrl instance = null;


    private void Awake()
    {
        instance = this;
        tr = GetComponent<Transform>();
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
                    monsterState = MONSTERSTATE.MOVE;
                    break;
                }
            case MONSTERSTATE.MOVE:
                {
                    //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    //추후에 플레이어가 추적감지범위내에 진입시 추적하고 공격감지범위에 진입했을시만 공격시작  =  done
                    //그리고 체력이 일정 미만으로 떨어질시 도망코드도 추가할것
                    //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                    animator.ResetTrigger("Attack");
                    animator.SetFloat("Move", 1);

                    if(IsGround())
                    {
                        if (Sense(tr) == (int)traceRange)                     //플레이어가 추적 감지 범위에 진입시 추적 시작
                        {

                            LookAtPlayer(2, 2, false);

                            rbody.velocity = new Vector2(tr.localScale.x, 0) * traceSpeed;

                        }
                        else if (Sense(tr) == (int)attackRange)                     //플레이어가 공격 감지 범위에 진입시 공격상태 돌입
                        {
                            rbody.velocity = new Vector2(0, 0);                     // 라바가 멈춰서 공격 한다.
                            monsterState = MONSTERSTATE.ATTACK;
                        }
                        else
                        {
                            if (canMove) StartCoroutine(LavaMove());

                        }
                    }else
                    {
                        rbody.velocity = gravity;
                    }
                   

                   

                   
                    break;
                }
            case MONSTERSTATE.ATTACK:
                {

                    LookAtPlayer(2, 2, false);
                   

                    animator.SetFloat("Move", 0);                           //이동 애니메이션 작동 정지
                    animator.SetTrigger("Attack");                          //공격 애니메이션 작동 시작

                    if (Sense(tr) != (int)attackRange)
                    {
                        monsterState = MONSTERSTATE.MOVE;
                    }else if(Sense(tr) ==(int)attackRange)
                    {
                        Vector2 playerPos = PlayerControl.instance.playerPos.position;
                        if(tr.position.x >playerPos.x && PlayerControl.instance.PlayerMovingDir()==1 || tr.position.x < playerPos.x && PlayerControl.instance.PlayerMovingDir() == -1)
                        {
                            float dist = Vector2.Distance(playerPos, tr.position);
                            rbody.velocity = (attackRange - dist) *runSpeed* new Vector2(-tr.localScale.x,0);
                        }
                    }



                    if (canAttack) StartCoroutine(LavaFire());

                  

                    break;
                }
            case MONSTERSTATE.DAMAGED:
                {
                    animator.SetTrigger("Damage");
                    SoundManager.instance.PlaySfx(tr.position, SoundManager.instance.lava_attack, 0, SoundManager.instance.bgmVolum);

                    Invoke("InvokeToMove", damageDelay);
                    break;
                }
            case MONSTERSTATE.DEAD:
                {
                    animator.SetTrigger("Die");
                    SoundManager.instance.PlaySfx(tr.position, SoundManager.instance.lava_die, 0, SoundManager.instance.bgmVolum);

                    break;
                }

        }
    }


    public override bool IsGround()
    {
        if (Physics2D.Raycast(rayPoint_C.transform.position, Vector2.down, 5.0f, groundLayer.value) ||
           Physics2D.Raycast(rayPoint_L.transform.position, Vector2.down, 5.0f, groundLayer.value) ||
           Physics2D.Raycast(rayPoint_R.transform.position, Vector2.down, 5.0f, groundLayer.value))
        {
            return true;
        }
        else
        {
            return false;
        }
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
        }
    }
  

    //이동 함수
    IEnumerator LavaMove()
    {
        canMove = false;
        rbody.velocity = new Vector2(tr.localScale.x, 0) * speed;

        yield return new WaitForSeconds(turnTerms);   //5초동안 x방향으로 이동
        rbody.velocity = new Vector2(0, 0);                //5초되면 정지
        yield return new WaitForSeconds(1.5f);            //1.5초 기다렸다가 
        tr.localScale = new Vector2(-tr.localScale.x, tr.localScale.y);     //바라보는 방향 수정
        rbody.velocity = new Vector2(tr.localScale.x, 0) * speed;
        canMove = true;

    }










    IEnumerator LavaFire()
    {
        canAttack = false;
        Fire();
        SoundManager.instance.PlaySfx(tr.position, SoundManager.instance.lava_bullet, 0, SoundManager.instance.bgmVolum);
        yield return new WaitForSeconds(this.atkSpeed);
        canAttack = true;

    }
    void Fire()
    {
        GameObject tempBullet = Instantiate(lavaFire);
        tempBullet.transform.position = firePos.position;

        float x1 = PlayerControl.instance.playerPos.position.x;
        float x2 = transform.position.x;

        float y1 = PlayerControl.instance.playerPos.position.y;
        float y2 = transform.position.y;

        Vector2 tmp;
        if (y1 <= 0) tmp = new Vector2(x1 > x2 ? 1 : -1, 0) * fireBallSpd;
        else 
           tmp = new Vector2(x1-x2, y1-y2).normalized * fireBallSpd;

        tempBullet.GetComponent<Rigidbody2D>().velocity = tmp;

    }


    


}
