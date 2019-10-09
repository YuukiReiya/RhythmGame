using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Test : MonoBehaviour
{
    [Header("Label")]
    [SerializeField] UILabel path;
    [SerializeField] UILabel exist;
    [SerializeField] UILabel error;

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
        error.text += condition + "\n" + stackTrace + "\n" + type.ToString() + "\n";
    }

    public void CheckPath()
    {
        exist.text = File.Exists(path.text) ? "true" : "false";
    }

    public void SetDefaultPath()
    {
        path.text = Application.persistentDataPath;
    }

    public void Play()
    {
        CheckPath();
        var audio = GetComponent<AudioSource>();
        Game.GameMusic.Instance.Source = audio;
        Game.GameMusic.Instance.LoadAndPlayAudioClip(path.text);
    }
}
