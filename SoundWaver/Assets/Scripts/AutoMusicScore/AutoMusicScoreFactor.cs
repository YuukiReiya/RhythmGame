//AudioSourceの終了検知
//http://waken.hatenablog.com/entry/2016/09/27/094953
//Pauseは使えないのでラッピングする必要がある
using Common;
using Game;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Yuuki.FileManager;
using Game.UI;
public class AutoMusicScoreFactor : Yuuki.SingletonMonoBehaviour<AutoMusicScoreFactor>
{
    //serialize param
    [SerializeField] int sampleCount = 2048;
    [SerializeField] int channelNumber = 0;
    [System.Serializable]
    struct ChartImage
    {
        public GameObject active;
        public GameObject disable;
        public UITexture image;
    }
    //[SerializeField]
    [Header("Chart Info")]
    [SerializeField] RadioButton autoMatchBPM;
    [SerializeField] UILabel musicTitle;
    [SerializeField] UILabel bpmLabel;
    [SerializeField] UILabel intervalLabel;
    [SerializeField] UILabel thresholdLabel;
    [SerializeField] UILabel registNameLabel;
    [SerializeField] UILabel unitPerBeatLabel;
    [SerializeField] UILabel unitPerBarLabel;
    [SerializeField] UITexture playTimeFill;
    [SerializeField] UILabel currentPlayTimeLabel;
    [SerializeField] UILabel musicFileTimeLabel;
    [SerializeField] ChartImage imageParam;
    [Header("Buttons")]
    [SerializeField] UIButton cancel;
    [SerializeField] UIButton execute;
    //[SerializeField] UIButton mute;
    [Header("Path")]
    [SerializeField] UILabel musicPath;
    [SerializeField] UILabel imagePath;

    //accessor
    //public string title { get; set; }
    public uint BPM { get; set; }
    AudioSource audioSource { get { return GameMusic.Instance.Source; } }
    //private param
    private float elapsedTime = 0;
    private float interval { get; set; }
    private float prevMax = 0;
    private float difference = 0.3f;
    private bool isExecute = false;
    private GameObject musicEngineObj;
    private List<Chart.Note> ret;
    private string currentFilePath;//参照している楽曲のパス
    private string executeFilePath;//譜面作成中の楽曲のパス
    //const param
    const string c_NotEnteredResistName = "表示する名前を入力してください";//譜面名の未入力時文字列
    const string c_NotEnteredMusicPath = "楽曲のパスを入力してください";//楽曲パスの未入力時文字列
    const string c_NotEnteredImagePath = "画像のパスを入力してください";//画像パスの未入力時文字列
    const uint c_InitialBarValue = 16;
    const uint c_InitialBeatValue = 4;
    const float c_InitialIntervalValue = 0.001f;
    const float c_InitialThreshold = 0.01f;
    // Start is called before the first frame update
    void Start()
    {
        ret = new List<Chart.Note>();
        execute.isEnabled = false;
        cancel.isEnabled = false;
        ResetOptionsValue();
    }

    private void FixedUpdate()
    {
        //再生時間を可視化
        playTimeFill.fillAmount = audioSource && audioSource.clip.length > 0 ? audioSource.time / audioSource.clip.length : 0.0f;

        if (isExecute)
        {
            Execute();
            //終了タイミング
            if (audioSource.time == 0.0f && !audioSource.isPlaying)
            {
                isExecute = false;
                execute.isEnabled = true;
                cancel.isEnabled = false;
                CreateData();
                ProcessEnd();
            }
        }
    }

