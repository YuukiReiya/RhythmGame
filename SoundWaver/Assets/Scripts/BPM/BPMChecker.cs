using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BPMChecker : MonoBehaviour
{
    [SerializeField] AudioClip checkAudioClip;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("Execute")]
    public void Execute()
    {
        Debug.Log("BPM = " + UniBpmAnalyzer.AnalyzeBpm(checkAudioClip));
    }
}
