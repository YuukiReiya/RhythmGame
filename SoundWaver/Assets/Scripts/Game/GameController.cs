using UnityEngine;

namespace Game
{
    public class GameController : Yuuki.SingletonMonoBehaviour<GameController>
    {
        //serialize param
        [SerializeField] SceneTransitionCommand sceneTransitionCommand;
        //private param
        //public param
        public AudioSource source;
        //accessor
        public float ElapsedTime { get; private set; }
        public uint Comb { get; set; }
        protected override void Awake()
        {
            base.Awake();
        }
        // Start is called before the first frame update
        void Start()
        {
#if UNITY_EDITOR
            //ゲームシーンでエディター起動時セレクトに戻してあげる
            if (!source.clip)
            {
            }
#endif
            Setup();
        }

        // Update is called once per frame
        void Update()
        {
            //終了タイミング
            if (source.time == 0.0f && !source.isPlaying)
            {
                sceneTransitionCommand.Execute();
                Destroy(ChartManager.Instance.gameObject);
            }
            //管理ノーツの更新
            NotesController.Instance.Renewal();
            //ノーツの移動
            NotesController.Instance.Move();
            //ノーツの廃棄
            NotesController.Instance.Discard();
            //経過時間の更新
            ElapsedTime = source.time;
        }

        void Setup()
        {
            //パラメータの初期化
            Comb = 0;

            //楽曲データロード済み
            if (GameMusic.Instance.Clip)
            {
                source.clip = GameMusic.Instance.Clip;
                source.Play();
                Music.CurrentSetup();
            }
            else
            {
                //エラー処理
                //(確認用のダイアログを出す.etc)
            }
        }
    }
}
