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
public class AutoMusicScoreFactor : Yuuki.SingletonMonoBehaviour<AutoMusicScoreFactor>
{
    //serialize param
    [SerializeField] int sampleCount = 2048;
    [SerializeField] int channelNumber = 0;
    [SerializeField] UILabel musicTitle;
    [SerializeField] UILabel chartName;
    [SerializeField] UILabel unitPerBeat;
    [SerializeField] UILabel unitPerBar;
    //accessor
    public string title { get; set; }
    public uint bpm { get; set; }
    AudioSource audioSource { get { return Game.GameMusic.Instance.Source; } }
    //private param
    float prevMax = 0;
    bool isStart = false;

    [SerializeField, Range(0, 100.0f),Tooltip("直前のフレームの最大スペクトラムと比較して、(この値 / 100) 大きければノーツを生成")]
    float difference = 15f;
    List<float> ret;

    //uint musicalBarCount = 0;//小節数

    //const param
    readonly string c_ChartSaveDirectory = Application.streamingAssetsPath;

    // Start is called before the first frame update
    void Start ()
    {
        //FileInfo fi = new FileInfo("Chart1.json");
        //sw = new StreamWriter("Chart1.json", true);
        //ret = new List<float>();

        //Chart test = new Chart();
        //test.Title = "シャイニングスター";
        //test.BPM = 158;

        //string jsonRead = JsonUtility.ToJson(test);
        //Debug.Log(jsonRead);

        //Read ();
    }

    // Update is called once per frame
    void Update ()
    {
        if (isStart)
        {
            Execute();
            //終了タイミング
            if(audioSource.time==0.0f&&!audioSource.isPlaying)
            {
                Debug.Log("再生終了");
                isStart = false;
                CreateChart();
            }
        }
    }
    
    void Execute ()
    {
#if false
        var spectrums = audioSource.GetSpectrumData (sampleCount, channelNumber, FFTWindow.Blackman);

        if(Music.IsJustChangedBar())
        {
            ret.Add(audioSource.time);
        }
#endif
        var spectrums = audioSource.GetSpectrumData(sampleCount, channelNumber, FFTWindow.Blackman);
        float max = spectrums.Max();

        if(Music.IsJustChanged)
        {
            if (max > prevMax + (difference / 100))
            {
                ret.Add(audioSource.time);
            }
        }
        prevMax = max;
        //specLists.Add(spectrums.ToList<float>());
    }

    public void Setup()
    {
        GameMusic.Instance.LoadAndFunction(
            FileManager.Instance.CurrentDirectory + "\\" + musicTitle,
            () =>
            {
                bpm = (uint)UniBpmAnalyzer.AnalyzeBpm(GameMusic.Instance.Clip);
                var sec = Music.GetSection(0);
                sec.Tempo = bpm;
                sec.UnitPerBar = int.Parse(unitPerBar.text);
                sec.UnitPerBeat = int.Parse(unitPerBeat.text);
                isStart = true;
                GameMusic.Instance.Source.clip = GameMusic.Instance.Clip;
                GameMusic.Instance.Source.Play();
            }
       );
    }

    public void SetupMusicName(string fileName)
    {
        musicTitle.text = fileName;
    }

    public void CreateChart()
    {
        var fileIO = new Yuuki.FileIO.FileIO();
        Chart chart = new Chart();
        chart.Title = musicTitle.text;
        chart.BPM = bpm;
        chart.timing = ret.ToArray();
        fileIO.CreateFile(
            c_ChartSaveDirectory + "\\" + chartName + Define.c_JSON,
            JsonUtility.ToJson(chart)
            );
    }

    void OnDestroy()
    {
        Chart chart = new Chart();
        chart.Title = title;
        chart.BPM = bpm;
        //chart.
        //chart.timing = specLists.Select(it=>it.Max()).ToArray();
        chart.timing = ret.ToArray();

        var jsonData = JsonUtility.ToJson(chart);
        //sw.Write(jsonData);
        //sw.Close();

        //sw = new StreamWriter ("kore.txt", true);
        //foreach (var it in ret)
        //{
        //    sw.WriteLine (it.ToString ());
        //}
        //sw.Close ();
    }
}