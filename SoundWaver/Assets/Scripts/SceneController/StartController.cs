using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Common;
using API.Util;
using Game.UI;
using Yuuki.MethodExpansions;
namespace Scenes
{
    public class StartController : MonoBehaviour
    {
        //  serialize param
        [Header("Tap to Screen")]
        [SerializeField] private UIWidget widget;
        [SerializeField] private AnimationCurve blinkCurve;
        //  private param
        private RadioButton menuSwitch;
        private IEnumerator blinkRoutine;

        // Start is called before the first frame update
        void Start()
        {
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
    }
}