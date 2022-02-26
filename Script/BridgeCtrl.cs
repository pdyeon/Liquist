using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeCtrl : MonoBehaviour {

    Transform tr;
    Animator bridgeAni;

    static public BridgeCtrl instance;

    public bool isBridge = false;

    private void Awake()
    {
        tr = transform;
        bridgeAni = this.GetComponent<Animator>();

        instance = this;
    }

	void Start () {
		
	}
	
	void Update () {
		
	}

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.tag == "Player")
        {
            StartCoroutine(BridgeOpen(other.collider));
        }
    }

    IEnumerator BridgeOpen(Collider2D other)
    {
        bridgeAni.SetBool("IsOnDmg", true);
        yield return new WaitForSeconds(0.5f);
        this.GetComponent<BoxCollider2D>().isTrigger = true;
    }
}
