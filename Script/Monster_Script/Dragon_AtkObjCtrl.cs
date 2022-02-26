using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon_AtkObjCtrl : MonoBehaviour {


    public float forceToPlayer = 100.0f;   //플레이어에게 가해지는 힘
    new SpriteRenderer renderer;
    public float slowDelTime = 3.0f;  //천천히 사라지는 시간
    public float delDelay = 2.0f;     //생성된후 몇초후에 사라지는지
    bool delayTimeOver = false;


    public static Dragon_AtkObjCtrl instance = null;

    float lerpProcess = 0;
    private void Awake()
    {
        instance = this;
        renderer = GetComponent<SpriteRenderer>();
    }

    void SlowDelete()
    {
        Color temp = renderer.color;
        lerpProcess += Time.deltaTime / slowDelTime;
        temp = Color.Lerp(new Color(temp.r, temp.g, temp.b, 1), new Color(temp.r, temp.g, temp.b, 0), lerpProcess);
        renderer.color = temp;
       //Debug.Log("SlowDeleting");
    }

    void BoolChange()
    {
        delayTimeOver = true;
        this.GetComponent<PolygonCollider2D>().enabled = false;
    }
    void Start () {
        Invoke("BoolChange", delDelay);
        
       
          //  Debug.Log(this.GetComponent<PolygonCollider2D>().points[0]
            //    + " " +this.GetComponent<PolygonCollider2D>().points[1]
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (delayTimeOver)
        {
            SlowDelete();
            if (renderer.color.a == 0)
            {
                Destroy(this.gameObject);
                Mon_Dragon_Ctrl.instance.attack1Count--;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Vector2 thisPos = transform.position;
            Vector2 playerPos = PlayerControl.instance.playerPos.position;

            //플레이어가 드래곤 공격오브젝트에 닿았을시 닿은 방향의 반방향으로 팅기게 설정
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(playerPos.x - thisPos.x, playerPos.y - thisPos.y).normalized * forceToPlayer);
           //  collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 1).normalized * forceToPlayer);

        }
    }


    //폴리곤 콜라이더 생성을 위한 좌표배열
    Vector2[] case1 = {new Vector2(2.716572f,2.732231f),new Vector2(-0.6188469f,7.87607f),
        new Vector2(-2.337864f,8.494915f),new Vector2(-2.318443f,6.068615f),
        new Vector2(0.365242f,2.422987f) };
    Vector2[] case2 = { new Vector2(-2.5f, 2.3f), new Vector2(2.7f, 2.7f),
        new Vector2(-0.6f, 7.9f), new Vector2(-2.3f, 8.3f), new Vector2(-2.4f, 6.8f),
        new Vector2(-3.5f, 9.0f), new Vector2(-5.9f, 10.0f), new Vector2(-5.7f, 6.7f) };
    Vector2[] case3 = { new Vector2(-2.5f, 2.3f), new Vector2(2.7f, 2.7f),
        new Vector2(-0.6f, 7.9f), new Vector2(-2.3f, 8.3f), new Vector2(-2.4f, 6.8f),
        new Vector2(-3.5f, 9.0f), new Vector2(-6.0f, 9.9f), new Vector2(-7.3f, 11.0f),
        new Vector2(-8.6f, 13.1f), new Vector2(-9.8f, 12.0f), new Vector2(-10.5f, 9.5f),
        new Vector2(-7.4f, 2.0f) };
    Vector2[] case4 = { new Vector2(-2.5f, 2.3f), new Vector2(2.7f, 2.7f),
        new Vector2(-0.6f, 7.9f), new Vector2(-2.3f, 8.3f), new Vector2(-2.4f, 6.8f),
        new Vector2(-3.5f, 9.0f), new Vector2(-6.0f, 9.9f), new Vector2(-7.3f, 11.0f),
        new Vector2(-7.8f, 12.3f), new Vector2(-8.6f, 13.1f), new Vector2(-10.0f, 11.4f),
        new Vector2(-12.6f, 14.8f), new Vector2(-15.2f, 16.4f), new Vector2(-17.8f, 15.8f),
        new Vector2(-15.0f, 9.4f), new Vector2(-14.5f, 6.1f), new Vector2(-16.7f, 6.6f),
        new Vector2(-16.6f, 5.1f), new Vector2(-14.3f, 2.0f) };

    //폴리곤 콜라이더를 애니메이션 단계별로 확장하는 기능
    public void PolyGonColliderChange(int i)
    {
        PolygonCollider2D pc = this.GetComponent<PolygonCollider2D>();
        switch(i)
        {
            case 0:
                {
                    pc.enabled = false;
                    break;
                }
            case 1:
                {
                    pc.SetPath(0, case1);
                    break;
                }
            case 2:
                {
                    pc.SetPath(0, case2);
                    break;
                }
            case 3:
                {
                    pc.SetPath(0, case3);
                    break;
                }
            case 4:
                {
                    pc.SetPath(0, case4);
                    break;
                }
        }
    }
}
