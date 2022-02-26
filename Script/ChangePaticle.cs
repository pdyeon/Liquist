using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePaticle : MonoBehaviour {

    public Animator animator;

    public static ChangePaticle instance;

    private void Awake()
    {
        instance = this;
        animator = GetComponent<Animator>();
    }

    public void ShowChangePaticle(int num)
    {
        switch (num)
        {
            case 0: // 슬라임
                break;
            case 1: // 라바레이
                animator.SetTrigger("Lava");
                break;
            case 2: // 닌자
                animator.SetTrigger("Ninja");
                break;
            case 3: // 스니키
                animator.SetTrigger("Snee");
                break;
            case 4: // 텔레볼
                animator.SetTrigger("Tele");
                break;
            case 5: // HP볼
                animator.SetTrigger("HP");
                break;
        }
    }
}
