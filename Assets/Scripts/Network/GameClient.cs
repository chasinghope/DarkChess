using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using System.Text;
using System;

/// <summary>
/// 客户端
/// </summary>
public class GameClient
{
    // 服务器地址
    string serverAddress;
    // 服务器端口
    int port;
    // 当前tcp客户端
    TcpClient localClient;
    // 接受服务器消息的线程
    Thread receiveThread;

    public GameClient(string serverAddress, int port)
    {
        this.serverAddress = serverAddress;
        this.port = port;
    }

    /// <summary>
    /// 开启客户端
    /// </summary>
    /// <returns></returns>
    public bool Start()
    {
        localClient = new TcpClient();
        try
        {
            IPAddress ip = IPAddress.Parse(serverAddress);
            localClient.Connect(ip ,port);

            receiveThread = new Thread(SocketReceiver);
            receiveThread.Start();
            Debug.Log("客户端启动");
        }
        catch (System.Exception ex)
        {
            Debug.Log("客户端连接服务器异常: " + ex.Message);
            if( !localClient.Connected)
            {
                return false;
            }
            throw;
        }
        return true;
    }


    /// <summary>
    /// 关闭客户端
    /// </summary>
    public void Close()
    {
        if(localClient != null)
        {
            localClient.Close();
        }
        if(receiveThread != null)
        {
            receiveThread.Abort();
        }
    }


    private const int listenPort = 9095;

    public void StartListener()
    {
        UdpClient listener = new UdpClient(listenPort);
        IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);

        try
        {
            while (true)
            {
                Debug.Log("Waiting for broadcast");
                byte[] bytes = listener.Receive(ref groupEP);

                Debug.Log($"Received broadcast from {groupEP} :");
                Debug.Log($" {Encoding.ASCII.GetString(bytes, 0, bytes.Length)}");
            }
        }
        catch (SocketException e)
        {
            Debug.Log(e);
        }
        finally
        {
            listener.Close();
        }
    }



    /// <summary>
    /// 客户端接受服务器信息
    /// </summary>
    void SocketReceiver()
    {
        if(localClient != null)
        {
            int bufferSize = 8192;
            byte[] resultBuffer = new byte[bufferSize];

            do
            {
                try
                {
                    int byteRead = localClient.Client.Receive(resultBuffer);
                    if(byteRead == 0)
                    {
                        Debug.Log("与服务器连接中断");
                        break;
                    }
                    // 处理接收到的数据
                    string msg = Encoding.UTF8.GetString(resultBuffer, 0, byteRead);
                    Debug.Log(msg);
                }
                catch (System.Exception ex)
                {
                    Debug.Log("与服务器接受数据  " + ex.Message);

                }
            } while (true);

        }
    }


    /// <summary>
    /// 发送信息
    /// </summary>
    void SendMsg()
    {
        if (localClient != null)
        {
            localClient.Client.Send(Encoding.UTF8.GetBytes("Hello Server"));
        }
    }

}
