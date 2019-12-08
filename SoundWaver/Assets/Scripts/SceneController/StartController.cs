using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Common;
using API.Util;

namespace Scenes
{
    public class StartController : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            FadeController.Instance.FadeOut(Define.c_FadeTime);
        }

        public void TransitionSelect()
        {
            FadeController.Instance.EventQueue.Enqueue(() => { SceneManager.LoadScene("SelectDev"); });
            FadeController.Instance.FadeIn(Define.c_FadeTime);
        }
        public void TransitionChartCreate()
        {
            FadeController.Instance.EventQueue.Enqueue(() => { SceneManager.LoadScene("ChartCreate"); });
            FadeController.Instance.FadeIn(Define.c_FadeTime);
        }
    }
}