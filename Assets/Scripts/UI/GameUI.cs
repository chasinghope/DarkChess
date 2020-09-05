using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameUI : MonoBehaviour
{
    [Header("按钮")]
    public Button ReadyBtn;
    public Button LayoutBtn;
    public Button GiveUpBtn;
    public Button ExitBtn;
    public Text ExplainText;

    void Start()
    {
        RegisterBtnEvent();
        ExplainText.text = "";
    }

    #region 按钮事件

    void RegisterBtnEvent()
    {
        ReadyBtn.onClick.AddListener(Ready_onClick);
        LayoutBtn.onClick.AddListener(Layout_onClick);
        GiveUpBtn.onClick.AddListener(Giveup_onClick);
        ExitBtn.onClick.AddListener(Exit_onClick);
    }

    void Ready_onClick()
    {
        ReadyBtn.transform.DOPunchPosition(new Vector3(0, 1, 0) * 5, 2f, 4, 0);
        GameManager.Instance.PlayButtonClickSound();
        GameManager.Instance.Ready_onClick();
    }

    void Layout_onClick()
    {
        LayoutBtn.transform.DOPunchPosition(new Vector3(0, 1, 0) * 5, 2f, 4, 0);
        GameManager.Instance.PlayButtonClickSound();
        GameManager.Instance.Ok_onClick();
    }

    void Giveup_onClick()
    {
        GiveUpBtn.transform.DOPunchPosition(new Vector3(0, 1, 0) * 5, 2f, 4, 0);
        GameManager.Instance.PlayButtonClickSound();
        GameManager.Instance.Giveup_onClick();
    }

    void Exit_onClick()
    {
        ExitBtn.transform.DOPunchPosition(new Vector3(0, 1, 0) * 5, 2f, 4, 0);
        GameManager.Instance.PlayButtonClickSound();
        GameManager.Instance.Quit_onClick();
        GameManager.Instance.PlayMainBGM();
        // UI控制
        UIManager.Instance.MainMenuPanel.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    #endregion

    /// <summary>
    /// 设置文本信息
    /// </summary>
    /// <param name="info">文本信息内容</param>
    public void SetExplainText(string info)
    {
        ExplainText.text = info;
    }
}
