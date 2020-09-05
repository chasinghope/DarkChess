using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FindRoomUI : MonoBehaviour
{

    public Text FindingText;
    public Button ReturnBtn;


    public GameObject RoomBtn;
    public GameObject ViewContent;

    public List<GameObject> allRoom = new List<GameObject>();


    void Start()
    {
        ReturnBtn.onClick.AddListener(Return_onClick);
        DefaultUI();
    }

    void Return_onClick()
    {
        GameManager.Instance.PlayButtonClickSound();
        GameManager.Instance.Quit_onClick();
        gameObject.SetActive(false);
        UIManager.Instance.NetWorkPanel.gameObject.SetActive(true);
    }

    /// <summary>
    /// 默认UI
    /// </summary>
    public void DefaultUI()
    {
        FindingText.gameObject.SetActive(true);
    }

    /// <summary>
    /// 添加服务器房间
    /// </summary>
    /// <param name="ip"></param>
    public void AddRoom(string ip)
    {
        // 取消显示
        FindingText.gameObject.SetActive(false);
        GameObject go = Instantiate(RoomBtn);
        go.transform.SetParent(ViewContent.transform);
        go.transform.localScale = Vector3.one;
        go.GetComponent<RoomUI>().SetupIP(ip);
        allRoom.Add(go);
    }

    /// <summary>
    /// 清空网络房间
    /// </summary>
    public void ClearRoom()
    {
        foreach (GameObject item in allRoom)
        {
            Destroy(item);
        }
        allRoom.Clear();
    }





}
