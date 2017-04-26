using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Academy.HoloToolkit.Unity;
using TcpServer;
using UnityEngine;
public class SocketServer : Singleton<SocketServer>
{
    private TcpHelper helper;
    void Start()
    {
        helper = new TcpHelper();
        Log.Init();
        helper.Run(5099);
    }

    void OnDestroy()
    {
        Debug.Log("关闭窗口");
        helper.Close();
        Log.DisConnect();
    }

   

}

