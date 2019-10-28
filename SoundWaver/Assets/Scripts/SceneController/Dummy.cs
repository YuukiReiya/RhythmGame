using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using API.Util;
using UnityEngine.SceneManagement;
namespace Game.Setup
{
    public class Dummy : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            SceneManager.LoadScene("Start");
            //FadeController.Instance.EventQueue.Enqueue(() => { CallbackRun(); });
        }
        void CallbackRun()
        {
            SceneManager.LoadScene("Start");
        }
    }
}