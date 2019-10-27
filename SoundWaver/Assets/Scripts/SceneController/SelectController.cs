using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yuuki;
using API.Util;
namespace Game
{
    public class SelectController : MonoBehaviour
    {

        // Start is called before the first frame update
        void Start()
        {
            //楽曲リストの表示
            FadeController.Instance.FadeOut(Common.Define.c_FadeTime);
            ChartManager.Instance.LoadToDisplay();
        }

        public void TransitionGame()
        {
            FadeController.Instance.EventQueue.Enqueue(() => { SceneManager.LoadScene("Load"); });
            FadeController.Instance.FadeIn(Common.Define.c_FadeTime);
        }

        public void TransitionStart()
        {
            FadeController.Instance.EventQueue.Enqueue(() => { SceneManager.LoadScene("Start"); });
            FadeController.Instance.FadeIn(Common.Define.c_FadeTime);
        }
    }
}