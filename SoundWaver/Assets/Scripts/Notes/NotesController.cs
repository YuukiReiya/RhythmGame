using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class NotesController : Yuuki.SingletonMonoBehaviour<NotesController>
    {
        //serialeze param
        [Header("Notes Control Parameter")]
        [SerializeField] float noteSpeed;
        [SerializeField, Tooltip("算出されたノーツのキーを受け付けない時間")] float waitTime = 5.0f;
        [SerializeField, Range(1.0f, 10.0f), Tooltip("ノーツを出現させる時間")] float activeTime;
        [SerializeField, Range(-10.0f, 0.0f), Tooltip("ノーツを強制削除する時間")] float disableTime;
        [System.Serializable]
        struct TimingLine
        {
            public float y, z;
        }
        [Header("Lane Parameter")]
        [SerializeField] TimingLine timingLine;//判定ラインの座標
        //[SerializeField] AudioSource audioSource;

        //private param
        private Queue<Chart.Note> noteQueue;
        //accessor
        public Vector3 JustTimingPosition { get { return new Vector3(0, timingLine.y, timingLine.z); } }
        public float NotesSpeed { get { return noteSpeed; } }
        public float WaitTime { get { return waitTime; } }
        public List<INote> Notes { get; private set; }
        //public Queue<INote> NotesQueue { get; private set; }
        protected override void Awake()
        {
            base.Awake();
            Notes = new List<INote>();
            noteQueue = new Queue<Chart.Note>(ChartManager.Instance.Chart.Notes);
            //NotesQueue = new Queue<INote>();
        }

        // Start is called before the first frame update
        void Start() { }

        // Update is called once per frame
        void Update() { }

        /// <summary>
        /// 管理リストの更新
        /// </summary>
        public void Renewal()
        {
            //登録するノーツのキュー
            foreach (var it in noteQueue)
            {
                //押す時間 - 経過時間 <= アクティブ時間
                if ((it.Time - GameController.Instance.ElapsedTime) <= activeTime)
                {
                    //初期化処理
                    var inst = SingleNotesPool.Instance.GetObject();
                    INote note;
                    if (inst.TryGetComponent(out note))
                    {
                        Debug.LogError("プールするノーツのプレハブにINoteコンポーネントがアタッチされていない");
                        continue;
                    }
                    note.Setup(it.LaneNumber, it.Time);
                    //登録するノーツキューの更新
                    noteQueue.Dequeue();
                    //更新リストに追加
                    Notes.Add(note);
                    continue;
                }
                break;
            }
        }

        public void Move()
        {
            foreach (var it in Notes)
            {
                it.Move();
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            float left = -10, right = 10;
            Gizmos.DrawLine(new Vector3(left, timingLine.y, timingLine.z), new Vector3(right, timingLine.y, timingLine.z));
        }

        public void Setup(Chart chart)
        {
            Debug.Log("notes:" + chart.Notes.Length);
            Debug.Log("chart:" + chart);
            foreach (var it in chart.Notes)
            {
                //var note = SingleNotesPool.Instance.GetObject().GetComponent<SingleNote>();
                //note.Setup(it.LaneNumber, it.Time);
                SingleNote note;
                if (SingleNotesPool.Instance.GetObject().TryGetComponent(out note))
                {
                    note.Setup(it.LaneNumber, it.Time);
                }
                else
                {
                    //Debug.Log("not found");
                }
            }
        }
    }
}
