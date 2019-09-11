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

    [SerializeField]
    float minimum = 15;

    [SerializeField]
    bool isOverWrite = true;

    List<List<float>> specLists;
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
        specLists = new List<List<float>> ();
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
        var spectrums = audioSource.GetSpectrumData (sampleCount, channelNumber, FFTWindow.Blackman);

        //foreach (var it in spectrums)
        //{
        //    sw.Write (it.ToString () + ",");
        //}
        //sw.WriteLine ();

        //float maxSpec = spectrums.Max();
        //sw.WriteLine(
        //    (maxSpec * 100) > prevMax + minimum ?
        //    audioSource.time.ToString()
        //    :
        //    "");

        if (spectrums.Max() * 100 > prevMax + minimum)
        {
            ret.Add(audioSource.time);
        }

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