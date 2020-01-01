#define DEBUG_MODE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Common;
using API.Util;
using Game.UI;
using Yuuki.MethodExpansions;
#if DEBUG_MODE
using System.Linq;
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
        //  private param
        private RadioButton menuSwitch;
        private IEnumerator blinkRoutine;
#if DEBUG_MODE
        private int tapCount = 0;
#endif
        //  const param
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
            FadeController.Instance.FadeOut(Define.c_FadeTime);
        }

        public void TransitionSelect()
        {
            FadeController.Instance.EventQueue.Enqueue(() => { SceneManager.LoadScene("SelectDev"); });
            FadeController.Instance.FadeIn(Define.c_FadeTime);
        }
        public void TransitionChartCreate()
        {
            FadeController.Instance.EventQueue.Enqueue(() => { SceneManager.LoadScene("ChartCreateDev"); });
            FadeController.Instance.FadeIn(Define.c_FadeTime);
        }

        public void Exit()
        {
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
            if (!Define.c_NotesSpeedList.Select(it=>it.Item2).Contains(gui)) { return; }
            //if (gui < Define.c_MinNoteSpeed || Define.c_MaxNoteSpeed < gui) { return; }
            DialogController.Instance.Open(
                "設定ファイルの値を書き換えました",
                () =>
                {
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
                    var index = Define.c_NotesSpeedList.First(it => it.Item2 == gui).Item1;
                    ini.NotesSpeedList[index] = real;
                    //ファイルを上書きモードで保存
                    io.CreateFile(
                     Define.c_SettingFilePath,
                     JsonUtility.ToJson(ini),
                     FileIO.FileIODesc.Overwrite
                     );
                }
                );
        }
#endif
    }
}