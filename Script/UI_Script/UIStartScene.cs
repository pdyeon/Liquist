using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class UIStartScene : MonoBehaviour {

    //메인화면->게임시작->프롤로그(1~8)-space 입력시 한씬씩 패스->인게임->최종보스->문이 생기고 닫히면->결과화면->메인화면

    public GameObject option;

    public void StartSceneChange()
    {
        //게임시작->프롤로그 전환->인게임씬
        SceneManager.LoadScene("PrologueScene");
    }

    public void OptionButton()
    {
        //옵션 씬
        Instantiate(option);
        //SceneManager.LoadScene("OptionScene");
    }

    public void ExitButton()
    {
        //게임 종료
        Application.Quit();
    }
}
