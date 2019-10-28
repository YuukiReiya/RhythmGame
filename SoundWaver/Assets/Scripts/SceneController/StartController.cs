using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Scenes
{
    public class StartController : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            API.Util.FadeController.Instance.FadeOut(Common.Define.c_FadeTime);
        }

        public void TransitionSelect()
        {
            API.Util.FadeController.Instance.EventQueue.Enqueue(() => { SceneManager.LoadScene("Select"); });
            API.Util.FadeController.Instance.FadeIn(Common.Define.c_FadeTime);
        }
        public void TransitionChartCreate()
        {
            API.Util.FadeController.Instance.EventQueue.Enqueue(() => { SceneManager.LoadScene("ChartCreate"); });
            API.Util.FadeController.Instance.FadeIn(Common.Define.c_FadeTime);
        }
    }
}