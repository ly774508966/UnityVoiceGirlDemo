using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using NAudio.Wave;
using UnityEngine;

public class iarPara
{
    public ManualResetEvent mre;//阻塞用的
    public int bytesTotal;//还需要的字节数
    public int bytesReaded;//已经读取的字节数
    public byte[] readBytes;
}
public class SocketClient
{
    private TcpClient m_tcpClient;
    private NetworkStream m_networkStream;
    private int index = 1;
    //连接
    public void Connect(string address,int port)
    {

        try
        {
            m_tcpClient = new TcpClient();
            m_tcpClient.BeginConnect(address, port, ConnectCallBack, m_tcpClient);//异步连接服务器
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    private void ConnectCallBack(IAsyncResult iar)
    {
        try
        {
            TcpClient client = (TcpClient) iar.AsyncState;
            client.EndConnect(iar);
            Debug.Log(string.Format("与服务器{0}连接成功", client.Client.RemoteEndPoint));
            m_networkStream = client.GetStream();//给ns赋值以后传流
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            throw;
        }
    }

    //关闭
    public  void Close()
    {
        if (m_tcpClient != null)
            m_tcpClient.Close();
        if(m_networkStream!=null)
            m_networkStream.Close();
    }

    //发送音频流
    public  void  SendStream(byte[] streamBytes)
    {
        m_networkStream.BeginWrite(streamBytes, 0, streamBytes.Length, SendCallBack, m_networkStream);
    }

    private void SendCallBack(IAsyncResult iar)
    {
        NetworkStream ns = (NetworkStream)iar.AsyncState;
         ns.EndWrite(iar);
    }
    //接收语音字符串
    public string   ReadString()
    {
        string response;
        try
        {
            byte[] sizeBytes = new byte[8];

            iarPara iar = new iarPara();
            iar.mre = new ManualResetEvent(false);
            iar.bytesTotal = 8;
            iar.bytesReaded = 0;
            iar.readBytes = sizeBytes;
         
            m_networkStream.BeginRead(iar.readBytes, 0, iar.bytesTotal-iar.bytesReaded, ReadCallBack, iar);
            iar.mre.WaitOne();//阻塞,等待完成

            int len = int.Parse(Encoding.ASCII.GetString(sizeBytes));
            byte[] responseBytes = new byte[len];
            iar.bytesTotal =len;
            iar.bytesReaded = 0;
            iar.readBytes = responseBytes;
            iar.mre.Reset();
            m_networkStream.BeginRead(iar.readBytes, 0, iar.bytesTotal - iar.bytesReaded, ReadCallBack, iar);
            iar.mre.WaitOne();
            response = Encoding.UTF8.GetString(responseBytes);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            throw e;
        }
        return response; //返回response
    }

    private void ReadCallBack(IAsyncResult iar)
    {
        iarPara i = (iarPara) iar.AsyncState;
        int recv= m_networkStream.EndRead(iar);
        if (recv == 0)
        {
            return;
        }
        i.bytesReaded += recv;
        Debug.Log("接收到"+recv+"个字节");
        if (i.bytesReaded == i.bytesTotal)
            m_networkStream.BeginRead(i.readBytes, 0, i.bytesTotal - i.bytesReaded, ReadCallBack, iar);
        i.mre.Set();
    }
    //读音频流
    public byte[] ReceiveBytes()
    {
        byte[] responseBytes;
        int len = 32044+1;
        try
        {
            // 读内容
            responseBytes = new byte[len];

            iarPara iar = new iarPara();
            iar.mre = new ManualResetEvent(false);
            iar.bytesTotal = len;
            iar.bytesReaded = 0;
            iar.readBytes = responseBytes;
            iar.mre.Reset();
            m_networkStream.BeginRead(iar.readBytes, 0, iar.bytesTotal - iar.bytesReaded, ReadCallBack, iar);
            iar.mre.WaitOne();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            throw e;
        }
        return responseBytes; //返回接收的音频流
    }

}
