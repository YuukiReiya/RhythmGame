using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hoge : MonoBehaviour
{
    AudioListener audioListener;
    // Start is called before the first frame update
    void Start ()
    {
        audioListener = GetComponent<AudioListener> ();
    }

    // Update is called once per frame
    void Update ()
    {
    }
}