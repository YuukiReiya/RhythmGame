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

        //const param
        private const float c_TransitionSoundFadeTime = 1.0f;
        // Start is called before the first frame update
        void Start()
        {
            //サウンドテーブル更新
            AudioManager.Instance.clips = audioClipTable.Table;

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

            //フェード
            FadeController.Instance.FadeOut(Define.c_FadeTime);
        }

        /// <summary>
        /// スタート画面へ遷移
        /// </summary>
        public void TransitionStart()
        {
            var audio = AudioManager.Instance;
            //SE
            audio.PlaySE("Transition");
            //フェード
            FadeController.Instance.EventQueue.Enqueue(
                () =>
                {
                    SceneManager.LoadScene("StartDev");
                    audio.SourceBGM.Stop();
                    audio.SourceSE.Stop();
                });
            FadeController.Instance.FadeIn(Define.c_FadeTime);
            //BGMフェード
            //MEMO:場合によってはフェードキューの中でもいいかも
            audio.FadeBGM(
                c_TransitionSoundFadeTime,
                audio.GetConvertVolume(audio.BGMVolume),
                0
                );
            //SEフェード
            audio.FadeSE(
                c_TransitionSoundFadeTime,
                audio.GetConvertVolume(audio.SEVolume),
                0
                );
        }
    }
}