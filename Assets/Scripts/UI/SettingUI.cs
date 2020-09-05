using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{

    public Button bgmBtn;
    public Button soundBtn;

    public Sprite bgmClose;
    public Sprite soundClose;

    Sprite bgmOpen;
    Sprite soundOpen;

    bool bgmIsMute = false;
    bool soundIsMute =false;

    void Start()
    {
        bgmOpen = bgmBtn.GetComponent<Image>().sprite;
        soundOpen = soundBtn.GetComponent<Image>().sprite;

        bgmBtn.onClick.AddListener(BGM_onClick);
        soundBtn.onClick.AddListener(Sound_onClick);
    }


    void BGM_onClick()
    {
        GameManager.Instance.PlayButtonClickSound();

        if (bgmIsMute)
        {
            MusicMgr.Instance.ChangeBGMVolume(1f);
            bgmBtn.GetComponent<Image>().sprite = bgmOpen;
        }
        else
        {
            MusicMgr.Instance.ChangeBGMVolume(0f);
            bgmBtn.GetComponent<Image>().sprite = bgmClose;
        }

        bgmIsMute = !bgmIsMute;
    }


    void Sound_onClick()
    {
        GameManager.Instance.PlayButtonClickSound();

        if (soundIsMute)
        {
            MusicMgr.Instance.ChangeSoundVolume(1f);
            soundBtn.GetComponent<Image>().sprite = soundOpen;
        }
        else
        {
            MusicMgr.Instance.ChangeSoundVolume(0f);
            soundBtn.GetComponent<Image>().sprite = soundClose;
        }

        soundIsMute = !soundIsMute;
    }

}
