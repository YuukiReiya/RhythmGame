using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class Lane : MonoBehaviour
{
    [SerializeField,Tooltip("ノーツの流れるレーン番号")] uint laneNumber;
    [SerializeField, Tooltip("関連付けさせるイベントトリガー")] EventTrigger eventTrigger;

    private void Awake()
    {
        SetupTapEvent();
    }

    void SetupTapEvent()
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener((it) => { OnTapLane(); });
        eventTrigger.triggers.Add(entry);
    }

    void OnTapLane()
    {
        Debug.Log("lane = " + laneNumber);

        var notes = NotesController.Instance.notes;
        if (notes.Where(it => it.LaneNumber == laneNumber).Count() == 0) { return; }
        INote note = notes.
            //対象のレーン
            Where(it => it.LaneNumber == laneNumber).
            //判定ラインに最も近いノーツ = (判定時間 - 再生時間 の差分が最も小さい)
            OrderBy(it => Mathf.Abs(it.DownTime - NotesController.Instance.elapsedTime)).
            First();

        //ノーツが"n"秒経過していたら処理しない
        var tapTime = note.DownTime - NotesController.Instance.elapsedTime;
        if (Mathf.Abs(tapTime) > 10.0f) { return; }

        //判定
        string judge;

        if(Mathf.Abs(tapTime)<=Common.Define.c_PerfectTime)
        {
            judge = "perfect";
        }
        else if(Mathf.Abs(tapTime) <= Common.Define.c_PerfectTime)
        {
            judge = "great";
        }
        else if (Mathf.Abs(tapTime) <= Common.Define.c_GoodTime)
        {
            judge = "good";
        }
        else
        {
            judge = "miss";
        }
        Debug.Log(judge);

        note.Unregister();
    }

    private void Reset()
    {
        if(!TryGetComponent<EventTrigger>(out eventTrigger)) { return; }
    }
}
