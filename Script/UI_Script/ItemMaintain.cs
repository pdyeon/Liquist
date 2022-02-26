using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMaintain : MonoBehaviour {

    public LayerMask groundLayer;

    Rigidbody2D rbody;

    private void Awake()
    {
        rbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        
        if (Physics2D.Raycast(transform.position, Vector2.down, 0.1f, groundLayer.value))
        {
            rbody.gravityScale = 0;
        }
    }

}
