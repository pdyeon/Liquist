using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public float bgmVolum = 1.0f; // 배경음

    public AudioSource bgmSource; // BGM전용

    //오디오 클립
    //배경
    public AudioClip prisonBgm; // 메인 배경음
    public AudioClip lavaBgm;
    public AudioClip iceBgm;
    public AudioClip bossBgm;

    //UI
    public AudioClip button;
    public AudioClip hpHeal;
    public AudioClip hpUp;
    public AudioClip itemUse;

    //NPC
    public AudioClip NPC1;
    public AudioClip NPC2;
    public AudioClip NPC3;
    public AudioClip NPC4;

    //몬스터
    //hpball
    public AudioClip hp_attack;
    public AudioClip hp_die;
    public AudioClip hp_Damage;

    //ninja
    public AudioClip ninja_Damage;
    public AudioClip ninja_Die;
    public AudioClip ninja_jump;
    public AudioClip ninja_shurikan;
    public AudioClip ninja_shortAttack;

    //lava
    public AudioClip lava_bullet;
    public AudioClip lava_attack;
    public AudioClip lava_die;

    //rabbit
    public AudioClip rabbit_Damage;
    public AudioClip rabbit_Die;
    public AudioClip rabbit_Kick1;
    public AudioClip rabbit_Kick2;

    //sneeky
    public AudioClip sneeky_Attack;
    public AudioClip sneeky_Die;

    //teleball
    public AudioClip tele_bullet;
    public AudioClip tele_Die;
    public AudioClip tele_attack;
    public AudioClip tele_Damage;
    public AudioClip tele_tel;

    //prizma1
    public AudioClip pri1_bullet;
    public AudioClip pri1_charge;
    public AudioClip pri1_splash;
    public AudioClip pri1_Damage;
    public AudioClip pri1_Die;
    public AudioClip pri1_groundHit;
    public AudioClip pri1_move;

    //prizma2
    public AudioClip pri2_Damage;
    public AudioClip pri2_appear;
    public AudioClip pri2_attack;
    public AudioClip pri2_Die1;
    public AudioClip pri2_Die2;

    //item
    public AudioClip get_item;

    //object
    public AudioClip door;
    public AudioClip irondoor;
    public AudioClip ladder;
    public AudioClip meltfloor;
    public AudioClip save;
    public AudioClip fireball;
    public AudioClip updowndoor;
    public AudioClip warp;
    public AudioClip key;

    //player
    public AudioClip player_die1;
    public AudioClip player_die2;
    public AudioClip player_move;
    public AudioClip player_jump;
    public AudioClip player_jumpAttack;

    public static SoundManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void PlaySfx(Vector3 pos, AudioClip sfx, float delayed, float volum)
    {
        StartCoroutine(PlaySfxDelayed(pos, sfx, delayed, volum));
    }

    IEnumerator PlaySfxDelayed(Vector3 pos, AudioClip sfx, float delayed, float volum)
    {
        yield return new WaitForSeconds(delayed);

        GameObject sfxObj = new GameObject("Sfx");
        sfxObj.transform.position = pos;

        AudioSource _audioSource = sfxObj.AddComponent<AudioSource>();

        _audioSource.clip = sfx;

        _audioSource.minDistance = 5.0f;
        _audioSource.maxDistance = 10.0f;
        _audioSource.volume = volum;

        _audioSource.Play();

        Destroy(sfxObj, sfx.length); // 효과음 종료되면 삭제

    }

    public void PlayBGM(AudioClip bgm, float delayed, bool loop)
    {
        StartCoroutine(PlayerBGM_Delayed(bgm, delayed, loop));
    }

    IEnumerator PlayerBGM_Delayed(AudioClip bgm, float delayed, bool loop)
    {
        yield return new WaitForSeconds(delayed);

        GameObject bgmObj = new GameObject("BGM");

        if (!bgmSource) bgmSource = bgmObj.AddComponent<AudioSource>();

        bgmSource.clip = bgm;

        bgmSource.volume = bgmVolum;
        bgmSource.loop = loop;
        bgmSource.Play();
    }
}
