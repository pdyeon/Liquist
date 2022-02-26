using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potal2Ctrl : MonoBehaviour {

    public GameObject sponPoint;

    Animator animator;

    public static Potal2Ctrl instance;

    private void Awake()
    {
        instance = this;
        animator = GetComponent<Animator>();
    }

    public void InOutPotal(int num)
    {
        if (num > 0)
        {
            animator.SetTrigger("In");
        }
        else if (num < 0)
        {
            animator.SetTrigger("Out");
        }
    }

}
