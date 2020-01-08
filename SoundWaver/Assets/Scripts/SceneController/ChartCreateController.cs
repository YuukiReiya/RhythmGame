using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using API.Util;
using UnityEngine.SceneManagement;
using Common;
using Game.UI;
using Game.Audio;
namespace Scenes
{
    //TODO:デフォルトBGMを用意して、譜面製作中以外でもBGMを流す。
    public class ChartCreateController : MonoBehaviour
    {
        //  serialize param
        [SerializeField] private RadioButton dataPanelswitch;
        [Header("Sound")]
        [SerializeField]private AudioClipList audioClipTable;
        [Header("Camera")]
        [SerializeField] private Camera camera2D;
        //private const param
        private const float c_TransitionSoundFadeTime = 1.0f;
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
            
            //概要‐詳細 切り替え
            dataPanelswitch.CallDisable();

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

            //フェード
            FadeController.Instance.FadeOut(Define.c_FadeTime);
        }

        public void TransitionStart()
        {
            var audio = AudioManager.Instance;
            //SE
            audio.PlaySE("Transition");
            FadeController.Instance.EventQueue.Enqueue(
                () =>
                {
                    SceneManager.LoadScene("StartDev");
                    audio.SourceBGM.Stop();
                    audio.SourceSE.Stop();
                }); 
            FadeController.Instance.FadeIn(Define.c_FadeTime);
            //BGMフェード
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