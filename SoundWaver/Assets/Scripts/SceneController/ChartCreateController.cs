using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using API.Util;
using UnityEngine.SceneManagement;
using Common;
using Game.UI;
namespace Scenes
{
    public class ChartCreateController : MonoBehaviour
    {
        //  serialize param
        [SerializeField] RadioButton dataPanelswitch;
        // Start is called before the first frame update
        void Start()
        {
            dataPanelswitch.CallDisable();
            FadeController.Instance.FadeOut(Define.c_FadeTime);
        }

        public void TransitionStart()
        {
            FadeController.Instance.EventQueue.Enqueue(() => { SceneManager.LoadScene("StartDev"); });
            FadeController.Instance.FadeIn(Define.c_FadeTime);
        }
    }
}