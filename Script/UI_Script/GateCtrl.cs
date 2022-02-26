using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateCtrl : MonoBehaviour {

    //public GameObject openGate;
    //public GameObject closeGate;
    Animator animator;
    public int Count = 1;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Count <= MonsterManager.instance.gateCount || SwitchCtrl.instance.isSwitchOn != false)
        {
            animator.SetBool("isOpen", true);
            //closeGate.SetActive(false);
            //openGate.SetActive(true);
        }
        else if (Count > MonsterManager.instance.gateCount || SwitchCtrl.instance.isSwitchOn != true)
        {
            animator.SetBool("isOpen", false);
        }
    }

}
