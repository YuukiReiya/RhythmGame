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
            while (noteQueue.Count > 0)
            {
                var ptr = noteQueue.Peek();
                //押す時間 - 経過時間 <= アクティブ時間
                if ((ptr.Time - GameController.Instance.ElapsedTime) <= activeTime)
                {
                    //初期化処理
                    var inst = SingleNotesPool.Instance.GetObject();
                    INote note;
                    if (!inst.TryGetComponent(out note))
                    {
                        Debug.LogError("プールするノーツのプレハブにINoteコンポーネントがアタッチされていない");
                        continue;
                    }
                    // note.Register(ptr.LaneNumber, ptr.Time);
                    note.Register(0, ptr.Time);
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

        /// <summary>
        /// ノーツリストの廃棄処理
        /// </summary>
        public void Discard()
        {
            //foreach文内ではコレクションの書き換えが出来ないためリストを分ける
            var discardNotes = new Queue<INote>();
            foreach(var it in Notes)
            {
                //押されるべき時間 - 実際の時間 > -(Good判定時間)
                if ((it.DownTime - GameController.Instance.ElapsedTime) < -Common.Define.c_GoodTime)
                {
                    //ミス判定処理
                    ScoreController.Instance.StartScoreEffect(ScoreController.Judge.MISS);

                    //ノーツの登録解除
                    it.Unregister();

                    //破棄リストに追加
                    discardNotes.Enqueue(it);
                }
                break;
            }
            //破棄リスト内のノーツの登録を解除しているだけ
            foreach (var it in discardNotes) { Notes.Remove(it); }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            float left = -10, right = 10;
            Gizmos.DrawLine(new Vector3(left, timingLine.y, timingLine.z), new Vector3(right, timingLine.y, timingLine.z));
        }

    }
}
