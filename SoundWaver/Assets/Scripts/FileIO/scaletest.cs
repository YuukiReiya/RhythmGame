using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
public class scaletest : MonoBehaviour
{
    public float rate = 1.1f;
    public float time = 0.2f;
    private Vector3 tScale;

    // Start is called before the first frame update
    void Start ()
    {
        tScale = transform.localScale;
    }

    // Update is called once per frame
    void Update ()
    {
        if (Music.IsPlaying && Music.IsJustChangedBeat())
        {
            transform.DOScale (tScale * rate, 0.0f);
            transform.DOScale (tScale, time);
        }
    }
}