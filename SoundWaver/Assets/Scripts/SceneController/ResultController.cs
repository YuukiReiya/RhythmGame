using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using API.Util;
using Common;
namespace Scenes
{
    public class ResultController : MonoBehaviour
    {
        //serialize param
        [SerializeField] private ResultScoreCanvas resultScoreCanvas;
        // Start is called before the first frame update
        void Start()
        {
            resultScoreCanvas.Setup();
            FadeController.Instance.FadeOut(Define.c_FadeTime);
        }

        public void TransitionSelect()
        {
            FadeController.Instance.EventQueue.Enqueue(() => { UnityEngine.SceneManagement.SceneManager.LoadScene("SelectDev"); });
            FadeController.Instance.FadeIn(Define.c_FadeTime);
        }
    }
}
