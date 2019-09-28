using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test3 : MonoBehaviour
{
    //public Yuuki.FileIO.ExternalFileIO exFileIO;
    AudioSource source;
    public string path;
    AudioClip clip;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        Debug.Log("call");
        path = ""+"a.mp3";
        //exFileIO.GetAudioClip(path, ref clip,()=> { Execute(); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Execute()
    {
        source.clip = clip;
        source.Play();
    }
}
