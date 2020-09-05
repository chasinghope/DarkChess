using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using UnityEngine.EventSystems;
using System.Text;

public class GameServer
{

    // ip + 端口
    string address;
    int port;
   
    // 当前服务器监听子线程
    Thread connectThread;

    // 远程客户端
    TcpClient romoteClient;

    #region 广播部分

    Socket socket_broadcast;
    Thread thread_broadcast;
    IPEndPoint ipe_broadcast;
    int port_broadcast = 9095;

    #endregion

    public GameServer(string address, int port)
    {
        this.address = address;
        this.port = port;


        // 初始化广播设置
        // 
        //初始化一个Scoket实习,采用UDP传输
        socket_broadcast = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        //初始化一个发送广播和指定端口的网络端口实例
        ipe_broadcast = new IPEndPoint(IPAddress.Broadcast, port_broadcast);
        //设置该scoket实例的发送形式
        socket_broadcast.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
        thread_broadcast = new Thread(Open_BroadCastSenderCor);



    }



    /// <summary>
    /// 实例化服务器端套接字对象
    /// </summary>
    void InitServerSocket()
    {
        // 缓冲区大小
        int bufferSize = 8192;

        IPAddress ip = IPAddress.Parse(address);
        TcpListener tcpListener = new TcpListener(ip, port);

        // 监听对象  开始监听
        tcpListener.Start();
        Debug.Log("服务器启动");

        // 如果有远程客户端连接，此时就会得到一个对象用于通讯，两边连通
        romoteClient = tcpListener.AcceptTcpClient();
        Debug.Log("客户端连接成功");

        NetworkStream stream = romoteClient.GetStream();
        // 循环接收信息输入
        do
        {
            try
            {
                // 获取与客户端连接的数据
                byte[] buffer = new byte[bufferSize];
                // 读取信息
                int byteRead = stream.Read(buffer, 0, bufferSize);

                // 客户端断开连接
                if(byteRead == 0)
                {
                    Debug.Log("客户端断开");
                    break;
                }

                // 具体处理接受信息的数据
                string msg = Encoding.UTF8.GetString(buffer, 0, byteRead);
                Debug.Log(msg);

            }
            catch (System.Exception ex)
            {
                Debug.Log("客户端异常: " + ex.Message);
                // 客户端出现异常或者断开的时候，关闭线程防止溢出
                tcpListener.Stop();

            }
        } while (true);

    }


    /// <summary>
    /// 每个半秒发送一个内容为  “chasing chess!”的广播包
    /// </summary>
    public void Open_BroadCastSender()
    {

        if (!thread_broadcast.IsAlive)
        {
            Debug.Log("开启了一个线程");
            thread_broadcast.Start();
        }
        else
        {
            Debug.Log("开启线程失败");
        }


    }

    void Open_BroadCastSenderCor()
    {
        string request = "chasing chess!";//初始化需要发送而的发送数据

        byte[] buffer = Encoding.Unicode.GetBytes(request);

        do
        {
            socket_broadcast.SendTo(buffer, ipe_broadcast);
            Thread.Sleep(1000);
         } while (true);
    }


    /// <summary>
    /// 关闭广播
    /// </summary>
    public void Close_BroadCastSender()
    {
        if (thread_broadcast.IsAlive)
        {
            thread_broadcast.Abort();
        }
        socket_broadcast.Close();
    }









    /// <summary>
    /// 启动服务器
    /// </summary>
    public void Start()
    {
        connectThread = new Thread(InitServerSocket);
        connectThread.Start();
    }

    /// <summary>
    /// 关闭服务器
    /// </summary>
    public void Close()
    {
        // 如果有客户端连接服务器，那么需要关闭
        if(romoteClient != null)
        {
            romoteClient.Client.Shutdown(SocketShutdown.Both);
        }
        if(connectThread != null)
        {
            connectThread.Abort();
        }

    }

    /// <summary>
    /// 发送信息
    /// </summary>
    public void SendMsg()
    {
        if( romoteClient != null)
        {
            romoteClient.Client.Send(Encoding.UTF8.GetBytes("Hello Client"));
        }          
    }



}
