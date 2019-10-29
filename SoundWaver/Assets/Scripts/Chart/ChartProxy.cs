using Game;
using UnityEngine;
using API.Util;
using Common;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;
public class ChartProxy : MonoBehaviour
{
    //serialize param
    [SerializeField] private UILabel title;
    //private param
    private Chart chart;
    //public param

    private void Reset()
    {
        title = GetComponentInChildren<UILabel>();
    }
    public void SetupChart(Chart chart)
    {
        this.chart = chart;
        this.title.text = chart.ResistName;
    }
    #region ボタン処理
    public void OnTap()
    {
        this.StartCoroutine(MainRoutine());
        //FadeController.Instance.EventQueue.Enqueue(
        //    () =>
        //    {
        //        //this.StartCoroutine(GameMusic.Instance.LoadToAudioClip(chart.FilePath));
        //        //TODO:代入前に初期化したい?
        //        ChartManager.Chart = this.chart;
        //        SceneManager.LoadScene("Load");
        //    }
        //    );
        //FadeController.Instance.FadeIn(Define.c_FadeTime);
    }

    IEnumerator MainRoutine()
    {
        FadeController.Instance.EventQueue.Enqueue(
            () =>
            {
                ChartManager.Chart = this.chart;
                SceneManager.LoadScene("Load");
                //MEMO:シーン遷移間にコルーチンを回すので、DontDestroyObjectでStartCoroutineをする必要がある
                //GameMusic.Instance.StartCoroutine(GameMusic.Instance.LoadToAudioClip(chart.FilePath));
                GameMusic.Instance.StartCoroutine(TestRoutine());
            }
            );
        FadeController.Instance.FadeIn(Define.c_FadeTime);
        yield break;
    }
#if UNITY_EDITOR
    IEnumerator TestRoutine()
    {
        yield return new WaitForSeconds(2.0f);
        yield return GameMusic.Instance.LoadToAudioClip(chart.FilePath);
    }
#endif
#endregion
}
