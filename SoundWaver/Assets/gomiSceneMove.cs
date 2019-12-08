using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gomiSceneMove : MonoBehaviour
{
    public void OnGomi()
    {
        API.Util.FadeController.Instance.EventQueue.Enqueue(
 () =>
 {
     UnityEngine.SceneManagement.SceneManager.LoadScene("Load");
                 //MEMO:シーン遷移間にコルーチンを回すので、DontDestroyObjectでStartCoroutineをする必要がある
                 var chart = ChartManager.Chart;
     Game.GameMusic.Instance.StartCoroutine(Game.GameMusic.Instance.LoadToAudioClip(chart.MusicFilePath));
 }
);
        API.Util.FadeController.Instance.FadeIn(Common.Define.c_FadeTime);

    }

    public Transform a;
    public Transform b;
    [ContextMenu("hoge")]
    public void hoge()
    {
        Debug.Log(
            "x:" + (a.position.x - b.position.x) + "\n" +
            "y:" + (a.position.y - b.position.y) + "\n" +
            "z:" + (a.position.z - b.position.z));
    }
}
