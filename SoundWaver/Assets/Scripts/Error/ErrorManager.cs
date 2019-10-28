using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ErrorManager : Yuuki.SingletonMonoBehaviour<ErrorManager>
{
    public Text text;
    public bool isActive = true;
    // Start is called before the first frame update
    void Start()
    {
        Application.logMessageReceived += HandleLog;
    }

    void HandleLog(string condition, string stackTrace, LogType type)
    {
        text.text += condition + "\n" + stackTrace + "\n" + type.ToString() + "\n";
    }

    [ContextMenu("Error")]
    void Error()
    {
        Debug.LogError("error");
    }

    private void OnDestroy()
    {
        Save();
    }

    public void Save()
    {
#if UNITY_EDITOR
        //エディタ上では空処理
        if (!isActive) { return; }
        Yuuki.FileIO.FileIO fileIO = new Yuuki.FileIO.FileIO();
        var content = System.DateTime.Now.ToString() + "\n" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + "\n\n" + text.text + "\n";
        fileIO.CreateFile(Application.persistentDataPath + "/ロガー出力.txt", content, Yuuki.FileIO.FileIO.FileIODesc.Append);
#elif UNITY_ANDROID
        Yuuki.FileIO.FileIO fileIO = new Yuuki.FileIO.FileIO();
        var content = System.DateTime.Now.ToString() + "\n" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + "\n\n" + text.text + "\n";
        fileIO.CreateFile(Application.persistentDataPath + "/ロガー出力.txt", content, Yuuki.FileIO.FileIO.FileIODesc.Append);
#endif
    }
}
