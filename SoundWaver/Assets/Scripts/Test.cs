using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public AudioClip a;
    FIOtest beatIO;
    FIOtest specIO;
    public AudioProcessor ap;
    float[] samples;
    // Start is called before the first frame update
    void Start ()
    {
        beatIO = new FIOtest ();
        specIO = new FIOtest ();
        beatIO.Setup ("Beat.txt");
        Debug.Log ("Beat.txt作成");
        specIO.Setup ("Spectrum.txt");
        //specIO

        int bpm = UniBpmAnalyzer.AnalyzeBpm (a);
        Debug.Log ("BPM = " + bpm);
        ap.onBeat.AddListener (OnBeat);
        ap.onSpectrum.AddListener (OnSpectrum);

        samples = new float[1024];
        AudioListener.GetOutputData (samples, 0);
        Debug.Log ("サンプル");
        foreach (var it in samples)
        {
            Debug.Log ("sample = " + it);
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if (ap.audioSource.time == 0.0f && ap.audioSource.isPlaying == false)
        {
            Debug.Log ("終了");
            beatIO.Shutdown ();
            foreach (var it in samples)
            {
                Debug.Log ("sample = " + it);
            }

            return;
        }
    }

    private void OnBeat ()
    {
        //Debug.Log ("Beat!!!");
        beatIO.Write (ap.audioSource.time.ToString ());
    }
    private void OnSpectrum (float[] spectrum)
    {
        for (int i = 0; i < spectrum.Length; ++i)
        {
            //    Debug.Log (spectrum[i]);
            specIO.Write (spectrum[i].ToString ());
        }
    }
}