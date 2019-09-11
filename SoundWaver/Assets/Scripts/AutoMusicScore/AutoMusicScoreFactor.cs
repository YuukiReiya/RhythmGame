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

    float prevMax = 0;
    StreamWriter sw;

    [SerializeField]
    float minimum = 15;

    List<List<float>> specLists;
    List<float> ret;

    // Start is called before the first frame update
    void Start ()
    {
        sw = new StreamWriter("specDatas.txt", true);
        specLists = new List<List<float>> ();

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

        float maxSpec = spectrums.Max();
        sw.WriteLine((maxSpec * 100) > prevMax + minimum ? maxSpec.ToString() : "");
    }

    void Read ()
    {
        var file = new FileStream ("specDatas.txt", FileMode.Open, FileAccess.Read);
        var sr = new StreamReader (file);
        List<string> readBuffer = new List<string> ();
        while (sr.EndOfStream != true)
        {
            string line = sr.ReadLine ();
            readBuffer.Add (line);
        }
        for (int i = 0; i < readBuffer.Count; ++i)
        {
            var s = readBuffer[i];
        }

        ret = new List<float> ();
        foreach (var frame in specLists)
        {
            float maxValue = 0;
            maxValue = frame.Max () * 100;

            if (maxValue > prevMax + minimum)
            {
                Debug.Log ("Hit ＝ " + maxValue);
                ret.Add (maxValue);
            }

            prevMax = maxValue;
        }
        Debug.Log ("かんりょう");
    }

    void OnDestroy()
    {
        sw.Close();

        //sw = new StreamWriter ("kore.txt", true);
        //foreach (var it in ret)
        //{
        //    sw.WriteLine (it.ToString ());
        //}
        //sw.Close ();
    }
}