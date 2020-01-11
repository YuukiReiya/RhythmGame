#if UNITY_EDITOR
#define VOL_MAX
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Game.Audio;
using Common;
using API.Util;
namespace Scenes
{
    public class CreditController : MonoBehaviour
    {
        //serialize param
        [Header("Sound")]
        [SerializeField] AudioClipList audioClipTable;
        [Header("Camera")]
        [SerializeField] Camera camera2D;
        //private param
        private bool wasTransition;
        //const param
        // Start is called before the first frame update
        void Start()
        {
#if UNITY_EDITOR
            FixedAspectRatio.Setup(Define.c_FixedResolutionWidth, Define.c_FixedResolutionHeight);
#if VOL_MAX
            AudioManager.Instance.SEVolume = Define.c_MaxVolume;
            AudioManager.Instance.BGMVolume = Define.c_MaxVolume;
#endif
#endif
            FixedAspectRatio.FitToWidth2D(camera2D);
            //サウンドテーブル更新
            AudioManager.Instance.clips = audioClipTable.Table;

            //初期化
            wasTransition = false;

            //SE音量
            AudioManager.Instance.FadeSE(
                Define.c_FadeTime,
                0,
#if VOL_MAX
                1
#else
                AudioManager.Instance.GetConvertVolume(AudioManager.Instance.SEVolume)
#endif
                );

            //BGM再生
            AudioManager.Instance.FadeBGM(
                Define.c_FadeTime,
                0,
#if VOL_MAX
                1
#else
                AudioManager.Instance.GetConvertVolume(AudioManager.Instance.SEVolume)
#endif
                );
            AudioManager.Instance.PlayBGM("BGM");

            //フェード
            FadeController.Instance.FadeOut(Define.c_FadeTime);
        }

        /// <summary>
        /// スタート画面へ遷移
        /// </summary>
        public void TransitionStart()
        {
            if (wasTransition) { return; }
            wasTransition = true;
            var audio = AudioManager.Instance;
            //SE
            audio.PlaySEEx(
                "Transition",
                false,
                () =>
                {
                    //BGMフェード
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
                    FadeController.Instance.FadeIn(Define.c_FadeTime);
                });
            //フェード
            FadeController.Instance.EventQueue.Enqueue(
                () =>
                {
                    SceneManager.LoadScene("StartDev");
                });
        }
    }
}