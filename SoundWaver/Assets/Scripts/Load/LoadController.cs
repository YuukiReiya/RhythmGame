using System.Collections;
using UnityEngine;
using Yuuki;
namespace Game
{

    public class LoadController : SingletonMonoBehaviour<LoadController>
    {
        IEnumerator routine;

        // Start is called before the first frame update
        void Start()
        {
            routine = null;
            NotesController.Instance.Setup(ChartManager.Instance.Chart);
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
