using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Academy.HoloToolkit.Unity;
using UnityEngine;

public class SocketServer : Singleton<SocketServer>
{

    private Socket m_server;
    private static int connectedCount;

    void Start ()
	{
	    IPAddress local = IPAddress.Parse("127.0.0.1");
        IPEndPoint iep=new IPEndPoint(local,5099);
        m_server=new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
        m_server.Bind(iep);
        m_server.Listen(5);
        Thread ServerThread=new Thread(ServerAccept);
        ServerThread.Start();
      

    }
	
	
	void Update () {
		
	}

    private void ServerAccept()
    {
        Log.Init();
        while (true)
        {
            Debug.Log("开始接收客户端信息");
            Log.WriteToLog("开始接收客户端信息");
            // 得到包含客户端信息的套接字
            Socket client = m_server.Accept();
            Debug.Log("来了来了");
            Log.WriteToLog("接收到第" + ++connectedCount + "个客户端");
            //创建消息服务线程对象
            ServerThread newclient = new ServerThread(client);

            //把ClientThread 类的ClientService方法委托给线程
            Thread newthread = new Thread(newclient.ClientService);
            // 启动消息服务线程
            newthread.Start(20);//接收的长度大小
        }
    }

    public void OnApplicationQuit()
    {
        Log.WriteToLog("服务器退出");
        m_server.Close();
    }

    public void CloseClient()
    {
        connectedCount--;
    }

}

