using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCtrl : MonoBehaviour {

    public static SwitchCtrl instance;

    Animator animator;

    public bool isSwitchOn = false;

    private void Awake()
    {
        instance = this;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isSwitchOn != false)
        {
            animator.SetBool("isOpen", true);
        }
        else if (isSwitchOn != true)
        {
            animator.SetBool("isOpen", false);
        }
    }

}
