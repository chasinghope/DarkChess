using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


/// <summary>
/// The custom networkmanager derived from 'NetworkManager'.
/// you can override virtual function to implement customize behaviour.
/// </summary>
public class MyNetManager : NetworkManager
{

    // [Header("UI引用")]
    // public Text receivedText;
    // public InputField inputfield;

    [Header("配置数据")]
    public string address;
    public string uid;

    [Header("引用")]
    public MyNetworkDiscovery discovery;

    public bool isConnecting = false;


    PlayerManager playerManager;
    NetworkClient myClient;


    public string targetUid;

    public delegate void Receive(string info);
    public event Receive ReceiveEvent;

    void Start()
    {
        address = null;
        uid = SystemInfo.deviceUniqueIdentifier;
        playerManager = PlayerManager.Instance;
        myClient = new NetworkClient();


    }

    /** Declare NetworkMessage class **/
    public class UidMessage : MessageBase
    {
        public string uid;
    };

    public class CustomMessage : MessageBase
    {
        public string text;
    };

    public class MyMsgType
    {
        // Unique device id
        public static short UID = MsgType.Highest + 1;
        // Your custom message type
        public static short Custom = MsgType.Highest + 2;
    };




    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("客户端是否连接上服务器了 ： " + myClient.isConnected);
        }
    }





    #region 私有方法 

    /// <summary>
    /// 情况发现的服务器列表
    /// </summary>
    void ClearIPList()
    {
        discovery.ClearIPList();
    }




    #endregion






    /** Callback functions **/
    public void OnConnected(NetworkMessage netMsg)
    {
        Debug.Log("Connected :" + netMsg.conn.address);
        isConnecting = true;
        // if started as client
        if (!NetworkServer.active)
        {
            Debug.Log("client 停止广播接收");
            sendUid();
            discovery.StopBroadcast();
            GameManager.Instance.gameUI.SetExplainText("已连接至服务器，请准备");
        }
        else
        {
            GameManager.Instance.gameUI.SetExplainText("有客户端加入房间，请准备");
        }
    }

    public void OnDisconnected(NetworkMessage netMsg)
    {
        Debug.Log("Disconnected :" + netMsg.conn.address);
        isConnecting = false;

        if (!discovery.running)
        {
            myClient.ReconnectToNewHost(address, 4444);
        }


        if (NetworkServer.active)
        {
            Debug.Log("Disconnected :" + netMsg.conn.connectionId);
            GameManager.Instance.gameUI.SetExplainText("断开连接，正在尝试重连");
            string disconnectedUid = playerManager.getPlayerUidByConnID(netMsg.conn.connectionId);
            if (null != disconnectedUid)
            {
                
                // 管理连接列表
                // listManager.displayConnectionState(disconnectedUid, false);
                
            }

            playerManager.setPlayerOffline(netMsg.conn.connectionId);
        }
        else
        {
            GameManager.Instance.gameUI.SetExplainText("服务器掉线，请退出游戏");
        }
    }

    /// <summary>
    /// Called when UidMessage has received.
    /// </summary>
    /// <param name="netMsg">A network message object</param>
    public void OnUID(NetworkMessage netMsg)
    {
        UidMessage msg = netMsg.ReadMessage<UidMessage>();
        Debug.Log("OnUID " + msg.uid);
        targetUid = msg.uid;

        // If UID already exsist, do not add new player
        if (playerManager.isExsistUID(msg.uid))
        {
            // Assign new connection id for re-connected client.
            playerManager.setPlayerConnId(netMsg.conn.connectionId, msg.uid);
            // string connectedUid = playerManager.getPlayerUidByConnID(netMsg.conn.connectionId);
            // listManager.displayConnectionState(connectedUid, true);              管理连接列表
        }

        else
        {
            // New connection
            playerManager.addPlayer(netMsg.conn.connectionId, msg.uid); //
            // 管理连接列表
            // listManager.addItem(msg.uid);
        }

        // 仅接受一个客户端请求，关闭服务器广播
        if (discovery != null && discovery.running)
        {

            discovery.StopBroadcast();
        }

    }

    /// <summary>
    /// Called when custom network message has received.
    /// </summary>
    /// <param name="netMsg">A network message object</param>
    public void OnCustomMessage(NetworkMessage netMsg)
    {
        CustomMessage msg = netMsg.ReadMessage<CustomMessage>();

        
        if (NetworkServer.active)
        {
            string uid = playerManager.getPlayerUidByConnID(netMsg.conn.connectionId);
            uid = uid.Substring(0, 10);
            // receivedText.text += msg.text = "[" + uid + "...]:" + msg.text + "\n";

            // NetworkServer.SendToAll(MyMsgType.Custom, msg);
        }
        else
        {
            // receivedText.text += msg.text;
        }
        Debug.Log("OnCustomMessage : " + msg.text);
        ReceiveEvent(msg.text);
    }

    /// <summary>
    /// Check if client is currently connected.
    /// </summary>
    /// <returns>returns true if client is connected.</returns>
    public bool isConnected()
    {
        return myClient.isConnected;
    }

    /// <summary>
    /// Send unique device identifier to server to identify this client.
    /// </summary>
    public void sendUid()
    {
        UidMessage msg = new UidMessage();
        msg.uid = uid;

        myClient.Send(MyMsgType.UID, msg);
    }


    /// <summary>
    /// Send raw network message to client
    /// </summary>
    /// <param name="uid">A unique device identifier.</param>
    public void sendMessageToClient(string info)
    {
        CustomMessage msg = new CustomMessage();
        int connID = -1;
        msg.text = info;

        PlayerManager.playerData Data = playerManager.getPlayerByUid(targetUid);
        connID = Data.ConnID;

        NetworkServer.SendToClient(connID, MyMsgType.Custom, msg);
    }

    /// <summary>
    /// Send raw network message to server
    /// </summary>
    /// <param name="text">A message text will sended.</param>
    public void sendMessageToServer(string info)
    {
        CustomMessage msg = new CustomMessage();
        msg.text = info;
        myClient.Send(MyMsgType.Custom, msg);
    }

    /// <summary>
    /// Start as server and if discovery not running, start broadcast.
    /// </summary>
    public void SetupServer()
    {
        if (!NetworkServer.active)
        {
            Debug.Log("SetupServer( )");
            ConnectionConfig config = new ConnectionConfig();
            config.AddChannel(QosType.ReliableSequenced);
            config.AddChannel(QosType.Unreliable);
            NetworkServer.Configure(config, 1000);
            NetworkServer.dontListen = false;

            NetworkServer.Listen(4444);
            // 注册回调绑定
            NetworkServer.RegisterHandler(MsgType.Connect, OnConnected);
            NetworkServer.RegisterHandler(MsgType.Disconnect, OnDisconnected);
            NetworkServer.RegisterHandler(MyMsgType.UID, OnUID);
            NetworkServer.RegisterHandler(MyMsgType.Custom, OnCustomMessage);
        }

        if (!discovery.running)
        {
            discovery.Initialize();
            discovery.StartAsServer();
        }
    }

    /// <summary>
    /// Start as client and if discovery not running, start broadcast receive mode.
    /// </summary>
    public void SetupClient()
    {
        Debug.Log("SetupClient()");

        ConnectionConfig config = new ConnectionConfig();
        config.AddChannel(QosType.ReliableSequenced);
        config.AddChannel(QosType.Unreliable);
        myClient.Configure(config, 1000);

        discovery.Initialize();
        discovery.StartAsClient();

        // Register message event handler
        myClient.RegisterHandler(MsgType.Connect, OnConnected);
        myClient.RegisterHandler(MsgType.Disconnect, OnDisconnected);
        myClient.RegisterHandler(MyMsgType.Custom, OnCustomMessage);
    }


    /// <summary>
    /// 客户端连接服务器
    /// </summary>
    public void ClientConnectServer(string addr)
    {
        if (!isConnected())
        {
            Debug.Log("正在尝试连接服务器 ：  " + addr);
            ConnectToServer(addr);
        }
        else
        {
            Debug.Log("客户端已经与服务器建立连接，不必多次尝试连接");
        }
    }

    /// <summary>
    /// Connect to server with IP address.
    /// </summary>
    /// <param name="givenAddress">An IP address trying to connect</param>
    void ConnectToServer(string givenAddress)
    {
        if (null == address) { address = givenAddress; }
        myClient.Connect(givenAddress, 4444);
    }

    /// <summary>
    /// 关闭与服务器的连接
    /// </summary>
    public void QuitConnectToServer()
    {
        isConnecting = false;

         // 取消注册短消息回调
         myClient.UnregisterHandler(MsgType.Connect);
         myClient.UnregisterHandler(MsgType.Disconnect);
         myClient.UnregisterHandler(MyMsgType.Custom);
         
         myClient.Disconnect();
         myClient.Shutdown();
         
         Debug.Log("断开与服务器的连接....@@");
         ClearIPList();
         address = null;
         playerManager.ClearPlayerManagerData();
         myClient = new NetworkClient();

    }

    /// <summary>
    /// Initialize NetworkServer object and Stop listening on port.
    /// </summary>
    public void DisableServer()
    {
        Debug.Log("StopServer");
        isConnecting = false;

        if (NetworkServer.active)
        {

            // 自己加的
            NetworkServer.dontListen = true;



            NetworkServer.ClearHandlers();
            NetworkServer.Reset();
            NetworkServer.Shutdown();
        }

        if ( discovery != null && discovery.running)
        {
            discovery.StopBroadcast();
        }

        address = null;
        playerManager.ClearPlayerManagerData();
        myClient = new NetworkClient();
    }

}
