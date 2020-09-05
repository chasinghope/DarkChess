using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Utils 
{
    /// <summary>
    /// 获取本机ip地址
    /// </summary>
    /// <returns></returns>
    public static string GetAddressIP()
    {
        ///获取本地的IP地址
        string AddressIP = string.Empty;
        foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        {
            if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
            {
                AddressIP = _IPAddress.ToString();
            }
        }
        return AddressIP;
    }

    static Socket sock;
    static Thread broadcastSender;


    public static void OpenBroadcastSender()
    {
        sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);//初始化一个Scoket实习,采用UDP传输



        broadcastSender = new Thread(BroadcastLoopSend);
        if (!broadcastSender.IsAlive)
        {
            broadcastSender.Start();
            Debug.Log("开启线程");
        }



    }


    public static void BroadcastLoopSend()
    {
        IPEndPoint iep = new IPEndPoint(IPAddress.Broadcast, 9095);//初始化一个发送广播和指定端口的网络端口实例
        sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);//设置该scoket实例的发送形式
        string request = "chasing chess!";//初始化需要发送而的发送数据

        byte[] buffer = Encoding.Unicode.GetBytes(request);          // string -> byte[]
        do
        {
            Debug.Log("正在发送数据");
            sock.SendTo(buffer, iep);
            Thread.Sleep(500);
        }
        while (true);

    }


    public static void CloseBroadcastSender()
    {
        broadcastSender.Abort();
        sock.Close();
    }



    /// <summary>
    /// int数组转byte数组
    /// </summary>
    /// <param name="src"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    public static byte[] IntToBytes(int[] src, int offset)
    {
        byte[] values = new byte[src.Length * 4];
        for (int i = 0; i < src.Length; i++)
        {
            values[offset] = (byte)src[i];
            values[offset + 1] = (byte)(src[i] >> 8);
            values[offset + 2] = (byte)(src[i] >> 16);
            values[offset + 3] = (byte)(src[i] >> 24);
            offset += 4;
        }
        return values;
    }


    /// <summary>
    /// int数组转byte数组
    /// </summary>
    /// <param name="src"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    public static int[] BytesToInt(byte[] src, int offset)
    {
        int[] values = new int[src.Length / 4];
        for (int i = 0; i < src.Length / 4; i++)
        {
            int value = (int)(src[offset] & 0xFF)
                | (int)(src[offset + 1] & 0xFF << 8)
                | (int)(src[offset + 2] & 0xFF << 16)
                | (int)(src[offset + 3] & 0xFF << 24);

            values[i] = value;
            offset += 4;
        }
        return values;
    }
}
