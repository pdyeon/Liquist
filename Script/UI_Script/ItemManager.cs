using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour {

    public ArrayList itemList = new ArrayList();

    public GameObject tel_Item;
    public GameObject spear_Item;

    public static ItemManager instance;

    private void Awake()
    {
        instance = this;
    }

}
