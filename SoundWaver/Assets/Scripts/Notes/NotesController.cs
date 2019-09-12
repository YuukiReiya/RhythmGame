﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class NotesController : Yuuki.SingletonMonoBehaviour<NotesController>
{
    [Header("Notes Control Parameter")]
    [SerializeField] float noteSpeed;
    public float NotesSpeed { get { return noteSpeed; } }
    [System.Serializable]
    struct TimingLine
    {
        public float y, z;
    }
    [SerializeField]
    TimingLine timingLine;//判定ラインの座標

    public List<INote> notes;
    [SerializeField] AudioSource audioSource;

    public Vector3 JustTimingPosition { get { return new Vector3(0, timingLine.y, timingLine.z); } } 

    public float elapsedTime { get; private set; }

    protected override void Awake ()
    {
        base.Awake ();
        notes = new List<INote> ();
    }

    // Start is called before the first frame update
    void Start () { }

    // Update is called once per frame
    void Update ()
    {
        Move();
        elapsedTime = audioSource.time;
    }

    /// <summary>
    /// 管理リストの更新
    /// </summary>
    void Renewal()
    {
        notes.RemoveAll(it => it.isReset);
    }

    void Move ()
    {
        foreach (var it in notes)
        {
            it.Move ();
        }
    }

    void OnDrawGizmos ()
    {
        Gizmos.color = Color.red;

        float left = -10, right = 10;
        Gizmos.DrawLine (new Vector3 (left, timingLine.y, timingLine.z), new Vector3 (right, timingLine.y, timingLine.z));
    }

#if false
    //何故かタップ時にすべてのレーンでレーン番号が"3"になる。

    /// <summary>
    /// タップ時に呼ばれるイベントのセットアップ
    /// TODO:イベントが増えるようなら登録メソッドを用意してパブリックにする
    /// </summary>
    void TapEventsSetup()
    {
        //レーンタップ
        for(int i=0;i<laneTapEvents.Length;++i)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener((it) => { Debug.Log("lane" + i); OnTapLane(i); });
            laneTapEvents[i].triggers.Add(entry);
        }
    }

    void OnTapLane(int laneNumber)
    {
        if (notes.Where(it => it.LaneNumber == laneNumber).Count() == 0) { return; }

        //判定ノーツ
        INote note = notes.
            //対象のレーン
            Where(it => it.LaneNumber == laneNumber).
            //判定ラインに最も近いノーツ = (判定時間 - 再生時間 の差分が最も小さい)
            OrderBy(it => Mathf.Abs(it.DownTime-audioSource.time)).
            First();

        //ノーツが"n"秒経過していたら処理しない
        var tapTime = note.DownTime - audioSource.time;
        if (Mathf.Abs(tapTime) < 10.0f) { return; }

        //
        note.Unregister();
    }
#endif
}