using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour {

    public int monCurrentNum = 0;
    public float monCurrentHP = 0;

    public GameObject[] monster;
    public int monsterCount = 0;
    public GameObject[] gateKeeper;
    public int gateCount = 0;

    public GameObject hpball;

    public static MonsterManager instance;

    private void Awake()
    {
        instance = this;
    }

}
