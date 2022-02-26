using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundChange : MonoBehaviour {

    public enum SoundSTATE
    {
        PRISON = 0,
        LAVA,
        ICE,
        BOSS
    }

    public SoundSTATE soundstate = SoundSTATE.PRISON;

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject bgm = null;
        bgm = GameObject.Find("BGM");
        if (other.tag == "Player")
        {
            switch (soundstate)
            {
                case SoundSTATE.PRISON:
                    bgm.GetComponent<AudioSource>().clip = SoundManager.instance.prisonBgm;
                    bgm.GetComponent<AudioSource>().Play();
                    //SoundManager.instance.bgmSource.clip = SoundManager.instance.prisonBgm;
                    break;
                case SoundSTATE.LAVA:
                    bgm.GetComponent<AudioSource>().clip = SoundManager.instance.lavaBgm;
                    bgm.GetComponent<AudioSource>().Play();
                    //SoundManager.instance.bgmSource.clip = SoundManager.instance.lavaBgm;
                    break;
                case SoundSTATE.ICE:
                    bgm.GetComponent<AudioSource>().clip = SoundManager.instance.iceBgm;
                    bgm.GetComponent<AudioSource>().Play();
                    //SoundManager.instance.bgmSource.clip = SoundManager.instance.iceBgm;
                    break;
                case SoundSTATE.BOSS:
                    bgm.GetComponent<AudioSource>().clip = SoundManager.instance.bossBgm;
                    bgm.GetComponent<AudioSource>().Play();
                    //SoundManager.instance.bgmSource.clip = SoundManager.instance.bossBgm;
                    break;
            }
        }
    }
}
