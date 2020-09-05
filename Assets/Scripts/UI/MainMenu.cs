using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainMenu : MonoBehaviour
{
    public Button NetMode_Btn;
    public Button Help_Btn;
    public Button Quit_Btn;


    public void Start()
    {
        // 按钮事件绑定
        NetMode_Btn.onClick.AddListener(NetMode_OnClick);
        Help_Btn.onClick.AddListener(Help);
        Quit_Btn.onClick.AddListener(Quit);
    }





    void NetMode_OnClick()
    {
        GameManager.Instance.PlayButtonClickSound();
        UIManager.Instance.NetWorkPanel.gameObject.SetActive(true);
    }

    void Help()
    {
        GameManager.Instance.PlayButtonClickSound();
        UIManager.Instance.HelpPanel.gameObject.SetActive(true);

    }

    void Quit()
    {
        GameManager.Instance.PlayButtonClickSound();
        Application.Quit();
    }

}
