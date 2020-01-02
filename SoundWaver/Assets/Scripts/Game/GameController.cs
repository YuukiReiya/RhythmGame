using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using API.Util;
using Common;
using Game.UI;
namespace Game
{
    public class GameController : Yuuki.SingletonMonoBehaviour<GameController>
    {
        //serialize param
        //[SerializeField] SceneTransitionCommand sceneTransitionCommand;
        [SerializeField,Tooltip("ゲーム開始前に待機(遅延)する時間")] private float delayTime;
        [SerializeField] private AudioSource source;
        [Header("SCORE")]
        [SerializeField] ScorePanel scorePanel;
        [Header("Pause")]
        [SerializeField] PauseCanvas pauseCanvas;
        [Header("Lane Material")]
        [SerializeField]private Material sideLaneMat;
        [SerializeField]private Material centerLaneMat;
        [SerializeField] private int laneMatPriority = 2;
        [Header("Notes Material")]
        [SerializeField] private Material notesMaterial;
        [SerializeField] private int notesMatPriority = 10;

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
            MaterialUtil.SetBlendMode(notesMaterial, MaterialUtil.Mode.Transparent, notesMatPriority);
            MaterialUtil.SetBlendMode(sideLaneMat, MaterialUtil.Mode.Transparent, laneMatPriority);
            MaterialUtil.SetBlendMode(centerLaneMat, MaterialUtil.Mode.Transparent, laneMatPriority);
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

                    NotesController.Instance.SetupNotesData();
                    Setup();
                    #region ノーツの初期表示
                    //※これがないと曲が鳴っていきなり表れてしまう
                    NotesController.Instance.Renewal();
                    NotesController.Instance.Move();
                    #endregion
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
            scorePanel.Setup();
            pauseCanvas.Setup(
                source,                                                                                     //参照するオーディオソース
#if LaneMaterialSpeed
                ()=> { SetLaneMatScrollSpeed(0); },                                         //ポーズ時に呼ばれるコールバック
                () => { SetLaneMatScrollSpeed(GetCalcLaneMatScrollSpeed()); } //アンポーズ時に呼ばれるコールバック
#else
                null, null
#endif
                );
#if LaneMaterialSpeed
            SetLaneMatScrollSpeed(0);//レーンのマテリアルのスクロールを抑制
#endif
            pauseCanvas.PauseButton.isEnabled = false;//ポーズボタン無効化
            Countdown.Instance.Widget.alpha = 0;

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

            //フェード
            FadeController.Instance.Stop();
            float alp = 0.3f;
            FadeController.Instance.SetAlpha(alp);
            FadeController.Instance.FadeOut(delayTime / 4);

            //カウントによる遅延処理
            Countdown.Instance.Execute(
                Define.c_WaitTimeCount,
                () =>
                {
                    isStart = true;
                    source.Play();
#if LaneMaterialSpeed
                    SetLaneMatScrollSpeed(GetCalcLaneMatScrollSpeed());//レーンマテリアルのスクロール速度を設定
#endif
                    pauseCanvas.PauseButton.isEnabled = true;//ポーズボタン有効化
                }
                );
        }

        /// <summary>
        /// ゲーム終了
        /// </summary>
        private void GameEnd()
        {
            //isStart = false;
            FadeController.Instance.EventQueue.Enqueue(
                () => 
                {
                    Destroy(NotesController.Instance.gameObject);
                    SceneManager.LoadScene("ResultDev"); 
                }
                );
            FadeController.Instance.FadeIn(Define.c_FadeTime);
        }

        /// <summary>
        /// セレクトシーンに移動
        /// </summary>
        public void TransitionSelect()
        {
            FadeController.Instance.EventQueue.Enqueue(
                () =>
                {
                    SceneManager.LoadScene("SelectDev");
                    var notes = SingleNotesPool.Instance.PoolList;
                    foreach(var it in notes)
                    {
                        it.SetActive(false);
                    }
                });
            FadeController.Instance.FadeIn(Define.c_FadeTime);
        }

#if LaneMaterialSpeed
#region     レーンマテリアルのUVスクロール有版処理
        /// <summary>
        /// レーンのマテリアルに設定するUVスクロールの速度を計算。
        /// ※ノーツ速度等から計算もしくは列挙にするため関数を用意
        /// </summary>
        /// <returns></returns>
        private float GetCalcLaneMatScrollSpeed()
        {
            float ret = 0;

            ret = 1.9f;
            centerLaneMat.SetFloat("_Pause", 1);
            sideLaneMat.SetFloat("_Pause", 1);
            return ret;
        }

        private void SetLaneMatScrollSpeed(float value)
        {
            const string c_PropertyName = "_uvScrollSpeedY";
#if UNITY_EDITOR
            if (!sideLaneMat.HasProperty(c_PropertyName)) {
                Debug.LogError("sideLaneMat is not found property.");
                return; 
            }
            if (!centerLaneMat.HasProperty(c_PropertyName))
            {
                Debug.LogError("centerLaneMat is not found property.");
                return;
            }
            if (value==0)
            {
                centerLaneMat.SetFloat("_Pause", 0);
                sideLaneMat.SetFloat("_Pause", 0);
            }
            //centerLaneMat.SetFloat("_Pause", );
#endif
            sideLaneMat.SetFloat(c_PropertyName, value);
            centerLaneMat.SetFloat(c_PropertyName, value);
        }
#endregion
#endif
    }
}
