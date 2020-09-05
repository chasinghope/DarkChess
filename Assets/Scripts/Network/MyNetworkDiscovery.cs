using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
// using Boo.Lang;

[RequireComponent(typeof(MyNetManager))]
public class MyNetworkDiscovery : NetworkDiscovery
{

    public MyNetManager netManager;
    private FindRoomUI m_findRoom;
    public FindRoomUI findRoom
    {
        get
        {
            if(m_findRoom == null)
                m_findRoom = GameObject.FindWithTag("FindRoomUI").GetComponent<FindRoomUI>();
            return m_findRoom;

        }
    }

    public List<string> ipList = new List<string>();


    void Start()
    {
        netManager = GameObject.FindWithTag("NetworkManager").GetComponent<MyNetManager>();
    }

    /// <summary>
    /// Called when the broadcast message has received.
    /// </summary>
    /// <param name="fromAddress">An IP address of the broadcast message sender</param>
    /// <param name="data">Custom data with broadcast message</param>
    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        base.OnReceivedBroadcast(fromAddress, data);

        Debug.Log(fromAddress + "/" + data);


        if ( !ipList.Contains(fromAddress) )
        {
            // netManager.serverIP = fromAddress;
            findRoom.AddRoom(fromAddress);
            ipList.Add(fromAddress);
        }
    }


    public void ClearIPList()
    {
        ipList.Clear();
        findRoom.ClearRoom();
    }

}
