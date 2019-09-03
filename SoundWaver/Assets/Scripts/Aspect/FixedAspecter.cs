using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedAspecter : MonoBehaviour
{
    [SerializeField]
    Camera fixedAspectCamera;
    [SerializeField]
    float _width = 1080;
    [SerializeField]
    float _height = 2160;
    // Start is called before the first frame update
    void Start ()
    {
        FixedAspectRatio.Execute (fixedAspectCamera, _width, _height);
    }

    /// <summary>
    /// Reset is called when the user hits the Reset button in the Inspector's
    /// context menu or when adding the component the first time.
    /// </summary>
    void Reset ()
    {
        fixedAspectCamera = Camera.main;
    }
}