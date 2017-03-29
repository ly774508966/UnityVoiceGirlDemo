using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;
using Academy.HoloToolkit.Unity;
using UnityEngine;
using System.Collections;
using System;
using System.Text;
using NAudio.Wave;

public class AudioCapture : Singleton<AudioCapture>
{
    //说话的女生
    public GameObject m_VocieGirl;

    //录音相关
    private IWaveIn m_iWaveIn;//录音类
    
    private const int m_c_sampleRate = 16000; //录音参数
    private const int m_c_bitDepth = 16;
    private const int m_c_channelCount = 1;

    //socket相关
    private SocketClient m_socketClient;

    private const  string m_c_serverAddress = "127.0.0.1";//常量
    private const int m_c_port = 5099;

    private const int m_c_bufferSize = 32001; //发送大小,每次发送1s就ok,再加1个字节的控制字符
    private const  int m_c_audioStreamSize = 32045;//接收大小,根据最后一个字节判断是否结束接收音频

    //控制音频的
    private bool m_isRecording; //是否正在录音,用来跳出发送流
    private bool m_stillPlayAudio;//结束进入update
    private List<AudioClip> m_lsAudioClips ;
    private int m_playAudioIndex ;
    private Queue<byte[]> m_queueBytes;//新线程音频流的存储位置,先进先出
    private byte[] m_audioBytes;//接收并转换成可以AudioClip.Create的Byte
    private List<AudioSource> m_listAudioSource;//因为要按照顺序播放好多音频,用这个而能够保证不会产生太大误差
    private double m_audioStartTime;

    //答案
    private string m_answer;

    //发送音频流
    private byte[] m_voiceBytes;//存储总的说话字节,每隔1s往服务器发送一次,最长60秒
    private int m_writeIndex;
    private int m_readIndex;
    private WaveFileWriter m_waveWriter;


    //需要分配变量的定义在start里
    public void Start()
    {
        m_listAudioSource = new List<AudioSource>();
        m_lsAudioClips=new List<AudioClip>();
        m_socketClient = new SocketClient();
        m_queueBytes = new Queue<byte[]>();
        m_audioBytes = new byte[m_c_audioStreamSize];
        m_socketClient.Connect(m_c_serverAddress, m_c_port);
        m_voiceBytes = new byte[32000 * 60];
        InitAudioPlaying();
    }

    //下一次要读的时候重新初始化相关的变量
    public void InitAudioPlaying()
    {
        m_lsAudioClips.Clear();
        m_listAudioSource.Clear();//clear应该比new快吧
        lock (m_queueBytes)
        {
            m_queueBytes.Clear();
        }
        Array.Clear(m_voiceBytes, 0, m_voiceBytes.Length);
        m_writeIndex = 0;
        m_isRecording = false;
        m_playAudioIndex = 0;
        m_stillPlayAudio = false;
        m_audioStartTime = 0;
        m_writeIndex = 0;
        m_readIndex = 0;

    }

    public void StartRecord()
    {
        try
        {
            m_VocieGirl.GetComponent<CharacterManager>().isSpeaking = false;
            if (m_stillPlayAudio)
            {
                m_lsAudioClips.Clear();
                m_queueBytes.Clear();
                foreach (AudioSource a in m_listAudioSource)
                {
                    Destroy(a);
                    //a.Stop();
                }
                       ;
            }
            InitAudioPlaying();
            m_stillPlayAudio = true;
            Thread waveInThread = new Thread(RecordingThread);
            waveInThread.Start();

        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            throw e;
        }
    }

    private void RecordingThread()
    {

        m_iWaveIn = new WaveInEvent();
        m_iWaveIn.WaveFormat = new WaveFormat(m_c_sampleRate, m_c_bitDepth, m_c_channelCount);

        string outputFilename = String.Format("NAudioDemo {0:yyy-MM-dd HH-mm-ss}.wav", DateTime.Now);
        m_waveWriter = new WaveFileWriter(Path.Combine("./", outputFilename), m_iWaveIn.WaveFormat);


        m_iWaveIn.StartRecording();
        m_iWaveIn.DataAvailable += CaptureOnDataAvailable;
        m_iWaveIn.RecordingStopped += OnRecordingStopped;
        m_socketClient.SendStream(Encoding.ASCII.GetBytes("start"));//发送开始标志,服务器分配变量内存
        m_socketClient.SendStream(Encoding.ASCII.GetBytes("send"));//发送传输标志
        m_isRecording = true;
        Thread sendBufferThread = new Thread(SendBuffer);
        sendBufferThread.Start();
    }
    private void CaptureOnDataAvailable(object sender, WaveInEventArgs waveInEventArgs)
    {
         Array.Copy(waveInEventArgs.Buffer,0,m_voiceBytes,m_writeIndex,waveInEventArgs.BytesRecorded);
         m_writeIndex += waveInEventArgs.BytesRecorded;
         m_waveWriter.Write(waveInEventArgs.Buffer, 0, waveInEventArgs.BytesRecorded);
    }

    void OnRecordingStopped(object sender, StoppedEventArgs e)
    {

        m_waveWriter.Dispose();
        m_waveWriter = null;
    }



   

