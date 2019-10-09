using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ErrorManager : Yuuki.SingletonMonoBehaviour<ErrorManager>
{
    public Text text;
    // Start is called before the first frame update
    void Start()
    {
        Application.logMessageReceived += HandleLog;
    }

    // Update is called once per frame
    void Update()
    {
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
}
