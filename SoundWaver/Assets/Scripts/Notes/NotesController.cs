using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.UI;
using System.IO;
using Yuuki.FileIO;
using Common;
using System.Linq;
namespace Game
{
    public class NotesController : Yuuki.SingletonMonoBehaviour<NotesController>
    {
        //serialeze param
        [Header("Notes Control Parameter")]
        [SerializeField] float noteSpeed;
        [SerializeField, Range(0, 1), Tooltip("算出されたノーツのキーを受け付けない時間")] float waitTime = 0.5f;
        [SerializeField, Range(1.0f, 10.0f), Tooltip("ノーツを出現させる時間")] float activeTime;
        [System.Serializable]
        struct TimingLine
        {
            public float y, z;
        }
        [Header("Lane Parameter")]
        [SerializeField] TimingLine timingLine;//判定ラインの座標
        [SerializeField] private Vector3 notesDirection;//ノーツの移動方向
        //[SerializeField] AudioSource audioSource;

        //private param
        private Queue<Chart.Note> noteQueue;
        //accessor
        public Vector3 JustTimingPosition { get { return new Vector3(0, timingLine.y, timingLine.z); } }
        public float NotesSpeed { get { return noteSpeed; } }
        public float WaitTime { get { return waitTime; } }
        public List<INote> Notes { get; private set; }
        public Vector3 NotesDirection { get { return notesDirection; } }
        //public Queue<INote> NotesQueue { get; private set; }
        protected override void Awake()
        {
            base.Awake();
            Notes = new List<INote>();
#if UNITY_EDITOR
            if (ChartManager.Chart.Notes != null)
            {
                noteQueue = new Queue<Chart.Note>(ChartManager.Chart.Notes);
            }
            else
            {
                noteQueue = new Queue<Chart.Note>();
            }
            return;
#endif
            noteQueue = new Queue<Chart.Note>(ChartManager.Chart.Notes);
        }

        public void SetupNotes()
        {
            Notes.Clear();
            noteQueue.Clear();
            foreach (var it in ChartManager.Chart.Notes)
            {
                noteQueue.Enqueue(it);
            }
            foreach (var it in SingleNotesPool.Instance.PoolList) { it.SetActive(false); }

            //ノーツ速度設定
            var io = new FileIO();
            IniFile ini;
            if (!File.Exists(Define.c_SettingFilePath))
            {
                //設定ファイルがないので生成
                ini = new IniFile();
                ini.Setup();
                //ファイルを上書きモードで生成
                io.CreateFile(
                 Define.c_SettingFilePath,
                 JsonUtility.ToJson(ini),
                 FileIO.FileIODesc.Overwrite
                 );
            }
            ini = JsonUtility.FromJson<IniFile>(io.GetContents(Define.c_SettingFilePath));
            //データ取得
            //TODO:こんな変換するぐらいなら最初から全部定数化しとけば良かった。。。
            var value = Define.c_NotesSpeedList.First(it => it.Item2 == ini.NotesSpeed);//Commonのリストから配列の添え字を引っ張てくる
            noteSpeed = ini.NotesSpeedList[value.Item1];//item1に配列番号が入っている
        }

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
                    note.Register(ptr.LaneNumber, ptr.Time);
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
                var time = (it.DownTime - GameController.Instance.ElapsedTime);
                //押されるべき時間 - 実際の時間 > -(Good判定時間)
                if (time < -Common.Define.c_GoodTime)
                {
                    //ミス判定処理
                    Judge.Execute(time);

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
