using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {

    public GameObject rayPoint_C;
    public GameObject rayPoint_L;
    public GameObject rayPoint_R;

    public float runSpeed = 5f;
    public float jumpForce = 10f;
    public float jumpLimit = 5f;
    public float limitJumpPos;

    public Transform playerPos;
    public Material playermaterial;

    Transform tr;
    public Animator animator;
    Rigidbody2D rbody;

    public LayerMask groundLayer;
    public LayerMask MonLayer;

    public bool isRight = false;
    public bool isjumping = false;
    public bool ismoveing = false;

    public bool isLanding = false;
    int isMovingRight = 0;

    //public bool isAttractToNinja = false; //플레이어가 닌자쪽으로 끌려가는지


    public static PlayerControl instance;

    private void Awake()
    {
        instance = this;
        tr = transform;
        GetComponent<SpriteRenderer>().material = playermaterial;
        rbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        
        UpSit();
        Land();
       
        JumpDown();
       
            
        Debug.DrawRay(rayPoint_C.transform.position, Vector2.down * 1.0f, Color.red);
    }

    private void FixedUpdate()
    {
        Move();
        Jump();
    }

   

    public virtual bool IsGround()
    {
        if (Physics2D.Raycast(rayPoint_C.transform.position, Vector2.down, 1.0f, groundLayer.value) ||
            Physics2D.Raycast(rayPoint_L.transform.position, Vector2.down, 1.0f, groundLayer.value) ||
            Physics2D.Raycast(rayPoint_R.transform.position, Vector2.down, 1.0f, groundLayer.value))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void UpSit()
    {
        if (FollowCam.instance.iscamPosUp != false)
        {
            animator.SetBool("Up", true);
        }
        else if (FollowCam.instance.iscamPosSit != false)
        {
            animator.SetBool("Sit", true);
        }
        else
        {
            animator.SetBool("Up", false);
            animator.SetBool("Sit", false);
        }
    }

    void Jump()
    {
        if (IsGround())
        {
            if (Input.GetKeyDown(KeyCode.Space) && PlayerEvent.instance.isClimb != true)
            {
                //rbody.constraints = RigidbodyConstraints2D.FreezeRotation;
                animator.SetTrigger("Jump");
                SoundManager.instance.PlaySfx(transform.position, SoundManager.instance.player_jump, 0, SoundManager.instance.bgmVolum);
                isjumping = true;
                
                rbody.velocity = Vector2.up * jumpForce;
            }
            
        }
        
    }

    void Move()
    {
        if (FollowCam.instance.iscamPosUp != true && FollowCam.instance.iscamPosSit != true)
        {
            Vector2 moveVelocity = Vector2.zero;

            float move = runSpeed * Time.smoothDeltaTime;
            float key = Input.GetAxis("Horizontal");
            tr.Translate(Vector2.right * move * key);
            if (key == 0)
            {
                isMovingRight = 0;
                animator.SetFloat("Move", -1);
            }
            else if (key < 0)
            {
                isMovingRight = -1;
                moveVelocity = Vector2.left;
                isRight = false;
                tr.localScale = new Vector2(-1, 1);
                animator.SetFloat("Move", 1);
            }
            else if (key > 0)
            {
                isMovingRight = 1;
                moveVelocity = Vector2.right;
                isRight = true;
                tr.localScale = new Vector2(1, 1);
                animator.SetFloat("Move", 1);
            }
        }
    }

    public int PlayerMovingDir()
    {
        return isMovingRight;
    }

    void Land()
    {
        if (IsGround())
        {
            isLanding = false;
            if (isjumping == false)
            {
                if (!Input.GetKeyDown(KeyCode.Space) && PlayerEvent.instance.isClimb != true)
                {
                    rbody.constraints = RigidbodyConstraints2D.FreezeRotation;
                    //rbody.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                    limitJumpPos = tr.position.y + jumpLimit;
                    //StartCoroutine(CountJumpPosIE());
                }  
            }
            if (isjumping == true)
            {
                rbody.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                animator.SetTrigger("Landing");
                isjumping = false;
            }

  
        }
        else if (!IsGround())
        {
            if(rbody.velocity.y <0)
            {
                isLanding = true;
            }
            //StartCoroutine(CountJumpPosIE());
            //rbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    void JumpDown()
    {
        if (tr.position.y > limitJumpPos && PlayerEvent.instance.isClimb != true)
        {
            //Debug.Log(tr.position.y);
            rbody.velocity = Vector2.down * jumpForce;
            //animator.SetTrigger("JumpDown");
        }
    }  

    //닌자공격 받았을시 닌자쪽으로 끌려가게 하는 함수
    
    
    void AttractToNinja()
    {
        tr.SetParent(Mon_NinjaCtrl.instance.atkPoint.transform);
        
    }

    public void ForceAdded(Vector2 dir)
    {
        rbody.AddForce(dir);
    }

}
