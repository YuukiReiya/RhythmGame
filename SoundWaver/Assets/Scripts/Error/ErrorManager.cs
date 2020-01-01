using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yuuki.FileIO;
using System.IO;
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
#if true
        //  既存ログデータの削除
        #region Initializing log data
        string path = Application.persistentDataPath + Define.c_Delimiter + c_LogFileName + c_Extension;
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        #endregion
#endif
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
