using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterCtrl : MonoBehaviour {

    public int mon_Num = 0;

    public GameObject rayPoint_C;
    public GameObject rayPoint_L;
    public GameObject rayPoint_R;

    protected new SpriteRenderer renderer; //렌더링 관련 변수
    public float alphaTerm = 1.0f;     //알파값 바뀌는 주기
    protected bool isDamageMotion = false;   //데미지 입을시 깜빡이는 모션 제어변수
    public float damageMotionTime = 2.0f;   //몇초동안 깜빡일지
    protected float timeCount = 0;             //타이머

    public float attackRange = 15.0f;    //공격 감지 범위
    public float traceRange = 20.0f;    //추적 감지 범위
    public float traceSpeed = 8.0f;    //추적 속도
    public float atkSpeed = 2.0f;    //공격 속도

    public int atkDmg = 5;      //공격력
    public bool isDamaged = false;   //데미지를 입고 있는지

    public float damageDelay = 0.5f;    //데미지 회복시간

    public Vector2 gravity = new Vector2(0,-20);   //중력값 즉 떨어지는 속도값
    //Transform tr;
    //ItemMake itemMake;

    public float slowDelTime = 3.0f;   //서서히 사라지는 시간
   protected float lerpProcess = 0;

    protected Collider2D collider2d;

    protected Transform tr;
    protected Animator animator;
    protected Rigidbody2D rbody;

    public GameObject hpBarBackGround;     //체력바 배경설정 (체력바 SetActive 위함)
    public Image hpBar;       //체력바 이미지
    public float hp1 = 0.7f;    //체력이 몇퍼센트이하일시 체력바 이미지소스 교체
    public float hp2 = 0.3f;    // ``

  
    
    public enum MONSTERSTATE
    {
        IDLE = 0,
        MOVE,
        ATTACK,
        DAMAGED,
        DEAD
    }

    public MONSTERSTATE monsterState = MONSTERSTATE.IDLE;

    public LayerMask groundLayer;

    public float hp = 10;
    public float currentHp;   //현재 체력값 , 체력바 설정을 위해 정의
    public float damage = 0;
    //public int firePoint = 0;
    //public int icePoint = 0;

  

    private void Start()
    {
        currentHp = hp;
        hpBarBackGround.SetActive(false);
       
    }

    //Transform tr;
    //ItemMake itemMake;

    private void Awake()
    {
       
         //처음 데미지를 입기전까지 체력바를 비활성화 해둔다.
        //currentHp = hp;
        hpBar.fillAmount = 1.0f;
        tr = transform;
        rbody = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        collider2d = GetComponent<Collider2D>();
        
    }

    public void Update()
    {
      
        
        if(monsterState == MONSTERSTATE.DEAD)
        {
            SlowDelete();
           
        }
        
        /* if (isDamageMotion)
        {
            timeCount += Time.deltaTime;
            DamageMotion();
            if(timeCount >=damageMotionTime)
            {
                isDamageMotion = false;
                timeCount = 0;
                renderer.color= new Color(renderer.color.r,renderer.color.g,renderer.color.b,1);

            }
        }
        */

    }


    // 몬스터가 죽으면 서서히 삭제되는 함수
   protected virtual void SlowDelete()
    {
        Color temp = renderer.color;
        lerpProcess += Time.deltaTime / slowDelTime;
        temp = Color.Lerp(new Color(temp.r, temp.g, temp.b, 1), new Color(temp.r, temp.g, temp.b, 0), lerpProcess);
        renderer.color = temp;
        if (renderer.color.a == 0)
        {
            Destroy(this.gameObject);
        }
    }

    //몬스터랑 플레이어 거리 감지 함수( 재정의 가능)
    public virtual int Sense(Transform tr)
    {
        Vector2 playerPos = PlayerControl.instance.playerPos.position;
        if (Vector2.Distance(playerPos, tr.position) <= traceRange)
        {
            if (Vector2.Distance(playerPos, tr.position) <= attackRange)
            {
                return (int)attackRange;
            }
            return (int)traceRange;
        }
        return 0;
    }

    public virtual bool IsGround()
    {
        if (Physics2D.Raycast(rayPoint_C.transform.position, Vector2.down, 2.0f, groundLayer.value) ||
            Physics2D.Raycast(rayPoint_L.transform.position, Vector2.down, 2.0f, groundLayer.value) ||
            Physics2D.Raycast(rayPoint_R.transform.position, Vector2.down, 2.0f, groundLayer.value))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual void InvokeToMove()
    {
        animator.SetFloat("Move", 1);
        MonsterStateChange(1);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
       
       if(other.gameObject.tag == "Player")
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
        else if(other.gameObject.tag == "PlayerBullet")
        {
            Damaged(BulletManager.instance.BulletDamage[GameManager.instance.PlayerNum]);
        }

        

       
    }


    

    //몬스터가 데미지를 입는 함수
    //플레이어 스크립트에서 SendMessage로 호출
    //dmg는 플레이어가 가하는 데미지 값을 나타내는 인자
    //체력이 0 이하로 내려가면 DEAD상태값이되고 
    //아닐시 DAMAGED상태값이 된다. :D
    //프리즈마는 재정의 필요.
    protected virtual void Damaged()
    {
        if(isDamaged == false)    //처음 데미지를 입을시만 아래코드를 실행하여 체력바를 활성화 시킨다.
        {
            hpBarBackGround.SetActive(true);
        }
       currentHp =  currentHp - GameManager.instance.playerAtkDmg;
        isDamaged = true;
        isDamageMotion = true;   //데미지 입을시 깜빡이는 효과 관련 제어변수
        animator.SetFloat("Move", 0);      //메카님 적용해주기

        float hpPer;
        hpPer = currentHp / hp;
        if (hpPer<hp2)
        {
            hpBar.color = new Color(1, 0, 0, 1);
        }else if(hpPer<hp1)
        {
            hpBar.color = new Color(1, 1, 0, 1);
        }
        else { hpBar.color = new Color(0, 1, 0, 1); }
        hpBar.fillAmount = hpPer;


        // Debug.Log(this.name + " damaged");
        if (currentHp <= 0)
        {
            CancelInvoke();
            rbody.velocity = new Vector2(0, 0);
            MonsterStateChange(3);
            animator.Play("Die");
            rbody.constraints = RigidbodyConstraints2D.FreezeAll;
            collider2d.enabled = false;
            hpBarBackGround.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            return;
        }
        else
            MonsterStateChange(2);
    }

    //인자값을 받는 데미지함수
    protected virtual void Damaged(float dmg)
    {
        if (isDamaged == false)    //처음 데미지를 입을시만 아래코드를 실행하여 체력바를 활성화 시킨다.
        {
            hpBarBackGround.SetActive(true);
        }
        currentHp = currentHp - dmg;
        isDamaged = true;
        isDamageMotion = true;   //데미지 입을시 깜빡이는 효과 관련 제어변수
        animator.SetFloat("Move", 0);      //메카님 적용해주기

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
            rbody.velocity = new Vector2(0, 0);
            MonsterStateChange(3);
            animator.Play("Die");
            rbody.constraints = RigidbodyConstraints2D.FreezeAll;
            collider2d.enabled = false;
            hpBarBackGround.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            return;
        }
        else
            MonsterStateChange(2);
    }

    //몬스터 상태값 변경 함수
    //열거형 상태값이 다른 몬스터 상태변경 편이를 위해 정의
    protected virtual void MonsterStateChange(int stateNum)   //1:Move 2:Damage 3:Dead
    {
        if (stateNum == 1)
        {
            monsterState = MONSTERSTATE.MOVE;
        }
        else if (stateNum == 2)
        {
            monsterState = MONSTERSTATE.DAMAGED;
        }
        else if(stateNum == 3)
        {
            monsterState = MONSTERSTATE.DEAD;
        }
    }

    //몬스터가 데미지를 입을시 
    //깜빡이는 효과
    protected virtual void DamageMotion()
    {
        Color temp = renderer.color;
        Color lerpColor = Color.Lerp(new Color(temp.r, temp.g, temp.b, 1), new Color(temp.r, temp.g, temp.b, 0.2f), Mathf.PingPong(Time.time, alphaTerm));
        renderer.color = lerpColor;
    }

    //몬스터가 데미지입은 상태에서
    //Move상태로 돌아가게 하는 함수
    //플레이어 트리거엔드에서 센드메시지
    protected virtual void DamageToMove()
    {
        isDamaged = false;
        MonsterStateChange(1);
    }

    //플레이어를 바라보게 하는 기능
    //x1은 몬스터가 플레이어 우측 얼마거리만큼 오면 바라보게하는지
    //x2는 몬스터가 플레이어 좌측 얼마만큼 오면 바라보게 하는지
    //몬스터의 방향이 너무 자주 바뀔때 x1,x2값을 올려주면 된다.
    public virtual void LookAtPlayer(float x1,float x2,bool isReversed)
    {
        Vector2 playerPos = PlayerControl.instance.playerPos.position;
        if (playerPos.x + x1< tr.position.x)
        {
            tr.localScale = new Vector2(isReversed? 1 : -1, 1);

        }
        else if (playerPos.x - x2 >= tr.position.x)
        {
            tr.localScale = new Vector2(isReversed ? -1 : 1, 1);

        }

        hpBarBackGround.transform.localScale = tr.localScale;

    }

    public void MonSendIcon() // 몬스터 bottle 실행시키기
    {
        GameControl.instance.OpenMonIcon(mon_Num);
    }


    public void MonCurrentHP() // 먹기 시작하기 전에 체력체크
    {
        MonsterManager.instance.monCurrentHP = currentHp;
    }
    


}
