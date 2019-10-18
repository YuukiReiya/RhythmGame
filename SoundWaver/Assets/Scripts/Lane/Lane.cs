using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    public class Lane : MonoBehaviour
    {
        //serialize param
        [SerializeField, Tooltip("ノーツの流れるレーン番号")] uint laneNumber;
        [SerializeField, Tooltip("関連付けさせるイベントトリガー")] EventTrigger eventTrigger;

        //accessor
        float elapsedTime { get { return GameController.Instance.ElapsedTime; } }

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
            var notes = NotesController.Instance.Notes;
            if (notes.Where(it => it.LaneNumber == laneNumber).Count() == 0) { return; }
            INote note = notes.
                //対象のレーン
                Where(it => it.LaneNumber == laneNumber).
                //判定ラインに最も近いノーツ = (判定時間 - 再生時間 の差分が最も小さい)
                OrderBy(it => Mathf.Abs(it.DownTime - elapsedTime)).//ココの判定いらない可能性:大
                First();

            //ノーツが"n"秒経過していたら処理しない
            //TODO:処理用確認
            var tapTime = note.DownTime - elapsedTime;
            if (Mathf.Abs(tapTime) > NotesController.Instance.WaitTime) { return; }

            //判定
            string judge;
            ScoreController.Score result = ScoreController.Score.PERFECT;

            //TODO:汚い
            if (Mathf.Abs(tapTime) <= Common.Define.c_PerfectTime)
            {
                judge = "perfect";
                result = ScoreController.Score.PERFECT;
            }
            else if (Mathf.Abs(tapTime) <= Common.Define.c_GreatTime)
            {
                judge = "great";
                result = ScoreController.Score.GREAT;
            }
            else if (Mathf.Abs(tapTime) <= Common.Define.c_GoodTime)
            {
                judge = "good";
                result = ScoreController.Score.GOOD;
            }
            else
            {
                judge = "miss";
                result = ScoreController.Score.MISS;
            }
            Debug.Log(judge);
            ScoreController.Instance.StartScoreEffect(result);

            //note.Unregister();
        }

        private void Reset()
        {
            if (!TryGetComponent<EventTrigger>(out eventTrigger)) { return; }
        }
    }
}
