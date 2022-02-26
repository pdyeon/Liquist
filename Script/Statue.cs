using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statue : MonoBehaviour {

    float timer = 0;  //기존 타이머
    float delayTimer;
    bool fire = false;

    public GameObject statue_Missile;
    public Transform spPoint;

    static public Statue_Missile instance;

    private void Awake()
    {
        
    }

    void Start () {



    }
	
	void Update () {
        timer += Time.smoothDeltaTime;
        delayTimer = Random.Range(2, 4);  //딜레이는 2~4초

        if (timer >= delayTimer)
        {
            Statue_Fire();
            timer = 0;
            delayTimer = 0;
        }
    }

    void Statue_Fire()
    {
        if (!fire)
        {
            Instantiate(statue_Missile, spPoint.position, spPoint.rotation);

            fire = true;
        }
        else
        {
            fire = false;
        }
      
    }
}
