//#undef UNITY_EDITOR
//#define UNITY_ANDROID
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class WWWLoader : Yuuki.SingletonMonoBehaviour<WWWLoader>
{
    public AudioSource source;
    public string path;
    //public TMPro.TextMeshProUGUI pro;
    public UnityEngine.UI.Text pro;
#if UNITY_EDITOR
    public static string StreamingAssetsPath { get { return Application.dataPath+ "/StreamingAssets"; } }
#elif UNITY_ANDROID
    // Android
    public static string StreamingAssetsPath { get { return "jar:file://" + Application.persistentDataPath + "!/assets"; } }
    //public static string StreamingAssetsPath { get { return "jar:file://" + "/storage/emulated/0/Android/"; } }
#elif UNITY_IPHONE
    // iOS
    public static string StreamingAssetsPath { get {return path= Application.dataPath + "/Raw"; } }
#else
    // ReleaseBuild
    public static readonly string StreamingAssetsPath { get { return Application.dataPath + "/StreamingAssets"; } }
#endif

    // Start is called before the first frame update
    void Start()
    {

        //Func();
        //StartCoroutine(LoadAudio(path));
        //path = Application.persistentDataPath + "../../../../short_song_kei_harujion.mp3";
        //path = Application.persistentDataPath + "../../../../a.txt";
        //path = Application.persistentDataPath + "/a.txt";
        path = Application.persistentDataPath + "/short_song_kei_harujion.wav";
        //StartCoroutine(LoadAudio(path));
    }

    public void GetDirs()
    {
        //var dirs = Directory.GetDirectories(Application.persistentDataPath+"../../../../");

        //pro.text = "";
        //foreach (var it in dirs)
        //{
        //    pro.text += it + "\n";
        //}

        StartCoroutine(LoadAudio(path));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator LoadAudio(string path)
    {
        string loadPath = string.Empty;

        //loadPath = StreamingAssetsPath + path;
        loadPath = path;
        //if (!TryGetLoadPath(path, out loadPath)) { yield break; }

        //pro.text = "読み込み :" + System.IO.File.Exists(loadPath) + "\nPath:" + loadPath + "\n\n" + "jar:file://" + Application.persistentDataPath + "!/assets";

        pro.text = File.Exists(loadPath) + "\n" + path;
        WWW request = new WWW(loadPath);
        while (!request.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
        pro.text += "1,";
        AudioClip clip = request.GetAudioClip(false, false);
        pro.text += "2,";
        while (clip.loadState == AudioDataLoadState.Loading)
        {
            yield return new WaitForEndOfFrame();
        }
        pro.text += "3,";
        //失敗
        if (clip.loadState == AudioDataLoadState.Failed) { yield break; }
        pro.text += "4,";

        if (clip == null) { yield break; }

        source.clip = clip;
        source.Play();
        pro.text += "5,";
        //pro.text = "" + loadPath;

    }
    [ContextMenu("first move")]
    void Func()
    {
#if false
        //string loadPath = string.Empty;
        //if (!TryGetLoadPath("myText.txt", out loadPath)) { return; }

        // string loadPath = Application.persistentDataPath + "/" + "音ゲーのtest.txt";
        string loadPath = StreamingAssetsPath + "/" + "My音ゲー.txt";

        //ファイル確認
        System.IO.FileInfo info = new System.IO.FileInfo(loadPath);
        if(info.Exists)
        {
            return;
            info.Delete();
        }

        //Debug.Log("path = " + loadPath);
        pro.text = loadPath;

        //ディレクトリ
        string directryName = System.IO.Path.GetDirectoryName(loadPath);
        if (!System.IO.Directory.Exists(directryName))
        {
            Debug.Log("Dir = " + directryName);
            System.IO.Directory.CreateDirectory(directryName);
        }
        
        System.IO.StreamWriter sw;
        sw = info.AppendText();
        sw.WriteLine("testですよーーーーー");
        sw.Flush();
        sw.Close();
#endif
        string loadPath = Application.persistentDataPath + "/" + "Music/music_test.txt";

        System.IO.FileInfo fileInfo = new System.IO.FileInfo(loadPath);


        //ファイルが存在するか確認
        if (fileInfo.Exists)
        {
            //既にファイルが存在している場合、
            //上書きするか、追加するか
            {
                //上書きなら削除
            //    fileInfo.Delete();
            }
        }

        //必要に応じてディレクトリを生成する
        {
            string dirName = System.IO.Path.GetDirectoryName(loadPath);
            if(!System.IO.Directory.Exists(dirName))
            {
                System.IO.Directory.CreateDirectory(dirName);
            }
        }

        // 保存先 persistentDataPath
        // [win] : C:/Users/HomePC/AppData/LocalLow/DefaultCompany/プロジェクト名/save_data.json
        // [Android] : /data/app/xxxx.apk
        // [ios] : /var/mobile/Applications/xxxxxx/myappname.app/Data


        var combinedPath = System.IO.Path.Combine(Application.persistentDataPath, "hoge.txt");
    }

    public void Create()
    {
        string loadPath = Application.persistentDataPath + "/" + "data.txt";
        FileInfo fi = new FileInfo(loadPath);

        string dirName = Path.GetDirectoryName(loadPath);
        if(!Directory.Exists(dirName))
        {
            Directory.CreateDirectory(dirName);
        }
        Debug.Log("Create");
        StreamWriter sw;
        sw = fi.AppendText();
        sw.WriteLine("test");
        sw.Flush();
        sw.Close();
    }

    private bool TryGetLoadPath(string filePath,out string streamingPath)
    {
        string loadPath = Application.persistentDataPath + "/" + filePath;

        //初回起動時
        if(!System.IO.File.Exists(loadPath))
        {
            streamingPath = StreamingAssetsPath + filePath;
        }
        //保存先にデータがある場合(2回目以降)
        else
        {
            streamingPath = loadPath;
        }
        //パスの存在有無
        if (!System.IO.File.Exists(streamingPath)) {
            streamingPath = string.Empty;
            return false;
        }
        return true;
    }
}
