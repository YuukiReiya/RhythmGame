using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Chart 
{
    public string Title;
    public uint BPM;

    //TODO:小節、拍子を入れる構造体に置換
    //[System.Serializable]
    //public struct Info
    //{
    //    public uint s;
    //    public uint b;
    //}
    //public Info[] isb;


    public float[] timing;
}
