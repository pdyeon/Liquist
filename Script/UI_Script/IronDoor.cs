using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronDoor : MonoBehaviour {

    Transform tr;

    public Vector2 startPos;
    public Vector2 endPos;
    public Vector2 presentPos;

    public bool isSwitchCheck = false;


    public static IronDoor instance;

    private void Awake()
    {
        tr = transform;

        startPos = tr.transform.position;  //시작점은 irondoor의 위치
        endPos = new Vector2(startPos.x, startPos.y + 8f);  //목표점은 irondoor의 y+8의 위치값

        instance = this;

    }

    private void FixedUpdate()
    {
        presentPos = this.transform.position;
        
    }	
    
}
