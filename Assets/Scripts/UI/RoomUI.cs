using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomUI : MonoBehaviour
{

    public Button roomBtn;
    public Text display;
    string content;


    // Start is called before the first frame update
    void Start()
    {
        roomBtn.onClick.AddListener( ()=>{ Room_onClick(content); } );
    }

    /// <summary>
    /// 客户端加入游戏
    /// </summary>
    /// <param name="info"></param>
    void Room_onClick(string info)
    {
        GameManager.Instance.PlayButtonClickSound();
        UIManager.Instance.FindRoomPanel.gameObject.SetActive(false);
        UIManager.Instance.NetWorkPanel.gameObject.SetActive(false);


        GameManager.Instance.ClientConnectToserver(info);
        UIManager.Instance.GamePanel.gameObject.SetActive(true);
    }

    public void SetupIP(string ip)
    {
        content = ip;
        display.text = content;
    }


}