    void Execute()
    {
        //再生時間更新
        var currentTime = (uint)audioSource.time;
        currentPlayTimeLabel.text = (currentTime / 60) + ":" + string.Format("{0:D2}", (currentTime % 60));
        var spectrums = audioSource.GetSpectrumData(sampleCount, channelNumber, FFTWindow.Blackman);
        float max = spectrums.Max();

        if (Music.IsJustChanged)
        {
            var v = max - prevMax;
            var diff = Mathf.Abs(v);

            elapsedTime += Time.deltaTime;//経過時間加算
            if (elapsedTime >= interval)
            {
                elapsedTime = 0;
#if UNITY_EDITOR
                //デバッグ
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Debug.Log(
                        "threshold:" + difference + "\n" +
                        "bar:" + int.Parse(unitPerBarLabel.text) + "\n" +
                        "beat:" + int.Parse(unitPerBeatLabel.text) + "\n" +
                        "interval:" + float.Parse(intervalLabel.text) + "\n" +
                        "BPM:" + BPM + "\n" +
                        "prev:" + prevMax + "\n" +
                        "now:" + max + "\n" +
                        "diff:" + v);
                }
#endif
                if (diff >= (difference))
                {
                    uint lane = 1;
                    //真ん中
                    if (diff < (difference * 2 / 3)) { lane = 1; }
                    //左
                    else if (v < 0) { lane = 0; }
                    //右
                    else { lane = 2; }
                    Chart.Note note = new Chart.Note(audioSource.time, lane);
                    ret.Add(note);
                }
            }
        }
        prevMax = max;
        //specLists.Add(spectrums.ToList<float>());
    }

    /// <summary>
    /// 設定項目のセットアップ
    /// </summary>
    public void ResetOptionsValue()
    {
        autoMatchBPM.CallActive();
        bpmLabel.text = string.Empty;
        unitPerBarLabel.text = c_InitialBarValue.ToString();
        unitPerBeatLabel.text = c_InitialBeatValue.ToString();
        intervalLabel.text = c_InitialIntervalValue.ToString();
        thresholdLabel.text = c_InitialThreshold.ToString();
    }

    public void SetupMusic(string fileName)
    {
        fileName= FileManager.Instance.CurrentDirectory + Define.c_Delimiter + fileName;
        if (!File.Exists(fileName))
        {
            DialogController.Instance.Open("ファイルが見つかりません。");
            Debug.LogError("AutoMusicScoreFactor.cs line169 Not found music file.\n" +fileName);
            ErrorManager.Save();
            return;
        }
        currentFilePath = Define.c_LocalFilePath + fileName;
        musicTitle.text = Path.GetFileName(fileName);
        musicPath.text = currentFilePath;
        execute.isEnabled = true;
    }
    public void SetupImage(string fileName)
    {
        fileName = FileManager.Instance.CurrentDirectory + Define.c_Delimiter + fileName;
        //エラー判定
        if (!File.Exists(fileName))
        {
            DialogController.Instance.Open("ファイルが見つかりません。");
            Debug.LogError("AutoMusicScoreFactor.cs line185 Not found image file.\n" + fileName);
            return;
        }
        imagePath.text = Define.c_LocalFilePath + fileName;
        StartCoroutine(LoadImageRoutine(imagePath.text));
    }
    private IEnumerator LoadImageRoutine(string path)
    {
        using(var request =UnityWebRequestTexture.GetTexture(path))
        {
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError("AutoMusicScoreFactor.cs line231: UnityWebRequest Error\n" + request.error);
                ErrorManager.Save();
                DialogController.Instance.Open(
                    "画像の読み込みに失敗しました。",
                    () =>
                    {
                        imageParam.disable.SetActive(true);
                        imageParam.active.SetActive(false);
                        imagePath.text = string.Empty;
                    }
                    );
                yield break;
            }
            imageParam.disable.SetActive(false);
            imageParam.active.SetActive(true);
            imageParam.image.mainTexture = DownloadHandlerTexture.GetContent(request);
        }
    }
    private void ResetImage()
    {
        imageParam.active.SetActive(false);
        imageParam.disable.SetActive(true);
        imageParam.image.mainTexture = null;
        imagePath.text = c_NotEnteredImagePath;
    }
    private void ResetMusic()
    {
        currentFilePath = string.Empty;
        musicPath.text = c_NotEnteredMusicPath;
        musicTitle.text = string.Empty;
        musicFileTimeLabel.text = "00:00";
        execute.isEnabled = false;
    }
    private void ResetSettingData()
    {
        ResetImage();
        ResetMusic();
        registNameLabel.text = string.Empty;
    }

    /// <summary>
    /// パス指定による音楽ファイルの決定
    /// </summary>
    public void SubmitMusicFile()
    {
        //  ファイル判定
        if (!File.Exists(musicPath.text))
        {
            DialogController.Instance.Open("ファイルが存在しません。");
            ResetMusic();
            return;
        }
        //  拡張子判定
        if (Path.GetExtension(musicPath.text) != Define.c_MP3)
        {
            DialogController.Instance.Open("拡張子が対応していません。");
            ResetMusic();
            return;
        }
        currentFilePath = Define.c_LocalFilePath + musicPath.text;
        musicTitle.text = Path.GetFileName(currentFilePath);
        execute.isEnabled = true;
    }

    public void SubmitImageFile()
    {
        //  ファイル判定
        if (!File.Exists(imagePath.text))
        {
            DialogController.Instance.Open("ファイルが存在しません。");
            ResetImage();
            return;
        }
        //  拡張子判定
        if (Path.GetExtension(imagePath.text) != Define.c_PNG)
        {
            DialogController.Instance.Open("拡張子が対応していません。");
            ResetImage();
            return;
        }
        currentFilePath = Define.c_LocalFilePath + imagePath.text;
        StartCoroutine(LoadImageRoutine(currentFilePath));
    }
    public void CreateData()
    {
        var fileIO = new Yuuki.FileIO.FileIO();
        Chart chart = new Chart();
        chart.Title = musicTitle.text;//曲名
        chart.MusicFilePath = executeFilePath;//楽曲パス
        chart.Comb = (uint)ret.Count;
        chart.BPM = BPM;//BPM
        chart.ResistName = registNameLabel.text;//譜面の名前
        chart.ImageFilePath = imagePath.text;//画像パス
        chart.Notes = ret.ToArray();
        chart.Interval = float.Parse(intervalLabel.text);
        fileIO.CreateFile(
            Define.c_ChartSaveDirectory + Define.c_Delimiter + registNameLabel.text + Define.c_JSON,
            JsonUtility.ToJson(chart),
            Yuuki.FileIO.FileIO.FileIODesc.Overwrite
            );
    }

    private void ProcessEnd()
    {
        DialogController.Instance.Open(
            "譜面ファイルの生成を\n完了しました。",
            () =>
            {
                ResetSettingData();
                ResetOptionsValue();
            }
            );
    }

