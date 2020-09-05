using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 控制页面之间的显示与跳转
/// 按钮的触发方式
/// 在GameManager之后实例化
/// </summary>
public class UIManager : MonoBehaviour
{
    #region 单例模式
    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    #endregion

    public Image MainMenuPanel;
    public Image NetWorkPanel;
    public Image FindRoomPanel;
    public Image GamePanel;
    public Image HelpPanel;

    private void Start()
    {
        // 面板初始化
        MainMenuPanel.gameObject.SetActive(true);
        NetWorkPanel.gameObject.SetActive(false);
        FindRoomPanel.gameObject.SetActive(false);
        GamePanel.gameObject.SetActive(false);
        HelpPanel.gameObject.SetActive(false);

    }

}
