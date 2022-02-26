using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerEventPlus : MonoBehaviour {


    //2018-11-07 트랩의 배열화

    //스크립트 수정 및 추가
    //ButtonCtrl
    //PrologueManager
    //UIStartScene
    //Statue
    //Statue_Missile
    //BridgeCtrl(?)
    //IronDoor(?)
    //PlayerEventPlus
    //GameManager
    //UIKeyManual
    //PlayerControl 함수 추가
    //FollowCam 변수 public
    //Brother_Switch 추가
    //추가로 프리팹의 canvas 수정


    //2018.11.16
    //키보틀->문사라짐(이게 lockdoor)  --(확인)
    //플라즈마보틀->e키 입력시 플라즈마보틀 없어지면서 엔딩문 오픈
    //브라더스 보틀->2개로 분열  --(실험) ->카메라 이동확인
    //옵션창
    //오브젝트 음악삽입(미정)
    //게임오버 --(미정)

    //playerChange 플라즈마 추가

    //destroy zone


    float timer = 0;
    bool isDamage = false;
    bool isDie = false;

    public bool isOpen = false;  //여닫이문

    Vector2 presentPos;  //현재 위치값(irondoor)
    float doorSpeed = 2.0f;


    bool FireOn_Off = false;

    public Transform savePoint;
    Transform tr;

    Animator animator;

    bool object_State = false;
    public static PlayerEventPlus instance;


    Transform[] trabObj;   //2018-11-07 trab 배열화
    public Transform[] brotherSwitch;  //2018-11-14
    bool NPCTouch = false;
    public int NPCCount = 0;
    public Transform[] NPCtext;

    private void Awake()
    {
        instance = this;
        tr = transform;

        NPCtext = new Transform[GameObject.Find("NPC").transform.GetChild(0).transform.GetChild(0).childCount];

        for (int i = 0; i < NPCtext.Length; i++)
        {
            NPCtext[i] = GameObject.Find("NPC").transform.GetChild(0).transform.GetChild(0).transform.GetChild(i);
        }

        /********************트랩의 배열화******************************/
        trabObj = new Transform[GameObject.Find("Trab").transform.childCount];

        for (int i = 0; i < trabObj.Length; i++)
        {
            trabObj[i] = GameObject.Find("Trab").transform.GetChild(i);
        }
        /**************************************************************/

        /********************스위치의 배열화******************************/
        brotherSwitch = new Transform[GameObject.FindWithTag("Brother_Switch").transform.childCount];

        for (int i = 0; i < brotherSwitch.Length; i++)
        {
            brotherSwitch[i] = GameObject.Find("Brother_Switch").transform.GetChild(i);
        }
        /**************************************************************/

        animator = GetComponent<Animator>();
    }

    void Update()
    {

        if (NPCCount == NPCtext.Length)
        {
            StartCoroutine(NPCEnd());
        }

        //Debug.Log(GameManager.instance.playerHP);

    }

    void FixedUpdate() {

        //GameObject.Find("irondoor").transform.Rotate(0, 0, 1);   //1도씩 서서히 회전
        //GameObject.Find("irondoor").transform.rotation = Quaternion.Euler(0, 0, 90);  //90도 즉각 회전
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Trab")  //태그 Trab 진입시
        {
            object_State = true;
        }
        else if(other.tag == "Lock_Door" && GameControl.instance.isKeyOpen )
        {
            //보틀은 e버튼 누르면 사용
            //여기서는 닿으면 문이 사라짐
            StartCoroutine(DestroyDoor(other));
        }
    }

    private void OnTriggerStay2D(Collider2D other)  //닿아 있는동안 계속 실행
    {
        if (other.tag == "Trab" && !UIManager.instance.isInvin)  //상태이상 돌입시
        {
            if (trabObj[0])  //acidfloorobj
            {
            
                StartCoroutine(DamageIE(30));
                timer += Time.smoothDeltaTime;

                if (timer >= 5)
                {
                    GameManager.instance.playerHP -= 30;
                    timer = 0;                      
                }

            }
            else if (trabObj[1])  //lavaFloor라는 이름을 가진 오브젝트에 진입시 피격
            {
                StartCoroutine(DamageIE(30));

                if (timer >= 5)
                {
                    timer += Time.smoothDeltaTime;

                    GameManager.instance.playerHP -= 50;
                    timer = 0;
                }
            }
            else if (trabObj[2])
            {
                StartCoroutine(DamageIE(30));
            }
        }

        if (other.tag == "Statue_Missile" && !UIManager.instance.isInvin)
        {
            StartCoroutine(DamageIE(30));
            Destroy(other.gameObject);
        }

        if (other.tag == "Tiled_Trab" && !UIManager.instance.isInvin)
        {
            StartCoroutine(DamageIE(30));
        }

        else   //상태이상 해제시
        {
            object_State = false;
            if (object_State == false)  //상태이상 해제시 5초당 체력5 회복
            {
                timer += Time.smoothDeltaTime;
                if (timer >= 5)
                {
                    if (GameManager.instance.playerHP < 100) //플레이어 HP가 100미만일때 체력2 회복
                    {
                        GameManager.instance.playerHP += 2;

                        if (GameManager.instance.playerHP >= 100)  //플레이어 HP가 100이상일때 체력을 100으로
                        {
                            GameManager.instance.playerHP = 100;
                        }
                    }
                    timer = 0;
                }
            }
        }


        if (other.tag == "SavePoint" && Input.GetKeyDown(KeyCode.F))
        {
            savePoint = other.gameObject.transform;

            other.GetComponent<Animator>().SetTrigger("Player_Save");
            Invoke("SavePointAni", 0.1f);
        }

        if (other.tag == "Door_Point")
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (!isOpen)
                {
                    other.transform.GetChild(0).gameObject.SetActive(false);  //해당 부모의 자식객체 (1번째) 비활성화
                    other.transform.GetChild(1).gameObject.SetActive(true);   //해당 부모의 자식객체 (2번째) 활성화

                    Invoke("OpenTimer", 0.1f);
                }
                else if (isOpen)
                {
                    other.transform.GetChild(0).gameObject.SetActive(true);
                    other.transform.GetChild(1).gameObject.SetActive(false);

                    Invoke("CloseTimer", 0.1f);
                }
            }
        }


        if (other.tag == "Switch" && Input.GetKeyDown(KeyCode.F))  //Switch에 닿았을때
        {
            if (!other.transform.parent.transform.GetChild(1).GetComponent<IronDoor>().isSwitchCheck)//!isSwitch)      //isSwitch가 false일때
            {
                StartCoroutine(IronDoorMove(other));  //상승
            }
            else if (other.transform.parent.transform.GetChild(1).GetComponent<IronDoor>().isSwitchCheck)  //isSwitch가 true일때
            {
                StartCoroutine(IronDoorMove(other));  //하강
            }
        }

        if (other.tag == "Wheel")
        {
            Transform[] wheelchild = new Transform[other.transform.childCount];

            if (Input.GetKeyDown(KeyCode.F))
            {
                Invoke("ImageOut", 0f);
                Invoke("ControllerOut", 0f);
                other.GetComponent<Animator>().SetTrigger("Play");
                Invoke("ImageIn", 1f);
                Invoke("ControllerIn", 1f);

                if (!FireOn_Off) //false상태일시(꺼진상태)
                {

                    for (int i = 0; i < wheelchild.Length; i++)
                    {
                        wheelchild[i] = other.transform.GetChild(i);
                        other.transform.GetChild(i).GetComponent<Animator>().SetBool("Fire_On_Off", true);  //false상태일시 불킴
                        other.transform.GetChild(i).transform.GetChild(0).gameObject.SetActive(true);
                    }

                    Invoke("FireOn_Off_true", 1.0f);
                }
                else if (FireOn_Off)  //true상태일시(켜진상태)
                {

                    for (int i = 0; i < wheelchild.Length; i++)
                    {
                        other.transform.GetChild(i).GetComponent<Animator>().SetBool("Fire_On_Off", false);  //true상태일시 불끔
                        other.transform.GetChild(i).transform.GetChild(0).gameObject.SetActive(false);
                    }

                    Invoke("FireOn_Off_false", 1.0f);
                }
            }
        }




        if (other.name == "Potal2_1" && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(Potal2(other));
        }
        else if (other.name == "Potal2_2" && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(Potal2(other));
        }

        //////////////////11.16///////////////////////////////// 엔딩 문
        if (other.name == "IceDoor" && GameControl.instance.isEndingDoor)
        {
            GameObject.Find("IceDoor").GetComponent<Animator>().SetBool("IsOpen", true);
            GameObject.Find("IceDoor").GetComponent<Collider2D>().isTrigger = true;

            Invoke("EndingScene", 2.0f);
        }

        if (other.tag == "NPC" && Input.GetKeyDown(KeyCode.F) && !NPCTouch)
        {
            StartCoroutine(NPC(other));         
        }

    }

    ////////////////////엔딩신으로//////////////////
    void EndingScene()
    {
        SceneManager.LoadScene("ResultScene");
    }
    ///////////////////////////////////////////////////

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.collider.tag == "MeltIronDoor")
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                StartCoroutine(MeltIronDoor(other.collider));
            }
        }
        else if (other.collider.tag == "SlimeMeltFloor" && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(SlimeMeltFloorIE());
            other.collider.GetComponent<Animator>().SetTrigger("Play");

            tr.position = other.transform.GetChild(0).position;
        }

        

        if(other.collider.tag == "Destroy_Zone")
        {
            StartCoroutine(DamageIE((int)GameManager.instance.playerHP));
        }


    }

    void FireOn_Off_true()
    {
        FireOn_Off = true;
    }
    void FireOn_Off_false()
    {
        FireOn_Off = false;
    }

    IEnumerator NPC(Collider2D other)  //플레이어 정지->텍스트->애니매이션 활성화->능력 ok, 플레이어 움직임->npc죽음
    {
        other.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        NPCTouch = true;
        GetComponent<PlayerControl>().enabled = false;
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator NPCEnd()  //플레이어 정지->텍스트->애니매이션 활성화->능력 ok, 플레이어 기동ok->npc죽음
    {
        GameObject obj = GameObject.Find("NPC");
        UIadmin.instance.playerState.SetActive(true);
        obj.GetComponent<Animator>().SetTrigger("Die");
        GetComponent<PlayerControl>().enabled = true;
        yield return new WaitForSeconds(1.5f);
        obj.GetComponent<Animator>().SetTrigger("DieIdle");
    }

    IEnumerator Potal2(Collider2D other)
    {
        if (other.name == "Potal2_1" && Input.GetKeyDown(KeyCode.F))  //부모->자식
        {
            other.GetComponent<Animator>().SetTrigger("In");    
            Invoke("ImageOut", 0f);                             
            Invoke("ControllerOut", 0f);
            yield return new WaitForSeconds(1.5f);                
            tr.transform.position = other.transform.GetChild(0).transform.position;  
            other.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Out");   
            Invoke("ControllerIn", 1f);
            Invoke("ImageIn", 1f);                              
        }
        else if (other.name == "Potal2_2" && Input.GetKeyDown(KeyCode.F))
        {
            other.GetComponent<Animator>().SetTrigger("In");
            Invoke("ImageOut", 0f);
            Invoke("ControllerOut", 0f);
            yield return new WaitForSeconds(1.5f);
            tr.transform.position = other.transform.parent.transform.position;
            other.transform.parent.GetComponent<Animator>().SetTrigger("Out");
            Invoke("ControllerIn", 1f);
            Invoke("ImageIn", 1f);
        }
    }

    IEnumerator MeltIronDoor(Collider2D other)
    {
        other.GetComponent<Animator>().SetTrigger("Play");
        yield return new WaitForSeconds(1.0f);
        other.GetComponent<Animator>().SetBool("Die", true);
        other.isTrigger = true;
    }

    IEnumerator IronDoorMove(Collider2D other)
    {
        if (!other.transform.parent.transform.GetChild(1).GetComponent<IronDoor>().isSwitchCheck)//!isSwitch)  //(1)isSwitch가 false, 현재 위치가 endpos.y보다 작을때
        {
            other.GetComponent<Animator>().SetBool("isOpen", true);  //(2)스위치 open애니메이션 실행
            
            while (true)
            {               
                presentPos = other.transform.parent.transform.GetChild(1).transform.position; //irondoor의 현재 위치
                other.transform.parent.transform.GetChild(1).Translate(Vector2.up * doorSpeed * Time.smoothDeltaTime); //(3)irondoor상승

                yield return new WaitForSeconds(0.02f);

                if(presentPos.y > other.transform.parent.transform.GetChild(1).GetComponent<IronDoor>().endPos.y)  //(4)if..현재 위치가 endpos.y보다 클때
                {
                    other.transform.parent.transform.GetChild(1).GetComponent<IronDoor>().isSwitchCheck = true;
                    break;             //(6)반복문 벗어나감
                }
            }
        }
        else  //(1)isSwitch가 false, 현재 위치가 endpos.y보다 클때
        {
            other.GetComponent<Animator>().SetBool("isOpen", false);  //(2)스위치 false애니메이션 실행
            
            while (true)
            {
                presentPos = other.transform.parent.transform.GetChild(1).transform.position; //irondoor의 현재 위치
                other.transform.parent.transform.GetChild(1).Translate(Vector2.down * doorSpeed * Time.smoothDeltaTime);  //(3)irondoor하강

                yield return new WaitForSeconds(0.02f);

                if (presentPos.y <= other.transform.parent.transform.GetChild(1).GetComponent<IronDoor>().startPos.y)  //(4)if..현재 위치가 startpos.y보다 작을때
                {
                    other.transform.parent.transform.GetChild(1).GetComponent<IronDoor>().isSwitchCheck = false;
                    break;                //(6)반복문 벗어나감
                }
            }
        }
    }
   
    void OpenTimer()
    {
        isOpen = true;
    }

    void CloseTimer()
    {
        isOpen = false;
    }

    IEnumerator DestroyDoor(Collider2D other)
    {
        other.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        other.gameObject.transform.GetChild(1).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        Destroy(other.gameObject);
    }

    IEnumerator SlimeMeltFloorIE()
    {
        Invoke("ImageOut", 0.1f);
        Invoke("ControllerOut", 0.1f);    
        yield return new WaitForSeconds(2.0f);
        Invoke("ImageIn", 0.1f);     
        Invoke("ControllerIn", 0.1f);
    }

    IEnumerator DamageIE(int damage) //피격시   
    {
        if (!isDamage)
        {
            isDamage = true;
            animator.SetTrigger("Damage");
            GameManager.instance.playerHP -= damage;
            if (GameManager.instance.playerHP <= 0)
            {
                isDie = true;
                animator.SetTrigger("Die");
                this.GetComponent<PlayerControl>().enabled = false;  //플레이어 컨트롤 x

                GameObject obj = GameObject.Find("GameOver_Canvas").transform.GetChild(0).gameObject;
                obj.SetActive(true);
                yield return new WaitForSeconds(1.7f);
                obj.SetActive(false);

                if (savePoint == null)
                {
                    tr.position = GameManager.instance.startPoint.transform.position;
                }
                else
                {
                    tr.position = savePoint.position;
                }

                this.GetComponent<PlayerControl>().enabled = true;   //플레이어 컨트롤 o 
                
                isDamage = false;
                isDie = false;
                GameManager.instance.playerHP = 100;
            }
            else if (GameManager.instance.playerHP > 0)
            {               
                this.GetComponent<PlayerControl>().enabled = false;
                yield return new WaitForSeconds(0.5f);
                this.GetComponent<PlayerControl>().enabled = true;
                yield return new WaitForSeconds(5.0f);
                isDamage = false;
            }
        }
    }

    void SavePointAni()
    {
        Invoke("ImageOut", 0.1f);
        Invoke("ControllerOut", 0.1f);
        tr.position = new Vector2(savePoint.position.x + 10, savePoint.position.y);
        Invoke("ImageIn", 2.0f);
        Invoke("ControllerIn", 2.0f);
    }

    void ImageOut()
    {
        this.GetComponent<SpriteRenderer>().enabled = false; //이미지 아웃
    }

    void ImageIn()
    {
        this.GetComponent<SpriteRenderer>().enabled = true; //이미지 인
    }

    void ControllerOut()
    {
        this.GetComponent<PlayerControl>().enabled = false; //플레이어 컨트롤러 아웃
    }

    void ControllerIn()
    {
        this.GetComponent<PlayerControl>().enabled = true; //플레이어 컨트롤러 인
    }

    

    /*
    void OnDisable()   //객체 비활성화시 실행되는 함수
    {
        Debug.Log("gfgg");
    }
     
    void OnEnable()    //객체 활성화시 실행되는 함수
    {
        Debug.Log("gfgg");
    }

    void OnClick()     //객체 클릭시 실행되는 함수
    {
        Debug.Log("gd");
    }
    */

    private void OnTriggerExit2D(Collider2D other)
    {
        timer = 0;
    }
}
