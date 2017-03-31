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
        
    }


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
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

