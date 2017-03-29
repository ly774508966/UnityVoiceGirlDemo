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

        Debug.Log("新客户连接建立：" + ((IPEndPoint)m_client.RemoteEndPoint).Address);
        while ((bytes = ReceiveMessage(m_client, fixedsize)).Length != 0)
        {
            data = System.Text.Encoding.ASCII.GetString(bytes, 0, fixedsize);
            Debug.Log("收到的数据:" + data);
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
            case "start":
                Debug.Log("进入start case");
                Loom.QueueOnMainThread(
                    () =>
                    {
                        CharacterManager.Instance.GetComponent<CharacterManager>().isSpeaking = true;
                    });

                break;
            case "stop":
                Loom.QueueOnMainThread(
        () =>
        {
            CharacterManager.Instance.GetComponent<CharacterManager>().isSpeaking = false;
        });
                Debug.Log("stop case");
                break;
            case "max":
                Loom.QueueOnMainThread(
       () =>
       {
          DisplayScript.Instance.SetWindowMax();
       });
                break;
            case "min":
                Loom.QueueOnMainThread(
                () =>
                                {
                                    DisplayScript.Instance.SetWindowMin();
                                });
                break;

        }
    }

}

