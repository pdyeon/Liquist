using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaticleTest : MonoBehaviour {


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ShowPaticle();
        }
	}

    void ShowPaticle()
    {
        Instantiate(PaticleManager.instance.paticle, this.transform.position, this.transform.rotation);
    }
}
