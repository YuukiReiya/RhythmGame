/// <summary>
/// 番場宥輝
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// FPS計測
/// </summary>
public class FPSmeter : MonoBehaviour {

    [SerializeField,Tooltip("TextComponent")] Text text;

    private int frameCount;
    private float prevTime;

	// Use this for initialization
	void Start () {
        frameCount = 0;
        prevTime = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
        ++frameCount;
        float time = Time.realtimeSinceStartup - prevTime;

        if (time >= 0.5f)
        {
            int fps = (int)(frameCount / time);
            //Debug.LogFormat("{0}fps", frameCount / time);
            //Debug.LogFormat("{0}fps", fps);
            text.text = "FPS:" + fps;
            frameCount = 0;
            prevTime = Time.realtimeSinceStartup;

        }
    }
}
