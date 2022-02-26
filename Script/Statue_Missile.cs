using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statue_Missile : MonoBehaviour {
    //미사일 속도, 스폰위치
    //코루틴은 오버라이드로 못받는지?

  
    float speed;       //발사 속도
    float timer = 0;   //객체 제거 타이머

    Rigidbody2D mRigidbody2D;

    static public Statue_Missile instance;

    private void Awake()
    {
        speed = Random.Range(15, 30);     //속도는 15~30          

        mRigidbody2D = GetComponent<Rigidbody2D>();

        instance = this;
    }

	void Start () {
		
	}
	
	void Update () {
       
        Statue_Missile_Move();

        timer += Time.smoothDeltaTime;
        if(timer >= 3f)
        {
            Destroy(this.gameObject);
            timer = 0;
        }

    }

    void Statue_Missile_Move()
    {
        mRigidbody2D.velocity = Vector2.left * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(timer>= 3f)   // 3초 경과시
        {
            Destroy(this.gameObject);         
        }
    }

}
