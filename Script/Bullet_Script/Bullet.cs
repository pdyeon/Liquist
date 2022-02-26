using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    //Transform tr;
    public float speed = 1000f;
    Rigidbody2D rbody;

    //ItemMake itemMake;

    public float destroyTime = 2.0f;

    private void Awake()
    {
        //itemMake = GetComponent<ItemMake>();
    }

    // Use this for initialization
    void Start () {
        //tr = transform;
        rbody = GetComponent<Rigidbody2D>();
        fireBullet();

        //rbody.AddForce(tr.forward * speed);

        Destroy(gameObject, destroyTime);
        /*
        if (BulletManager.instance.currentBullet == BulletManager.instance.Thorn)
        {
           
        }
        */
    }

    void fireBullet()
    {
        if (PlayerControl.instance.isRight)
        {
            rbody.velocity = Vector2.right * speed;
        }
        else if (!PlayerControl.instance.isRight)
        {
            rbody.velocity = Vector2.left * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Transform DestroyPoint = tr;

        if (other.tag == "Monster" || other.tag == "GateKeeper")
        {
            Destroy(gameObject);
            /*
            if (BulletManager.instance.Thorn != true)
            {
                Instantiate(itemMake.item, DestroyPoint.position, DestroyPoint.rotation);
                
            }
            else if (BulletManager.instance.Thorn == true)
            {
                Destroy(gameObject);
            }
            */
        }
    }

}
