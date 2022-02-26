using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour {

    public GameObject player;

    Transform tr;
    public Transform pl;   //2018-11-18

    public float updownViewSize = 5f;
    public float delay = 1.0f;
    public float distance = 10.0f;

    public bool iscamPosUp = false;
    public bool iscamPosSit = false;

    public static FollowCam instance;

    private void Awake()
    {
        instance = this;
        tr = transform;
        pl = player.transform;
    }

    // Update is called once per frame
    void LateUpdate () {
        MoreView();
    }

    void CamPos()
    {
        iscamPosUp = false;
        iscamPosSit = false;
        tr.position = new Vector3(pl.position.x, pl.position.y + distance, tr.position.z);
    }

    void CamPosUp()
    {
        iscamPosUp = true;
        Vector3 otherViewtr = new Vector3(pl.position.x, pl.position.y + distance + updownViewSize, tr.position.z);
        tr.position = Vector3.Lerp(tr.position, otherViewtr, delay * Time.deltaTime); 
    }

    void CamPosDown()
    {
        iscamPosSit = true;
        Vector3 otherViewtr = new Vector3(pl.position.x, pl.position.y + distance - updownViewSize, tr.position.z);
        tr.position = Vector3.Lerp(tr.position, otherViewtr, delay * Time.deltaTime);
    }

    void MoreView()
    {
        if (Input.GetKey(KeyCode.UpArrow) && PlayerControl.instance.isjumping != true && PlayerEvent.instance.isClimb != true)
        {
            CamPosUp();
        }
        else if (Input.GetKey(KeyCode.DownArrow) && PlayerControl.instance.isjumping != true && PlayerEvent.instance.isClimb != true)
        {
            CamPosDown();
        }
        else { CamPos(); }
    }

}
