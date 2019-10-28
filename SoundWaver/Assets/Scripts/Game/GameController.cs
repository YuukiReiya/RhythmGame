using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using API.Util;
namespace Game
{
    public class GameController : Yuuki.SingletonMonoBehaviour<GameController>
    {
        //serialize param
        //[SerializeField] SceneTransitionCommand sceneTransitionCommand;
        [SerializeField,Tooltip("ゲーム開始前に待機(遅延)する時間")] private float delayTime;
        //private param
        private bool isStart;
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
            #region ノーツの初期表示
            //※これがないと曲が鳴っていきなり表れてしまう
            NotesController.Instance.Renewal();
            NotesController.Instance.Move();
            #endregion
            StartCoroutine(DelayStart());
        }

        // Update is called once per frame
        void Update()
        {
            //ゲームが始まっている?
            if (!isStart) { return; }
            //終了タイミング
            if (source.time == 0.0f && !source.isPlaying)
            {
                GameEnd();
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
                //source.Play();
                Music.CurrentSetup();
            }
            else
            {
                //エラー処理
                //(確認用のダイアログを出す.etc)
            }
        }

        private IEnumerator DelayStart()
        {
            FadeController.Instance.Stop();
            float alp = 0.3f;
            FadeController.Instance.SetAlpha(alp);
            //FadeController.Instance.EventQueue.Enqueue()
                FadeController.Instance.FadeOut(delayTime / 8);
            yield return new WaitForSeconds(delayTime);
            isStart = true;
            source.Play();
        }

        /// <summary>
        /// ゲーム終了
        /// </summary>
        private void GameEnd()
        {
            SceneManager.LoadScene("Select");
            Destroy(NotesController.Instance.gameObject);
        }
    }
}
