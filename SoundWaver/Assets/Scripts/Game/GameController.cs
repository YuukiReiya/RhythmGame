using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using API.Util;
using Common;
using UnityEngine.UI;
namespace Game
{
    public class GameController : Yuuki.SingletonMonoBehaviour<GameController>
    {
        //serialize param
        //[SerializeField] SceneTransitionCommand sceneTransitionCommand;
        [SerializeField,Tooltip("ゲーム開始前に待機(遅延)する時間")] private float delayTime;
        [SerializeField] private AudioSource source;
        [SerializeField] PauseCanvas pauseCanvas;
        //private param
        private bool isStart;
        //public param
        //accessor
        public float ElapsedTime { get; private set; }
        //public uint Comb { get; set; }

#if UNITY_EDITOR
        public bool isTest; 
#endif

        protected override void Awake()
        {
            base.Awake();
        }
        // Start is called before the first frame update
        void Start()
        {
#if UNITY_EDITOR
            //ゲームシーンのテスト
            if (isTest)
            {
                Judge.Reset();
                var path = Define.c_StreamingAssetsPath + Define.c_Delimiter + "Sounds\\" + Define.c_Delimiter + System.IO.Path.GetFileNameWithoutExtension(Define.c_PresetFilePath[0].Item1) + Define.c_MP3;
                IEnumerator routine()
                {
                    yield return GameMusic.Instance.LoadToAudioClip(path);

                    using (var www = UnityEngine.Networking.UnityWebRequest.Get(
                        Define.c_ChartSaveDirectory + Define.c_Delimiter + System.IO.Path.GetFileName(Define.c_PresetFilePath[0].Item2)
                        ))
                    {
                        yield return www.SendWebRequest();
                        if (www.isNetworkError || www.isHttpError) { Debug.LogError(www.error); }
                        ChartManager.Chart = JsonUtility.FromJson<Chart>(www.downloadHandler.text);
                    }

                    //クリップオブジェクトの生成まで待機
                    yield return new WaitUntil(
                        () => { return GameMusic.Instance.Clip; }
                        );
                    //楽曲ファイルのロード中は待機
                    yield return new WaitWhile(
                        () => { return GameMusic.Instance.Clip.loadState == AudioDataLoadState.Loading; }
                        );
                    source.clip = GameMusic.Instance.Clip;

                    if (NotesController.Instance == null)
                    {
                        SceneManager.LoadScene("SelectDev");
                        yield break;
                    }

                    NotesController.Instance.SetupNotes();
                    Setup();
                    #region ノーツの初期表示
                    //※これがないと曲が鳴っていきなり表れてしまう
                    NotesController.Instance.Renewal();
                    NotesController.Instance.Move();
                    #endregion
                    StartCoroutine(DelayStart());

                    yield break;
                }
                StartCoroutine(routine());
                return;
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
            //param
            isStart = false;
            pauseCanvas.Setup(source);

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
            FadeController.Instance.FadeOut(delayTime / 4);
            yield return new WaitForSeconds(delayTime);
            isStart = true;
            source.Play();
        }

        /// <summary>
        /// ゲーム終了
        /// </summary>
        private void GameEnd()
        {
            //isStart = false;
            SceneManager.LoadScene("Result");
            Destroy(NotesController.Instance.gameObject);
        }
    }
}
