using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMake : MonoBehaviour {

    public GameObject item;

    public static ItemMake instance;

    private void Awake()
    {
        instance = this;
    }

}
