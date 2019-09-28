using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Chart 
{
    public string Title;
    /// <summary>
    /// 譜面データは一律同じ場所で連番で管理するようにする
    /// </summary>
    public string FilePath;//楽曲ファイルが格納されているパス
    public uint BPM;
    public string ResistName;//登録名
    public uint Number;//楽曲番号
    public float NotesInterval;
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
