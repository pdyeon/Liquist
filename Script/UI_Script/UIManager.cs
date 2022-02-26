using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    public bool isUIMake = false; // Cavas가 게임상에 존재하냐?
    public bool isUIAllClose = true; //전체 UI가 닫혀있냐?
    public bool isUIOpen = false; // UI가 하나라도 열려있냐?
    public GameObject saveAllUI;

    public bool isInvenOpen = false;
    public bool isMenuOpen = false;
    public bool isSettingOpen = false;

    public bool isFullbottle1;
    public bool isFullbottle2;
    public bool isFullbottle3;

    public bool isEmptybottle1;
    public bool isEmptybottle2;

    public int bottle1Num;
    public int bottle2Num;
    public int bottle3Num;

    public bool isInvin;

    public int soundCount;

    public static UIManager instance;

    private void Awake()
    {
        instance = this;

        isFullbottle1 = false;
        isFullbottle2 = false;
        isFullbottle3 = false;

        isEmptybottle1 = false;
        isEmptybottle2 = false;

        bottle1Num = -1;
        bottle2Num = -1;
        bottle3Num = -1;

        isInvin = false;

        soundCount = 4;
    }

}
