using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaticleManager : MonoBehaviour {

    public GameObject paticle;

    //플레이어
    public GameObject player_DMG;
    public GameObject player_Change;
    public GameObject player_Eatmon; //몬스터
    public GameObject player_Eathp;
    public GameObject player_brother_change;

    public GameObject player_itemuse;

    //몬스터
    public GameObject lava_Die;
    public GameObject tele_Attack;
    public GameObject hpball_Attack;
    public GameObject prizma1_Attack1;
    public GameObject prizma1_Attack2;
    public GameObject prizma2_Attack1;
    public GameObject prizma2_Attack2;

    //오브젝트
    public GameObject statue1;
    public GameObject statue2;
    public GameObject statue3;
    public GameObject prison;
    public GameObject item;
    public GameObject map_ice;
    public GameObject map_fire;

    public static PaticleManager instance;

    public Color initcolor;
    public Color targetcolor;

    private void Awake()
    {
        instance = this;
    }

}
