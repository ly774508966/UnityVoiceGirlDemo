using System.Collections;
using System.Collections.Generic;
using Academy.HoloToolkit.Unity;
using UnityEngine;
using UnityEngine.UI;

public class SpeakManager : Singleton<SpeakManager>
{
    private int m_tapIndex = 0;
    public Image micImage;//说话时进行提示的一张单独图片
    public Image BGImage;//隐藏开始录音前的提示文字
    public Image BGImage1;//作为text
    public Text ReplyText;
    public Text buttonText;

    private float speakTime;

    private float wordIntervalTime = 0.2387878787878f;//每个字的间
    private int speakWordIndex = 0;
    private AudioSource m_audioSource;

    public string m_answer;

    public void Awake()
    {
        m_audioSource = GetComponent<AudioSource>();
    }
    public void OnSpeakingTapped()
    {
        if (m_tapIndex++ % 2 == 0)
        {
            //Debug.Log("StartRecord");
            //speakWordIndex = 0;
            //ReplyText.text = "";
            AudioCapture.Instance.SendMessage("StartRecord");
            buttonText.text = "结束";
            //HideAll();
            //micImage.gameObject.SetActive(true);
        }
        else
        {
            buttonText.text = "开始";
            //Debug.Log("StopRecord");
            //HideAll();
            //BGImage1.gameObject.SetActive(true);
            AudioCapture.Instance.SendMessage("StopRecord");
        }
    }

    private void HideAll()
    {
        BGImage.gameObject.SetActive(false);
        BGImage1.gameObject.SetActive(false);
        micImage.gameObject.SetActive(false);
    }
    public IEnumerator SetAnswer()
    {
        ReplyText.text += m_answer[speakWordIndex++];
        //if(answer[speakWordIndex-1]!=' ')
        //m_audioSource.Play();
        yield return new WaitForSeconds(wordIntervalTime);
        if(speakWordIndex < m_answer.Length)
        StartCoroutine("SetAnswer");
    }
}

