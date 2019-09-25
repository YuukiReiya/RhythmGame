//#define WWW
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
#if WWW
#else
using UnityEngine.Networking;
#endif
public class Test2 : MonoBehaviour
{
    public AudioSource source;
    public UILabel label;
    public TMPro.TextMeshProUGUI uGUI;
    IEnumerator routine = null;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Execute()
    {
        string file = label.text;
        if(!File.Exists(file))
        {
            uGUI.text = "file not exist";
            return;
        }
        if (routine != null) {
            StopCoroutine(routine);
            routine = null;
        }
        routine = LoadAudio(file);
        StartCoroutine(routine);
    }

    IEnumerator LoadAudio(string path)
    {
#if WWW
        WWW request = new WWW(path);
        while (!request.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
        uGUI.text += "WWW success\n";
        AudioClip clip = request.GetAudioClip(false, false, AudioType.MPEG);
        while (clip.loadState == AudioDataLoadState.Loading)
        {
            yield return new WaitForEndOfFrame();
        }
        uGUI.text += "Clip load success\n";
        if (clip.loadState != AudioDataLoadState.Loaded) {
            uGUI.text += "Clip load state = " + clip.loadState + "\n";
            yield break;
        }
        if (clip == null) {
            uGUI.text += "Clip == null\n";
            yield break;
        }
        uGUI.text += "Clip ok\n";
        source.clip = clip;
        source.volume = 1.0f;
        source.Play();
        uGUI.text += "music start";
#else
        uGUI.text = "";
        using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(path,AudioType.MPEG))
        {
            uGUI.text = "send to reqest\n";
            //  リクエスト送信
            yield return request.SendWebRequest();

            while (!request.isDone)
            {
                uGUI.text = "loading now\n";
                yield return null;
            }
            if(request.isNetworkError)
            {
                uGUI.text += request.error;
                yield break;
            }
            AudioClip clip = ((DownloadHandlerAudioClip)request.downloadHandler).audioClip;
            if (clip.loadState != AudioDataLoadState.Loaded)
            {
                uGUI.text += "load state = " + clip.loadState;
                yield break;
            }

            source.clip = clip;
            source.Play();
            uGUI.text += "success";
        }
#endif
    }

    public void Setup()
    {
        uGUI.text = "";
        //label.text = "file://" + "/storage/emulated/0/Android/a.mp3";
        label.text = "file://" + Application.persistentDataPath + "a.mp3";
    }
}
