using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hoge : MonoBehaviour
{
    public AudioSpectrum audioSpectrum;
    public float scale = 10;
    public GameObject[] objects;
    // Start is called before the first frame update
    void Start ()
    {

    }

    // Update is called once per frame
    void Update ()
    {
        for (int i = 0; i < objects.Length; ++i)
        {
            var obj = objects[i];
            var lScale = obj.transform.localScale;
            lScale.y = audioSpectrum.Levels[i] * scale;
            obj.transform.localScale = lScale;
        }
    }
}