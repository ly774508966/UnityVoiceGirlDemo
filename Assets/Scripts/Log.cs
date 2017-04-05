using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Log
{

    static FileStream fs;
    static StreamWriter sw;

    public static void Init()
    {
        fs = new FileStream("日志.txt", FileMode.Create);
        sw = new StreamWriter(fs);
    }

    public static void WriteToLog(string log)
    {
        sw.WriteLine(log);
        sw.Flush();
    }

    public static void DisConnect()
    {
        sw.Close();
        fs.Close();
    }


}

