using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrologueManager : MonoBehaviour {

    GameObject[] prologueScene;   //이미지 저장 배열
    AudioSource _aud;

    int count = 0;
    float timer = 0f;

    void Awake () {

        prologueScene = new GameObject[GameObject.Find("PrologueImg").transform.childCount];

        _aud = GetComponent<AudioSource>();

        for (int i = 0; i < prologueScene.Length; i++)  //이미지 크기(갯수)만큼 저장
        {
            prologueScene[i] = GameObject.Find("PrologueImg").transform.GetChild(i).gameObject;
        }    
    }


    void Update () {

        timer += Time.smoothDeltaTime;
        if(timer >= 3)
        {
            DeletePrologueScene();
            timer = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            DeletePrologueScene();
            timer = 0;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartGameScene();
        }
    }

    void DeletePrologueScene()
    {
        if(count == prologueScene.Length - 1)
        {
            StartGameScene();          
        }
        else
        {
            Destroy(prologueScene[count].gameObject);
            count++;
        }
    }

    void StartGameScene()
    {
        SceneManager.LoadScene("GameScene");       
    }
}
