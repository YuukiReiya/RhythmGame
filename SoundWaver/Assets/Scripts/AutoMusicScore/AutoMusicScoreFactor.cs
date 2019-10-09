//AudioSourceの終了検知
//http://waken.hatenablog.com/entry/2016/09/27/094953
//Pauseは使えないのでラッピングする必要がある
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Common;
using Yuuki.FileManager;
using Game;
using UnityEngine.SceneManagement;
public class AutoMusicScoreFactor : Yuuki.SingletonMonoBehaviour<AutoMusicScoreFactor>
{
    //serialize param
    [SerializeField] int sampleCount = 2048;
    [SerializeField] int channelNumber = 0;
    [SerializeField, Range(0, 100.0f),Tooltip("直前のフレームの最大スペクトラムと比較して、(この値 / 100) 大きければノーツを生成")]
    float difference = 0.3f;
    [Header("Chart info")]
    [SerializeField] UILabel musicTitle;
    [SerializeField] UILabel intervalSec;
    [SerializeField] UILabel chartName;
    [SerializeField] UILabel unitPerBeat;
    [SerializeField] UILabel unitPerBar;
    [SerializeField] UISprite playTimeValue;
    [Header("Buttons")]
    [SerializeField] UIButton cancel;
    [SerializeField] UIButton execute;
    [SerializeField] UIButton mute;
    //accessor
    public string title { get; set; }
    public uint bpm { get; set; }
    AudioSource audioSource { get { return Game.GameMusic.Instance.Source; } }
    //private param
    float prevMax = 0;
    bool isExecute = false;
    GameObject musicEngineObj;
    List<float> ret;
    string currentFilePath;//参照している楽曲のパス
    string executeFilePath;//譜面作成中の楽曲のパス

    //const param
    readonly string c_ChartSaveDirectory = Application.streamingAssetsPath+"\\Charts\\";

    // Start is called before the first frame update
    void Start ()
    {
        ret = new List<float>();
        mute.isEnabled = false;
        execute.isEnabled = false;
        cancel.isEnabled = false;
    }

    // Update is called once per frame
    void Update ()
    {
        //再生時間を可視化
        playTimeValue.fillAmount = audioSource && audioSource.clip.length > 0 ? audioSource.time / audioSource.clip.length : 0.0f;

        if (isExecute)
        {
            Execute();
            //終了タイミング
            if(audioSource.time==0.0f&&!audioSource.isPlaying)
            {
                Debug.Log("再生終了");
                isExecute = false;
                mute.isEnabled = false;
                execute.isEnabled = true;
                cancel.isEnabled = false;
                CreateChart();
            }
        }
    }
    
    void Execute ()
    {
        var spectrums = audioSource.GetSpectrumData(sampleCount, channelNumber, FFTWindow.Blackman);
        float max = spectrums.Max();

        if(Music.IsJustChanged)
        {
            if (max > prevMax + (difference / 100))
            {
                ret.Add(audioSource.time);
            }
        }
        Debug.Log(Music.IsJustChanged ? "in" : "not go execute");

        prevMax = max;
        //specLists.Add(spectrums.ToList<float>());
    }

    public void SetupMusic(string fileName)
    {
#if UNITY_ANDROID
        currentFilePath = FileManager.Instance.CurrentDirectory + "/" + fileName;
#else
        currentFilePath = FileManager.Instance.CurrentDirectory + "\\" + fileName;
#endif
        musicTitle.text = fileName;
        var ext = System.IO.Path.GetExtension(fileName);
        switch (ext)
        {
            case Define.c_MP3:
            //case Define.c_WAV://非対応？
                execute.isEnabled = true;
                break;
            default:
                execute.isEnabled = false;
                break;
        }
    }
    public void CreateChart()
    {
        var fileIO = new Yuuki.FileIO.FileIO();
        Chart chart = new Chart();
        chart.Title = musicTitle.text;//曲名
        chart.FilePath = executeFilePath;//楽曲パス
        chart.BPM = bpm;//BPM
        chart.timing = new float[ret.Count];
        chart.NotesInterval = float.Parse(intervalSec.text);
        chart.timing = ret.ToArray();
        fileIO.CreateFile(
            c_ChartSaveDirectory + chartName.text + Define.c_JSON,
            JsonUtility.ToJson(chart),
            true
            );
        Debug.Log("譜面データ作成:" + c_ChartSaveDirectory + chartName.text + Define.c_JSON);
    }

#region Button処理
        /// <summary>
        /// 音楽ファイル読み込み-再生
        /// </summary>
        public void Setup()
        {
        ErrorManager.Instance.text.text = "";
        ErrorManager.Instance.text.text += "tap\n";
        if (musicEngineObj != null) { Destroy(musicEngineObj); }
        ErrorManager.Instance.text.text += "path:" + currentFilePath;

        GameMusic.Instance.LoadAndFunction(
                currentFilePath,
                //読み込み終了後の処理
                () =>
                {
                    ErrorManager.Instance.text.text += "load end\n";
                    bpm = (uint)UniBpmAnalyzer.AnalyzeBpm(GameMusic.Instance.Clip);
                    //MusicEngine 生成
                    musicEngineObj = Instantiate(Resources.Load<GameObject>("Prefab/MusicEngine"));
                    ErrorManager.Instance.text.text = musicEngineObj ? "生成" : "インスタンスねーよ？";
                    var musicEngine = musicEngineObj.GetComponent<Music>();
                    //MusicEngineの動的設定
                    Music.CurrentSource.clip = GameMusic.Instance.Clip;
                    var sec = Music.GetSection(0);
                    sec.Tempo = bpm;
                    sec.UnitPerBar = int.Parse(unitPerBar.text);
                    sec.UnitPerBeat = int.Parse(unitPerBeat.text);
                    musicEngine.Setup();
                    //audio
                    GameMusic.Instance.Source = Music.CurrentSource;
                    GameMusic.Instance.Source.Play();
                    isExecute = true;
                    //buttons
                    mute.isEnabled = true;
                    execute.isEnabled = false;
                    cancel.isEnabled = true;
                    //param
                    executeFilePath = currentFilePath;
                }
           );
        }

        public void MuteButton()
        {
            audioSource.mute = !audioSource.mute;
        }

        public void CancelButton()
        {
            GameMusic.Instance.Source.clip = null;
            GameMusic.Instance.Source = null;
            isExecute = false;
            //buttons
            mute.isEnabled = false;
            execute.isEnabled = true;
        }
#endregion
}