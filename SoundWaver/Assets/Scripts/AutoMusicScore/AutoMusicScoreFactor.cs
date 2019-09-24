using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
public class AutoMusicScoreFactor : MonoBehaviour
{
    [SerializeField]
    int sampleCount = 2048;
    [SerializeField]
    int channelNumber = 0;
    [SerializeField]
    AudioSource audioSource;

    [Header("Music Infomation")]
    public string title;
    public uint bpm;

    float prevMax = 0;
    StreamWriter sw;

    [SerializeField, Range(0, 100.0f),Tooltip("直前のフレームの最大スペクトラムと比較して、(この値 / 100) 大きければノーツを生成")]
    float difference = 15f;

    [SerializeField]
    bool isOverWrite = true;

    List<float> ret;

    //uint musicalBarCount = 0;//小節数
    //uint 

    // Start is called before the first frame update
    void Start ()
    {
        FileInfo fi = new FileInfo("Chart1.json");
        if (fi.Exists)
        {
            if (isOverWrite) { fi.Delete(); }
        }

        sw = new StreamWriter("Chart1.json", true);
        ret = new List<float>();

        Chart test = new Chart();
        test.Title = "シャイニングスター";
        test.BPM = 158;

        string jsonRead = JsonUtility.ToJson(test);
        Debug.Log(jsonRead);

        //Read ();
    }

    // Update is called once per frame
    void Update ()
    {
        Execute();
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

    void OnDestroy()
    {
        Chart chart = new Chart();
        chart.Title = title;
        chart.BPM = bpm;
        //chart.
        //chart.timing = specLists.Select(it=>it.Max()).ToArray();
        chart.timing = ret.ToArray();

        var jsonData = JsonUtility.ToJson(chart);
        sw.Write(jsonData);
        sw.Close();

        //sw = new StreamWriter ("kore.txt", true);
        //foreach (var it in ret)
        //{
        //    sw.WriteLine (it.ToString ());
        //}
        //sw.Close ();
    }
}