using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using API.Util;
using UnityEngine.SceneManagement;
using Common;
namespace Scenes
{
    public class ChartCreateController : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            FadeController.Instance.FadeOut(Define.c_FadeTime);
        }

        public void TransitionStart()
        {
            FadeController.Instance.EventQueue.Enqueue(() => { SceneManager.LoadScene("Start"); });
            FadeController.Instance.FadeIn(Define.c_FadeTime);
        }
    }
}