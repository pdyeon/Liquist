using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonCtrl : MonoBehaviour {

    public void OnClickExit()
    {
        Application.Quit();
    }

    public void OnClickMenuX()
    {
        UIManager.instance.isMenuOpen = false;
        UIadmin.instance.menu.SetActive(false);
    }

    public void OnClickMenuSetting()
    {
        UIManager.instance.isSettingOpen = true;
        UIadmin.instance.setting.SetActive(true);
        UIManager.instance.isMenuOpen = false;
        UIadmin.instance.menu.SetActive(false);
    }

    public void OnClickSettingX()
    {
        UIManager.instance.isSettingOpen = false;
        UIadmin.instance.setting.SetActive(false);
    }

    public void OnClickMainMenu()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void OnClickReset()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OnClickBackToSetting()
    {
        UIManager.instance.isSettingOpen = false;
        UIadmin.instance.setting.SetActive(false);
        UIManager.instance.isMenuOpen = true;
        UIadmin.instance.menu.SetActive(true);
    }

    public void OnClickNext()
    {       
         gameObject.transform.parent.transform.GetChild(0).transform.
           GetChild(PlayerEventPlus.instance.NPCCount).gameObject.SetActive(false);

        PlayerEventPlus.instance.NPCCount++;

        if (PlayerEventPlus.instance.NPCCount == PlayerEventPlus.instance.NPCtext.Length)
        {
            gameObject.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            gameObject.transform.parent.transform.GetChild(0).transform.
               GetChild(PlayerEventPlus.instance.NPCCount).gameObject.SetActive(true);
        }
    }

    public void PlusSound()
    {
        if (UIManager.instance.soundCount < 4)
        {
            SoundManager.instance.bgmSource.volume += 20;
            UIManager.instance.soundCount++;
            UIadmin.instance.soundLV[UIManager.instance.soundCount].SetActive(true);
        }

    }

    public void MiusSound()
    {
        if (UIManager.instance.soundCount > -1)
        {
            SoundManager.instance.bgmSource.volume -= 20;
            UIadmin.instance.soundLV[UIManager.instance.soundCount].SetActive(false);
            UIManager.instance.soundCount--;
        }
    }
}
