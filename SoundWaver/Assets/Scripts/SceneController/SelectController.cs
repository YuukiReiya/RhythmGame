using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using API.Util;
using Common;
using Game.UI;
using Game.Audio;
using Yuuki.MethodExpansions;
namespace Game
{
    public class SelectController : MonoBehaviour
    {
        //serialize param
        [SerializeField] private GameObject refinePanel;
        [SerializeField] private ChartDeleter deletePanel;
        [Header("Sound")]
        [SerializeField] private AudioClipList audioClipTable;
        [Header("Camera")]
        [SerializeField] private Camera camera2D;
        //privtae param
        private bool wasTransition;
        //public param
        //const param
        const float c_TransitionGameSoundFade = 1.0f;
        // Start is called before the first frame update
        void Start()
        {
#if UNITY_EDITOR
            FixedAspectRatio.Setup(Define.c_FixedResolutionWidth, Define.c_FixedResolutionHeight);
#endif
           //アスペクト比変更
            FixedAspectRatio.FitToWidth2D(camera2D);

            //サウンドテーブル更新
            AudioManager.Instance.clips = audioClipTable.Table;

            //初期化
            wasTransition = false;
            ChartManager.Instance.Setup();

#if UNITY_EDITOR
            //MEMO:SEの再生を行うのでフェード中に音が聞こえてしまう。
            //回避するために関数を使用せずにアクティブの切り替えを行うか、最初から切っとかないといけない

            if (refinePanel.activeSelf)
            {
                Debug.LogError("SelectController.cs line39 実行ファイル書き出し前にインスペクター上でアクティブの制御を行う必要あり");

                //絞り込みパネルの非表示
                CloseRefine();
            }
            if (deletePanel.gameObject.activeSelf)
            {
                Debug.LogError("SelectController.cs line46 実行ファイル書き出し前にインスペクター上でアクティブの制御を行う必要あり");

                //削除パネルの非表示
                CloseDelete();
            }
#endif

            //楽曲リストの表示
            ChartManager.Instance.LoadToDisplay();

            //SE音量
            AudioManager.Instance.FadeSE(
                Define.c_FadeTime,
                0,
                AudioManager.Instance.GetConvertVolume(AudioManager.Instance.SEVolume)
                );

            //BGM再生
            AudioManager.Instance.FadeBGM(
                Define.c_FadeTime,
                0,
                AudioManager.Instance.GetConvertVolume(AudioManager.Instance.BGMVolume)
                );
            AudioManager.Instance.PlayBGM("BGM");


            FadeController.Instance.FadeOut(Define.c_FadeTime);
        }

        public void TransitionStart()
        {
            if (wasTransition) { return; }
            wasTransition = true;
            var audio = AudioManager.Instance;
            //SE
            audio.PlaySEEx(
                "Return",
                false,
                () =>
                {
                    //BGMフェード
                    //MEMO:場合によってはフェードキューの中でもいいかも
                    audio.FadeBGM(
                        Define.c_FadeTime,
                        audio.GetConvertVolume(audio.BGMVolume),
                        0
                        );
                    //SEフェード
                    audio.FadeSE(
                        Define.c_FadeTime,
                        audio.GetConvertVolume(audio.SEVolume),
                        0
                        );
                    //フェードの開始命令
                    FadeController.Instance.FadeIn(Define.c_FadeTime);
                });
            FadeController.Instance.EventQueue.Enqueue(
                () =>
                {
                    SceneManager.LoadScene("StartDev");
                });
        }

        public void OpenRefine()
        {
            AudioManager.Instance.PlaySE("Open");
            refinePanel.SetActive(true);
        }

        public void CloseRefine()
        {
            AudioManager.Instance.PlaySE("Close");
            refinePanel.SetActive(false);
        }

        public void OpenDelete()
        {
            AudioManager.Instance.PlaySE("Open");
            deletePanel.gameObject.SetActive(true);
        }

        public void CloseDelete()
        {
            AudioManager.Instance.PlaySE("Close");
            deletePanel.gameObject.SetActive(false);
            deletePanel.DestroyScrollChildren();
        }

        /// <summary>
        /// ゲーム開始
        /// </summary>
        public void Play()
        {
            if (wasTransition) { return; }
            var audio = AudioManager.Instance;
            //SE
            audio.PlaySE("Submit");
            //フェード
            FadeController.Instance.EventQueue.Enqueue(
                () =>
                {
                    audio.SourceBGM.Stop();
                    audio.SourceSE.Stop();
                    SceneManager.LoadScene("Load");
                    //MEMO:シーン遷移間にコルーチンを回すので、DontDestroyObjectでStartCoroutineをする必要がある
                    var chart = ChartManager.Chart;
                    GameMusic.Instance.StartCoroutine(GameMusic.Instance.LoadToAudioClip(chart.MusicFilePath));
                });
            //BGMフェード
            audio.FadeBGM(
                c_TransitionGameSoundFade,
                audio.GetConvertVolume(audio.BGMVolume),
                0
                );
            //SEフェード
            audio.FadeSE(
                c_TransitionGameSoundFade,
                audio.GetConvertVolume(audio.SEVolume),
                0,
                () =>
                {
                    FadeController.Instance.FadeIn(Define.c_FadeTime);
                }
                );
            wasTransition = true;
        }
    }
}