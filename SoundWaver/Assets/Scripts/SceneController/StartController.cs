#define DEBUG_MODE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Common;
using API.Util;
using Game.UI;
using Yuuki.MethodExpansions;
using UnityEngine.Networking;
using Game.Audio;
#if DEBUG_MODE
using System.IO;
using Yuuki.FileIO;
using Game;
#endif
namespace Scenes
{
    public class StartController : MonoBehaviour
    {
        //  serialize param
        [Header("Tap to Screen")]
        [SerializeField] private RadioButton tapToScreenButton;
        [SerializeField] private UIWidget widget;
        [SerializeField] private AnimationCurve blinkCurve;
        [Header("Current develop version")]
        [SerializeField] private UILabel versionLabel;
#if DEBUG_MODE
        [Header("Debug")]
        [SerializeField] private GameObject debugObj;
        [SerializeField] private int openTapCount = 3;
        [SerializeField] private UILabel guiSpeedLabel;
        [SerializeField] private UILabel realSpeedLabel;
#endif
        [Header("Sound")]
        [SerializeField] private AudioClipList audioClipTable;
        //  private param
        private RadioButton menuSwitch;
        private IEnumerator blinkRoutine;
#if DEBUG_MODE
        private int tapCount = 0;
#endif
        //  const param
        private const float c_TransitionSoundFadeTime = 1.0f;
        private const string c_VersionInfo =
#if UNITY_EDITOR
            "D";
#else
            "M";
#endif

