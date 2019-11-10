using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yuuki.FileIO;
using Common;
public static class ErrorManager
{
    //serialize param
    //private param
    private static string contents;
    //public param
    //const param
    const string c_LogFileName = "Log";
    const string c_Extension = ".txt";

    public static void Setup()
    {
        Application.logMessageReceived += HandleLog;
    }

    private static void HandleLog(string condition, string stackTrace, LogType type)
    {
        contents+= condition + "\n" + stackTrace + "\n" + type.ToString() + "\n";
    }

    public static void Save()
    {
        FileIO fileIO = new FileIO();
        var content = System.DateTime.Now.ToString() + "\n" + SceneManager.GetActiveScene().name + "\n\n" + contents + "\n";
        fileIO.CreateFile(Application.persistentDataPath + Define.c_Delimiter + c_LogFileName + c_Extension, content, FileIO.FileIODesc.Append);
    }
}
