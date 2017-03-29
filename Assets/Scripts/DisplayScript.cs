using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Academy.HoloToolkit.Unity;
using UnityEngine;

public class DisplayScript : Singleton<DisplayScript>
{
    [DllImport("user32.dll")]
    public static extern bool ShowWindow(System.IntPtr hwnd, int nCmdShow);
    [DllImport("user32.dll", EntryPoint = "GetForegroundWindow")]
    public static extern System.IntPtr GetActiveWindow();
    [DllImport("user32.dll", EntryPoint = "GetForegroundWindow")]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    void  Start()
    {
        FileStream fs = new FileStream("1.txt", FileMode.Append);
        StreamWriter sw = new StreamWriter(fs);
        sw.Write("displays connected" + Display.displays.Length);
        sw.Close();
        fs.Close();
        Debug.Log("displays connected: " + Display.displays.Length);
        // Display.displays[0] is the primary, default display and is always ON.
        // Check if additional displays are available and activate each.
        //if (Display.displays.Length == 1)
        //{
        //    Display.displays[0].Activate();
        //}

        //if (Display.displays.Length == 2)
        //{
        //    Display.displays[1].Activate();
        //}

        //SetWindowMin();
        //yield return new WaitForSeconds(1f);
        //SetWindowMax();
        //yield return new WaitForSeconds(1f);
        //SetWindowMin();
        //yield return new WaitForSeconds(1f);
        //SetWindowMax();
        //yield return new WaitForSeconds(1f);
        //SetWindowMin();
        //yield return new WaitForSeconds(1f);
        //SetWindowMax();
        //yield return new WaitForSeconds(1f);
    }


    void Update()
    {
    }

    public void SetWindowMin()
    {
        ShowWindow(User32API.GetCurrentWindowHandle(), 2);//最小化
    }

    public void SetWindowMax()
    {
        ShowWindow(User32API.GetCurrentWindowHandle(), 3);//最小化
    }

}

