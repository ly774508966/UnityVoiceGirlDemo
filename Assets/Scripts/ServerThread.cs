using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class ServerThread
{


    private Socket m_client;
    public ServerThread(Socket client)
    {
        this.m_client = client;
    }

    public void ClientService(object param)
    {
        int fixedsize = (int)param;
        string data = null;
        byte[] bytes = null;

        while ((bytes = ReceiveMessage(m_client, fixedsize)).Length != 0)
        {
            data = System.Text.Encoding.ASCII.GetString(bytes, 0, fixedsize);
            Log.WriteToLog("收到的命令:" + data);
            // 处理客户端发来的消息，这里是转化为大写字母
            data=data.Trim( '\0');
            ExecuteOrder(data);
        }
        //关闭套接字
        m_client.Close();
        Debug.Log("客户关闭连接:" + ((IPEndPoint)m_client.RemoteEndPoint).Address);
    }


    byte[] ReceiveMessage(Socket s, int size)
    {
        size = 20;
        int offset = 0;
        int recv;
        int dataleft = size;
        byte[] msg = new byte[size];
        while (dataleft > 0)
        {
            recv = s.Receive(msg, offset, dataleft, 0);
            if (recv == 0)
            {
                break;
            }
            offset += recv;
            dataleft -= recv;
        }
        return msg;
    }

    public void ExecuteOrder(string order)
    {
        
        //Debug.Log(sb.ToString().Length);
        switch (order)
        {
            case "exit":
                Log.WriteToLog("客户端断开");
                Loom.QueueOnMainThread(
                    () =>
                    {
                        SocketServer.Instance.CloseClient();
                        m_client.Close();
                    });
              
                break;
            case "start":
                Log.WriteToLog("--------开始说话");
                Loom.QueueOnMainThread(
                    () =>
                    {
                        CharacterManager.Instance.GetComponent<CharacterManager>().isSpeaking = true;
                    });

                break;
            case "stop":
                Log.WriteToLog("--------结束说话");
                Loom.QueueOnMainThread(
        () =>
        {
            CharacterManager.Instance.GetComponent<CharacterManager>().isSpeaking = false;
        });
                break;
            case "max":
                Log.WriteToLog("--------最大化");

                Loom.QueueOnMainThread(
       () =>
       {
          DisplayScript.Instance.SetWindowMax();
       });
                break;
            case "min":
                Log.WriteToLog("--------最小化");
                Loom.QueueOnMainThread(
                () =>
                                {
                                    DisplayScript.Instance.SetWindowMin();
                                });
                break;

        }
    }

}

