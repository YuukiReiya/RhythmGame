using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tmp : MonoBehaviour
{
    const int BPM = 158;
    public AudioSource audioSource;
    public AudioSource shotSe;
    Read r;
    // Start is called before the first frame update
    void Start ()
    {
        r = new Read ();
        r.values = new List<float> ();
        r.Execute ("Beat.txt");
        Debug.Log ("書き出し官僚");
        foreach (var item in r.values)
        {
            Debug.Log (item);
        }
        //15 = 60 / 4
        Debug.Log ("bpm16 = " + ((float) 15 / (float) BPM));
        Debug.Log ("size = " + r.values.Count);
    }
    int c = 0;
    // Update is called once per frame
    void Update ()
    {
        Debug.Log ("c = " + c);
        if (c == r.values.Count) { return; }
        if (audioSource.time == r.values[c])
        {
            Debug.Log ("shot se");
            shotSe.PlayOneShot (shotSe.clip);
            c++;
        }
        else if (audioSource.time >= r.values[c])
        {
            c++;
        }
    }
}