#region Button処理
    /// <summary>
    /// 音楽ファイル読み込み-再生
    /// TODO:既存譜面の判定が雑…
    /// </summary>
    public void ProcessStart()
    {
        if (musicEngineObj != null) { Destroy(musicEngineObj); }

        //譜面名が入力されていない場合
        if (registNameLabel.text == c_NotEnteredResistName)
        {
            DialogController.Instance.Open(
                "登録名を未入力にすることは\nできません。"
                );
            return;
        }

        //譜面名が被った場合
        var charts = Directory.GetFiles(Define.c_ChartSaveDirectory);
        if(charts.Where(i => Path.GetFileNameWithoutExtension(i) == registNameLabel.text).Count() > 0)
        {
            DialogController.Instance.Open(
                "譜面が既に存在しています\n上書きしますか？",
                () => { MakeChartProcess(); },
                null, null
                );
        }
        else
        {
            MakeChartProcess();
        }
    }

    private void MakeChartProcess()
    {
        //音楽ファイルの確認
        //※ファイルの有無はそのままのパス。
        //ただし、読み込み時は"file:"を付けローカルファイルの指定とする必要がある。

        GameMusic.Instance.LoadAndFunction(
        currentFilePath,
        //読み込み終了後の処理
        () =>
        {
            //BPM自動判定
            if (autoMatchBPM.IsActive)
            {
                BPM = (uint)UniBpmAnalyzer.AnalyzeBpm(GameMusic.Instance.Clip);
            }
            //BPM手動入力
            else
            {
                BPM = uint.Parse(bpmLabel.text);
            }
            //MusicEngine 生成
            //TODO:アセットバンドル化?
            musicEngineObj = Instantiate(Resources.Load<GameObject>("Prefab/MusicEngine"));
            var musicEngine = musicEngineObj.GetComponent<Music>();
            //MusicEngineの動的設定
            Music.CurrentSource.clip = GameMusic.Instance.Clip;
            var sec = Music.GetSection(0);
            sec.Tempo = BPM;
            sec.UnitPerBar = int.Parse(unitPerBarLabel.text);
            sec.UnitPerBeat = int.Parse(unitPerBeatLabel.text);
            musicEngine.Setup();
            //audio
            GameMusic.Instance.Source = Music.CurrentSource;
            GameMusic.Instance.Source.Play();
            isExecute = true;
            //buttons
            execute.isEnabled = false;
            cancel.isEnabled = true;
            //param
            uint clipTime = (uint)audioSource.clip.length;
            uint minute = clipTime / 60;
            uint second = clipTime % 60;
            musicFileTimeLabel.text = minute + ":" + string.Format("{0:D2}", second);
            difference = float.Parse(thresholdLabel.text);
            interval = float.Parse(intervalLabel.text);
            executeFilePath = currentFilePath;
            prevMax = 0;
            elapsedTime = 0;
        }
   );

    }

    public void MuteButton()
    {
#if UNITY_ANDROID
        FantomLib.AndroidPlugin.SetMediaVolume(0, true);
#endif
    }

    public void CancelButton()
    {
        GameMusic.Instance.Source.clip = null;
        GameMusic.Instance.Source = null;
        isExecute = false;
        //buttons
        execute.isEnabled = true;
        cancel.isEnabled = false;
        currentPlayTimeLabel.text ="0:00";
        DialogController.Instance.Open(
            "譜面の作成を中止しました。"
            );
    }
#endregion
}
