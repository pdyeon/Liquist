using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEvent : MonoBehaviour {

    public bool isClimb = false;
    bool isSwitchClick = false; // 코루틴 제어 플레그
    bool isDamage = false; // 코루틴 제어 플레그
    bool isPotal = false;
    bool isgameover = false;

    public Vector2 effpos;

    public RuntimeAnimatorController[] playeranimator;

    //bool addForceVxEnabled = false;
    //float addForceVxStartTime = 0.0f;

    //Transform savePoint;
    Transform tr;

    //public float teleportsize = 1.0f;

    public static PlayerEvent instance;

    Rigidbody2D rbody;
    //Animator animator;

    private void Awake()
    {
        instance = this;
        tr = transform;
        rbody = GetComponent<Rigidbody2D>();
        //animator = GetComponent<Animator>();
    }

    void Update()
    {
        effpos = new Vector2(tr.position.x, tr.position.y + 2);
        //Teleport();
        //ChangeCharactor();
    }

    
    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.tag == "SavePoint")
    //    {
    //        savePoint = other.gameObject.transform;
    //    }

    //}
    
    IEnumerator DamageIE()
    {
        if (!isDamage)
        {
            isDamage = true;
            PlayerControl.instance.animator.SetTrigger("Damage");

            Instantiate(PaticleManager.instance.player_DMG, effpos, tr.rotation);
             
            GameManager.instance.playerHP -= 30;
            this.GetComponent<PlayerControl>().enabled = false;
            if (GameManager.instance.playerHP <= 0)
            {
                PlayerControl.instance.animator.SetTrigger("Die");
                SoundManager.instance.PlaySfx(transform.position, SoundManager.instance.player_die2, 0, SoundManager.instance.bgmVolum);
                StartCoroutine(Gameover());
                GameManager.instance.playerHP = 100;
                //Invoke("GoSavePoint", 2.0f);
                //GameManager.instance.playerHP = 100;
            }
            yield return new WaitForSeconds(0.5f);
            this.GetComponent<PlayerControl>().enabled = true;
            yield return new WaitForSeconds(1.5f);
            isDamage = false;
        }
    }

    IEnumerator Gameover()
    {
        Time.timeScale = 0;
        //이미지 생성 삭제
        UIadmin.instance.gameover.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        UIadmin.instance.gameover.SetActive(false);
        tr.position = PlayerEventPlus.instance.savePoint.position;
        Time.timeScale = 1;
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "MonsterBullet" && !UIManager.instance.isInvin && !PlayerChange.instance.isAttack)
        {
            StartCoroutine(DamageIE());
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        /*
        if (other.tag == "Item" || other.tag == "BItem")
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                if (other.gameObject == ItemManager.instance.tel_Item)
                { GameManager.instance.isTel_Item = true; }

                else if (other.gameObject == ItemManager.instance.spear_Item)
                {
                    BulletManager.instance.currentBullet = BulletManager.instance.Spear;
                    BulletManager.instance.currentDamage = BulletManager.instance.SpearDamage;
                    BulletManager.instance.isSpear = true;
                }

                Destroy(other.gameObject);
                //드롭애니메이션
            }
        }
        */
        if (other.tag == "Ladder")
        {
            isClimb = true;
            rbody.gravityScale = 0;

            if (Input.GetKey(KeyCode.UpArrow))
            {
                rbody.constraints = RigidbodyConstraints2D.FreezeRotation;
                float move = PlayerControl.instance.runSpeed * Time.smoothDeltaTime;
                float key = Input.GetAxis("Vertical");
                transform.Translate(Vector2.up * move * key);
                PlayerControl.instance.animator.SetBool("LadderUp",true);
                //transform.Translate(Vector2.up * move * key);
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                rbody.constraints = RigidbodyConstraints2D.FreezeRotation;
                float move = PlayerControl.instance.runSpeed * Time.smoothDeltaTime;
                float key = Input.GetAxis("Vertical");
                transform.Translate(Vector2.up * move * key);
                PlayerControl.instance.animator.SetBool("LadderDown", true);
                //transform.Translate(Vector2.up * -move * key);
            }
            else
            {
                rbody.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                PlayerControl.instance.animator.SetBool("LadderUp", false);
                PlayerControl.instance.animator.SetBool("LadderDown", false);
            }
        }
        
        if (other.tag == "Potal1")
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                this.GetComponent<SpriteRenderer>().enabled = false; // 이미지 끄기
                other.gameObject.GetComponent<Potal1Ctrl>().InOutPotal(1);
                this.GetComponent<SpriteRenderer>().enabled = false;
                //Invoke("PotalMove1", 1f);
                StartCoroutine(PotalMove1IE(other.gameObject));
                
            }
        }

        if (other.tag == "Potal2")
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                this.GetComponent<SpriteRenderer>().enabled = false; // 이미지 끄기
                other.gameObject.GetComponent<Potal2Ctrl>().InOutPotal(1);
                //Potal2Ctrl.instance.InOutPotal(1);
               
                this.GetComponent<SpriteRenderer>().enabled = false;
                //Invoke("PotalMove2", 1f);
                StartCoroutine(PotalMove2IE(other.gameObject));
            }
        }

        if (other.tag == "Monster" && !UIManager.instance.isInvin && !PlayerChange.instance.isAttack)
        {
            StartCoroutine(DamageIE());
        }

        if (other.tag == "Monster" && PlayerChange.instance.isAttack)
        {
            other.SendMessage("Damaged", SendMessageOptions.DontRequireReceiver);
            //hit.transform.SendMessage("MonCurrentHP", SendMessageOptions.DontRequireReceiver);
        }

    }

    IEnumerator PotalMove2IE(GameObject potal)
    {
        yield return new WaitForSeconds(1f);
        isPotal = true;
        if (isPotal)
        {
            this.GetComponent<SpriteRenderer>().enabled = false;
            tr.position = potal.GetComponent<Potal2Ctrl>().sponPoint.transform.position;//Potal1Ctrl.instance.sponPoint.transform.position;
            potal.GetComponent<Potal2Ctrl>().sponPoint.GetComponent<Potal1Ctrl>().InOutPotal(-1);
            isPotal = false;
            Invoke("PotalShow", 1f);
        }
    }

    IEnumerator PotalMove1IE(GameObject potal)
    {
        yield return new WaitForSeconds(1f);
        isPotal = true;
        if (isPotal)
        {
            this.GetComponent<SpriteRenderer>().enabled = false;
            tr.position = potal.GetComponent<Potal1Ctrl>().sponPoint.transform.position;//Potal1Ctrl.instance.sponPoint.transform.position;
            potal.GetComponent<Potal1Ctrl>().sponPoint.GetComponent<Potal2Ctrl>().InOutPotal(-1);
            isPotal = false;
            Invoke("PotalShow", 1f);
        }
    }

    /*
    void PotalMove1()
    {
        this.GetComponent<SpriteRenderer>().enabled = false;
        tr.position = Potal1Ctrl.instance.sponPoint.transform.position;
        //rbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        Potal2Ctrl.instance.InOutPotal(-1);
        Invoke("PotalShow", 1f);
    }
    */
    /*
    void PotalMove2()
    {
        this.GetComponent<SpriteRenderer>().enabled = false;
        tr.position = Potal2Ctrl.instance.sponPoint.transform.position;
        //rbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        Potal1Ctrl.instance.InOutPotal(-1);
        Invoke("PotalShow", 1f);
    }
    */
    void PotalShow()
    {
        this.GetComponent<SpriteRenderer>().enabled = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Ladder")
        {
            isClimb = false;
            rbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            rbody.gravityScale = 1;
        }
        
    }

    /*
    void Teleport()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (GameManager.instance.isTel_Item == true)
            {
                if (PlayerControl.instance.isRight != false)
                {
                    tr.position = new Vector2(tr.position.x + teleportsize, tr.position.y);
                }
                else if (PlayerControl.instance.isRight != true)
                {
                    tr.position = new Vector2(tr.position.x - teleportsize, tr.position.y);
                }

                GameManager.instance.isTel_Item = false;
            }
        }
    }
    */
    
    public virtual void AddForceAnimatorVx(float vx)
    {
        if (vx != 0.0f)
        {
            if (PlayerControl.instance.isRight == true)
            {
                rbody.AddForce(new Vector2(vx * -1, 0.0f));
                //rbody.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            }
            else if (PlayerControl.instance.isRight == false)
            {
                rbody.AddForce(new Vector2(vx * 1, 0.0f));
                //rbody.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            }

        }
    }

}
