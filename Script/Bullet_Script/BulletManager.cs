using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour {

    public GameObject[] Bullet;
    public float[] BulletDamage;
    //public int changeCharacter = 0;

    public GameObject currentBullet;
    public float currentDamage;

    public int bulletNum = 0;

    public static BulletManager instance;

    private void Awake()
    {
        instance = this;
        currentBullet = Bullet[0];
        currentDamage = BulletDamage[0];
    }

    private void Update()
    {
        ChangeBullet();
    }

    void ChangeBullet()
    {
        currentBullet = Bullet[bulletNum];
        currentDamage = BulletDamage[bulletNum];
    }
}
