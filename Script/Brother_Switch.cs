using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brother_Switch : MonoBehaviour {

    Animator brother_Ani;
    Collider2D coll;
    Transform tr;

    bool isFull = false;
    bool isBrother01In = false;
    bool isBrother02In = false;

    float doorSpeed = 10.0f;

    public GameObject brotherDoor;

    float timer = 0.0f;

    private void Awake()
    {
        tr = transform;
        coll = GetComponent<Collider2D>();
        brother_Ani = GetComponent<Animator>();
    }

    void Start () {
		
	}
	
	void Update () {
		
        if(isBrother01In && isBrother02In)
        {
            timer += Time.smoothDeltaTime;
            if(timer <= 2.0f)
            {
                brotherDoor.transform.Translate(Vector2.up * doorSpeed * Time.smoothDeltaTime);
            }           
        }

	}

    private void OnTriggerStay2D(Collider2D other)
    {
        StartCoroutine(Brother_Bottle(other));
    }

    IEnumerator Brother_Bottle(Collider2D other)
    {

        if (other.name == "Player_slime_Brother01" && Input.GetKeyDown(KeyCode.F) && !isFull)
        {
            brother_Ani.SetBool("Brother01_Ani", true);
            other.GetComponent<Renderer>().enabled = false;
            other.GetComponent<PlayerControl>().enabled = false;
            yield return new WaitForSeconds(1.0f);
            isBrother01In = true;

            if(isBrother01In && !isBrother02In)
            {
                tr.parent.gameObject.transform.GetChild(1).GetComponent<Animator>().SetBool("Brother01_In", true);
            }
            else if (isBrother01In && isBrother02In)
            {
                tr.parent.gameObject.transform.GetChild(1).GetComponent<Animator>().SetBool("Brother01_In", false);
                tr.parent.gameObject.transform.GetChild(1).GetComponent<Animator>().SetBool("Full", true);
                coll.GetComponent<Collider2D>().enabled = false;
            }

            brother_Ani.SetBool("Brother01_Ani", false);
            other.GetComponent<Renderer>().enabled = true;
            other.GetComponent<PlayerControl>().enabled = true;

        }
        else if (other.name == "Player_slime_Brother02" && Input.GetKeyDown(KeyCode.F))
        {
            brother_Ani.SetBool("Brother02_Ani", true);
            other.GetComponent<Renderer>().enabled = false;
            other.GetComponent<PlayerControl>().enabled = false;
            yield return new WaitForSeconds(1.0f);
            isBrother02In = true;

            if (isBrother02In && !isBrother01In)
            {
                tr.parent.gameObject.transform.GetChild(1).GetComponent<Animator>().SetBool("Brother02_In", true);
            }
            else if (isBrother02In && isBrother01In)
            {
                tr.parent.gameObject.transform.GetChild(1).GetComponent<Animator>().SetBool("Brother02_In", false);
                tr.parent.gameObject.transform.GetChild(1).GetComponent<Animator>().SetBool("Full", true);
                coll.GetComponent<Collider2D>().enabled = false;
            }

            brother_Ani.SetBool("Brother02_Ani", false);
            other.GetComponent<Renderer>().enabled = true;
            other.GetComponent<PlayerControl>().enabled = true;
        }   
    }

}
