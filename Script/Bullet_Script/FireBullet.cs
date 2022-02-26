using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBullet : MonoBehaviour {

    public Transform bulletPos;
    bool fire;

    public static FireBullet instance;

    private void Awake()
    {
        instance = this;
        fire = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(FireIE());
            //Fire();
        }
    }

    IEnumerator FireIE()
    {
        if (!fire)
        {
            fire = true;
            if (BulletManager.instance.currentBullet != null)
            {
                Instantiate(BulletManager.instance.currentBullet, bulletPos.position, bulletPos.rotation);
            }
            yield return new WaitForSeconds(0.5f);
            fire = false;
        }
        
    }
    /*
    void Fire()
    {
        if (BulletManager.instance.currentBullet != null)
        {
            Instantiate(BulletManager.instance.currentBullet, bulletPos.position, bulletPos.rotation);
        }
        
        if (BulletManager.instance.currentBullet != BulletManager.instance.Thorn)
        {
            BulletManager.instance.currentBullet = BulletManager.instance.Thorn;
            BulletManager.instance.currentDamage = BulletManager.instance.ThornDamage;
        }
        
    }
    */

}
