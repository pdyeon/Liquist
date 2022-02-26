using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGroundCtrl : MonoBehaviour {

    public float speed = 0.1f;
    public float switchingTime = 2.0f;
    public bool isUp = false;

    bool turn = false;
    float timer = 0.0f;

    BoxCollider2D bcoll;
    Transform tr;
    public static MoveGroundCtrl instance;

    private void Awake()
    {
        instance = this;
        tr = transform;
        bcoll = GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        timer += Time.deltaTime;

        if (isUp != true)
        {
            if (turn != true)
            {
                tr.position = new Vector2(tr.position.x + speed, tr.position.y);
                if (timer >= switchingTime) 
                {
                    timer = 0.0f;
                    turn = true;
                }
            }
            else if (turn != false)
            {
                tr.position = new Vector2(tr.position.x - speed, tr.position.y);
                if (timer >= switchingTime)
                {
                    timer = 0.0f;
                    turn = false;
                }
            }
        }
        else if (isUp != false)
        {
            if (turn != true)
            {
                tr.position = new Vector2(tr.position.x, tr.position.y + speed);
                if (timer >= switchingTime)
                {
                    timer = 0.0f;
                    turn = true;
                }
            }
            else if (turn != false)
            {
                tr.position = new Vector2(tr.position.x, tr.position.y - speed);
                if (timer >= switchingTime)
                {
                    timer = 0.0f;
                    turn = false;
                }
            }
        }
        
    }

    private void Update()
    {
        if (PlayerControl.instance.IsGround())
        {
            bcoll.isTrigger = false;
        }
        else if (!PlayerControl.instance.IsGround())
        {
            bcoll.isTrigger = true;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        float halfspeed = speed;
        if (other.tag == "Player" && !Input.GetKeyDown(KeyCode.LeftArrow) || !Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (isUp != true)
            {
                if (turn != true)
                {
                    other.transform.position = new Vector2(other.transform.position.x + halfspeed, other.transform.position.y);
                }
                else if (turn != false)
                {
                    other.transform.position = new Vector2(other.transform.position.x - halfspeed, other.transform.position.y);
                }
            }
            /*
            else if (isUp != false)
            {
                if (turn != true)
                {
                    //PlayerControl.instance.limitJumpPos = tr.position.y + PlayerControl.instance.jumpLimit;
                    other.transform.position = new Vector2(other.transform.position.x, other.transform.position.y + halfspeed);
                }
                else if (turn != false)
                {
                    //PlayerControl.instance.limitJumpPos = tr.position.y + PlayerControl.instance.jumpLimit;
                    other.transform.position = new Vector2(other.transform.position.x, other.transform.position.y - halfspeed);
                }
            }
            */
            
        }
    }

}
