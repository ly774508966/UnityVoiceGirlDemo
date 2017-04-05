using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandDeal 
{

    public static void ExecuteCommand(string order)
    {
        switch (order)
        {
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

