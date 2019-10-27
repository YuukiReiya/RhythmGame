using System.Collections;
using UnityEngine;
using Yuuki;
using API.Util;
using Common;
namespace Game
{

    public class LoadController : SingletonMonoBehaviour<LoadController>
    {
        IEnumerator routine;

        // Start is called before the first frame update
        void Start()
        {
            routine = null;
            FadeController.Instance.FadeOut(Define.c_FadeTime);
        }

        // Update is called once per frame
        void Update()
        {
            if (routine == null)
            {
                while (GameMusic.Instance.Clip.loadState != AudioDataLoadState.Loaded)
                {
                    return;
                }
                routine = Translation();
                StartCoroutine(routine);
            }

        }

        IEnumerator Translation()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
            yield break;
        }
    }
}
