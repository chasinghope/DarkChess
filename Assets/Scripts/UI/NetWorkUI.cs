using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetWorkUI : MonoBehaviour
{
    public Button CreateBtn;
    public Button JoinBtn;
    public Button ReturnBtn;

    private void Start()
    {
        CreateBtn.onClick.AddListener(Create_onClick);
        JoinBtn.onClick.AddListener(Join_onClick);
        ReturnBtn.onClick.AddListener(ReturnBtn_onClick);
    }


    void Create_onClick()
    {
        GameManager.Instance.PlayButtonClickSound();
        gameObject.SetActive(false);
        UIManager.Instance.GamePanel.gameObject.SetActive(true);
        GameManager.Instance.StartAsServer();
    }

    void Join_onClick()
    {
        GameManager.Instance.PlayButtonClickSound();
        // gameObject.SetActive(false);
        UIManager.Instance.FindRoomPanel.gameObject.SetActive(true);
        GameManager.Instance.StartAsClient();
    }

    void ReturnBtn_onClick()
    {
        GameManager.Instance.PlayButtonClickSound();
        gameObject.SetActive(false);
    }
}