    public void SendBuffer()
    {
        var bytes = new byte[m_c_bufferSize]; //前bufferSize字节存音频流,最后一个字节判断是否是最后一个字节流,以便break掉跳出.0表示正常发送,1表示最后一块音频流
        int dataSize = m_c_bufferSize - 1;
        while (m_isRecording)
        {
            while (m_writeIndex - m_readIndex >= dataSize)
            {
                Array.Clear(bytes, 0, dataSize); //重置缓冲区
                Debug.Log("发送音频流");
                Array.Copy(m_voiceBytes, m_readIndex, bytes, 0, dataSize);
                bytes[dataSize] = 0; //表示正常发送
                m_socketClient.SendStream(bytes); //发送音频流
                m_readIndex += dataSize;
            }
        }
        Debug.Log("发送最后一块音频流");
        Array.Clear(bytes, 0, 32000); //最后一块音频流除了最后一个字节,都为0
        Array.Copy(m_voiceBytes, m_readIndex, bytes, 0, m_writeIndex - m_readIndex);
        bytes[dataSize] = 1; //最后一块音频流
        m_socketClient.SendStream(bytes); //发送最后一块音频流
        m_socketClient.SendStream(Encoding.ASCII.GetBytes("stop")); //发送结束标志
        Thread getResultThread = new Thread(GetResult);
        getResultThread.Start();
    }

    public void GetResult()
    {
        string question = m_socketClient.ReadString();
        Debug.Log("question is " + question);
        FileStream fs = new FileStream("1.txt", FileMode.Append);
        StreamWriter sw=new StreamWriter(fs);
        sw.WriteLine("我听到的是-{0}",question);
        sw.Close();
        fs.Close();
        GetAnswer(question);
    }


    //获得小i返回的问答结果
    public void GetAnswer(string question)
    {
        Loom.QueueOnMainThread(
            () =>
            {
                StartCoroutine(talk(question));
            }
            );

    }

    public IEnumerator talk(string question)
    {
        string url = "http://cluster.xiaoi.com/robot/app/zhgszs/ask.action?platform=web&userId=test03&ver=101&format=xml&question=";
        WWW www = new WWW(url + question);
        yield return www;
        if (www.error != null)
            Debug.Log("机器人不回答我" + www.error);
        else
        {
            if (www.isDone)
            {
                ParsingXml(www.text);
            }
        }

    }

    private void ParsingXml(string _xml)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(_xml);
        XmlNodeList childNode = null;
        XmlNodeList root = xmlDoc.GetElementsByTagName("Response");//指定一个节点
        XmlNode rootn = root[0]["Content"];  //为什么要转语言编码
        m_answer = rootn.InnerText;
        Debug.Log("我的回复是" + m_answer);
       
        m_socketClient.SendStream(Encoding.ASCII.GetBytes("speak"));//测试语音
        m_socketClient.SendStream(Encoding.UTF8.GetBytes(m_answer));//发送答案过去让朗读
        //SpeakManager.Instance.SetAnswer(answer);//明明在main线程里,可能因为协程跑到外层子线程里了？
        PlayAudioStream();//接收答案音频流并朗读     
    }

    public void  PlayAudioStream()
    {
        Thread getBytesThread=new Thread(Task_GetBytes);
        getBytesThread.Start();
    }

    public void Task_GetBytes()
    {
        while (true)
        {

            m_audioBytes = m_socketClient.ReceiveBytes();//获取音频字节,这个是阻塞的,可以不用担心
            //可以接收
            lock (m_queueBytes)//防止另外的线程的读
            {
                m_queueBytes.Enqueue(m_audioBytes);//加入队列里
                Debug.Log("audioBytes size" + m_audioBytes.Length);

            }
            if (m_audioBytes[m_c_audioStreamSize - 1] == 1)//
            {
                break;//1的话就break出去
            }
        }
    }

    public  void StopRecord()
    {

        m_iWaveIn.StopRecording();
        m_isRecording = false;

    }


    public void Update()
    {
        if (m_stillPlayAudio) //播放完所有音频后为false,不再进入
        {
            lock (m_queueBytes)
            {
                foreach (AudioSource var in m_listAudioSource)
                {
                    m_VocieGirl.GetComponent<CharacterManager>().isSpeaking = false;
                    if (var.isPlaying)
                    {
                        m_VocieGirl.GetComponent<CharacterManager>().isSpeaking = true;
                        break;
                    }
                }

                if (m_queueBytes.Count >= 1)
                {
                
                    Byte[] bytes = m_queueBytes.Dequeue(); //取出bytes
                   

                    AudioClip audioClip = WavUtility.ToAudioClip(bytes, 0, "answerClip"+m_lsAudioClips.Count.ToString());
                    Debug.Log("m_lsAudioClips.Count"+ m_lsAudioClips.Count);

                    m_lsAudioClips.Add(audioClip);
                   
                }
            }
            if (m_playAudioIndex > m_lsAudioClips.Count)
            {
                m_stillPlayAudio = false; //音频已经播放结束
            }
            if (m_playAudioIndex < m_lsAudioClips.Count)
            {
               
                AudioSource audioSource = m_VocieGirl.AddComponent<AudioSource>();
                audioSource.clip = m_lsAudioClips[m_playAudioIndex];

                if (m_audioStartTime == 0)
                {
                    m_audioStartTime = AudioSettings.dspTime;
                }

                audioSource.PlayScheduled(m_audioStartTime + m_playAudioIndex);
                m_playAudioIndex++;
                m_listAudioSource.Add(audioSource);
               
            }
        }
    }

 


}
