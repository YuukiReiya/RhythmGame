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
        [System.Serializable]
        struct TimingLine
        {
            public float y, z;
        }
        [Header("Lane Parameter")]
        [SerializeField] TimingLine timingLine;//判定ラインの座標
        //[SerializeField] AudioSource audioSource;

        //private param
        public List<INote> notes;
        private Queue<INote> noteQueue;

        //accessor
        public Vector3 JustTimingPosition { get { return new Vector3(0, timingLine.y, timingLine.z); } }
        public float NotesSpeed { get { return noteSpeed; } }
        public float WaitTime { get { return waitTime; } }

        //public float elapsedTime { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            notes = new List<INote>();
        }

        // Start is called before the first frame update
        void Start() { }

        // Update is called once per frame
        void Update() { }

        /// <summary>
        /// 管理リストの更新
        /// </summary>
        void Renewal()
        {
            notes.RemoveAll(it => it.isReset);
        }

        public void Move()
        {
            foreach (var it in notes)
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
