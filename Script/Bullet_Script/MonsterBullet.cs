using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBullet : MonoBehaviour
{




    public float bulletExistTime = 2.0f;
  
    public float bulletSpeed = 2.0f;



    public static MonsterBullet instance = null;

    private void Awake()
    {
        instance = this;

    }

    // Use this for initialization
    void Start()
    {
        Destroy(this.gameObject, bulletExistTime);
    }

    private void OnTriggerEnter2D(Collider2D coll)
    { 
        if (coll.gameObject.name == "Player")
        {
            if(this.gameObject.name == "Lava Fire(Clone)")
            {
                GameManager.instance.Damaged(Mon_LavaCtrl.instance.atkDmg);    //플레이어가 데미지를 입고
                Destroy(this.gameObject);                                                                     //투사체는 사라진다.

            }
            else if(this.gameObject.name == "PrizmaBullet(Clone)")
            {
                GameManager.instance.Damaged(Mon_PrizmaMonCtrl.instance.atkDmg);    //플레이어가 데미지를 입고
                Destroy(this.gameObject);                                                                     //투사체는 사라진다.

            }


        }
    }


}
