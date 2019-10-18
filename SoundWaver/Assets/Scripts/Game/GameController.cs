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

        protected override void Awake()
        {
            base.Awake();
        }
        // Start is called before the first frame update
        void Start()
        {
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
            //経過時間の更新
            ElapsedTime = source.time;
        }

        void Setup()
        {
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