        // Start is called before the first frame update
        void Start()
        {
#if DEBUG_MODE
            debugObj.SetActive(false);
            tapCount = 0;
#endif
            if (tapToScreenButton.IsActive)
            {
                tapToScreenButton.CallDisable();
            }

            if(!TryGetComponent(out menuSwitch))
            {
                Exit();
                return;
            }
            // tap to screen の点滅開始命令
            menuSwitch.onDisableFunc += () =>
            {
                SetupBlink();
                if (blinkRoutine != null)
                {
                    StopCoroutine(blinkRoutine);
                    blinkRoutine = null;
                }
                blinkRoutine = WidgetBlinkRoutine();
                this.StartCoroutine(blinkRoutine, () => { blinkRoutine = null; });
            };
            //停止処理
            menuSwitch.onActiveFunc += () =>
            {
                if (blinkRoutine != null)
                {
                    StopCoroutine(blinkRoutine);
                    blinkRoutine = null;
                }
                SetupBlink();
            };
            //バージョン情報
            versionLabel.text = c_VersionInfo + Application.version;
            menuSwitch.CallDisable();

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
        /// セレクトシーンへ遷移
        /// </summary>
        public void TransitionSelect()
        {
            var audio = AudioManager.Instance;
            //SE
            audio.PlaySE("Transition");
            //フェード
            FadeController.Instance.EventQueue.Enqueue(
                () => 
                {
                    SceneManager.LoadScene("SelectDev");
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

        /// <summary>
        /// 譜面作成シーンへ遷移
        /// </summary>
        public void TransitionChartCreate()
        {
            var audio = AudioManager.Instance;
            //SE
            audio.PlaySE("Transition");
            FadeController.Instance.EventQueue.Enqueue(
                () =>
                {
                    SceneManager.LoadScene("ChartCreateDev");
                    audio.SourceBGM.Stop();
                    audio.SourceSE.Stop();
                });
            FadeController.Instance.FadeIn(Define.c_FadeTime);
            audio.FadeBGM(
                c_TransitionSoundFadeTime,
                audio.GetConvertVolume(audio.BGMVolume),
                0
                );
            audio.FadeSE(
                c_TransitionSoundFadeTime,
                audio.GetConvertVolume(audio.SEVolume), 
                0);
        }

        /// <summary>
        /// クレジット画面へ遷移
        /// </summary>
        public void TransitionCredit()
        {
            var audio = AudioManager.Instance;
            //SE
            audio.PlaySE("Transition");
            FadeController.Instance.EventQueue.Enqueue(
                () =>
                {
                    SceneManager.LoadScene("Credit");
                    audio.SourceBGM.Stop();
                    audio.SourceSE.Stop();
                });
            FadeController.Instance.FadeIn(Define.c_FadeTime);
            audio.FadeBGM(
                c_TransitionSoundFadeTime,
                audio.GetConvertVolume(audio.BGMVolume),
                0
                );
            audio.FadeSE(
                c_TransitionSoundFadeTime,
                audio.GetConvertVolume(audio.SEVolume),
                0);
        }

        public void Exit()
        {
            AudioManager.Instance.PlaySE("Transition");
#if UNITY_EDITOR
            Debug.LogError("Exit Game.");
            return;
#endif
            Application.Quit();
        }

        private void SetupBlink()
        {
            widget.alpha = blinkCurve.Evaluate(0);
        }

        private IEnumerator WidgetBlinkRoutine()
        {
            while (true)
            {
                var ratio = blinkCurve.Evaluate(Time.time);
                widget.alpha = Mathf.Clamp01(ratio);
                yield return null;
            }
        }

        public void CacheClear()
        {
            AudioManager.Instance.PlaySE("CacheClear");
            DialogController.Instance.Open(
                "キャッシュをクリアします。\nよろしいですか?",
                () =>
                {
                    this.StartCoroutine(
                        CreateSystemFileRoutine(),
                        ()=>
                        {
                            //ローカル
                            //TODO:リファイン必須
                            IEnumerator UpdateVolume()
                            {
                                var io = new FileIO();
                                var ini = JsonUtility.FromJson<IniFile>(io.GetContents(Define.c_SettingFilePath));
                                var audio = AudioManager.Instance;
                                float fadeTime = 1.0f;
                                audio.BGMVolume = ini.BGMVol;
                                audio.SEVolume = ini.SEVol;
                                audio.FadeBGM(
                                    fadeTime,
                                    audio.SourceBGM.volume,
                                    audio.GetConvertVolume(ini.BGMVol)
                                    );
                                audio.FadeSE(
                                    fadeTime,
                                    audio.SourceSE.volume,
                                    audio.GetConvertVolume(ini.SEVol)
                                    );
                                yield return null;
                            }
                            this.StartCoroutine(
                                UpdateVolume(),
                                () =>
                                {
                                    DialogController.Instance.Open("キャッシュをクリアしました。");
                                });
                        }
                        );
                }, null
                );
        }

        private IEnumerator CreateSystemFileRoutine()
        {
            FileIO io = new FileIO();
            IniFile ini;
            do
            {
                //  上書き生成
                ini = new IniFile();
                ini.Setup();
                io.CreateFile(
                    Define.c_SettingFilePath,
                    JsonUtility.ToJson(ini),
                    FileIO.FileIODesc.Overwrite
                    );
                yield return null;

                //  値が初期値になっているか判定
                if (File.Exists(Define.c_SettingFilePath))
                {
                    using (var request = UnityWebRequest.Get(Define.c_LocalFilePath + Define.c_SettingFilePath))
                    {
                        yield return request.SendWebRequest();
                        if (request.isNetworkError || request.isHttpError)
                        {
                            Debug.LogError("StartController.cs line144 UnityWebRequest\n" + request.error);
                            ErrorManager.Save();
                        }
                        ini = JsonUtility.FromJson<IniFile>(request.downloadHandler.text);
                        bool ret = false;
                        if (ini.CurrentPath != Define.c_InitialCurrentPath) { continue; }
                        if(ini.BGMVol != Define.c_InitialVol) { continue; }
                        if(ini.SEVol != Define.c_InitialVol) { continue; }
                        if(ini.NotesSpeed != Define.c_InitialNotesSpeed) { continue; }
                        if (ini.NotesSpeedList.Length != Define.c_NotesSpeedList.Length) { continue; }
                        for(int i=0;i<Define.c_NotesSpeedList.Length;++i)
                        {
                            if (ini.NotesSpeedList[i] != Define.c_NotesSpeedList[i].Item3) 
                            {
                                ret = false;
                                break;
                            }
                            ret = true;
                        }
                        //  値が初期値
                        if (ret) { break; }
                    }
                }
            }
            while (true);
            yield break;
        }

#if DEBUG_MODE
        public void DebugAction()
        {
            //close
            if (debugObj.activeSelf)
            {
                tapCount = 0;
                debugObj.SetActive(false);
                return;
            }

            //open
            tapCount++;
            if (tapCount < openTapCount) { return; }

            debugObj.SetActive(true);
        }
        public void DebugExecute()
        {
            //値が入っていなければ処理しない
            if (guiSpeedLabel.text == string.Empty || realSpeedLabel.text == string.Empty)
            {
                return;
            }
            //guiチェック
            var gui = uint.Parse(guiSpeedLabel.text);
            //if (!Define.c_NotesSpeedList.Select(it=>it.Item2).Contains(gui)) { return; }
            if (gui < Define.c_MinNoteSpeed || Define.c_MaxNoteSpeed < gui) { return; }

            //realチェック
            var real = float.Parse(realSpeedLabel.text);

            IniFile ini;
            var io = new FileIO();
            if (!File.Exists(Define.c_SettingFilePath))
            {
                //作る
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
            (uint, uint, float) tuple = Define.c_NotesSpeedList[Define.c_InitialNotesSpeed];
            foreach (var it in Define.c_NotesSpeedList)
            {
                if (it.Item2 == gui)
                {
                    tuple = it;
                    ini.NotesSpeedList[tuple.Item1] = real;
                    //ファイルを上書きモードで保存
                    io.CreateFile(
                     Define.c_SettingFilePath,
                     JsonUtility.ToJson(ini),
                     FileIO.FileIODesc.Overwrite
                     );
                    DialogController.Instance.Open("設定ファイルの値を書き換えました\nGUI:" + gui + "\nReal:" + real);
                    return;
                }
            }
            DialogController.Instance.Open("エラー。\n書き込みに失敗。");
        }
#endif

        public void OnTapToScreenSE()
        {
            string key = "TapToScreen_Forward";
            if(!tapToScreenButton.IsActive)
            {
                //初期画面(ボタンが押されていない状態)の時はキーの上書き
                key = "TapToScreen_Back";
            }
            AudioManager.Instance.PlaySE(key);
        }

#if UNITY_EDITOR
        [ContextMenu("Reset audio clip list")]
        private void ResetAudioClipTable()
        {
            //audioClipTable.Table.
            AudioManager.Instance.clips = audioClipTable.Table;
        }
#endif
    }
}