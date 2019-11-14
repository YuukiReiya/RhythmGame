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
    public UILabel number;
    private void Reset()
    {
        title = GetComponentInChildren<UILabel>();
    }
    public void SetupChart(Chart chart,uint number)
    {
        this.chart = chart;
        this.title.text = chart.ResistName;
        this.number.text = number.ToString();
    }
    #region ボタン処理
    public void OnTap()
    {
        this.StartCoroutine(MainRoutine());
    }

    IEnumerator MainRoutine()
    {
        FadeController.Instance.EventQueue.Enqueue(
            () =>
            {
                ChartManager.Chart = this.chart;
                SceneManager.LoadScene("Load");
                //MEMO:シーン遷移間にコルーチンを回すので、DontDestroyObjectでStartCoroutineをする必要がある
                GameMusic.Instance.StartCoroutine(GameMusic.Instance.LoadToAudioClip(chart.FilePath));
            }
            );
        FadeController.Instance.FadeIn(Define.c_FadeTime);
        yield break;
    }
#endregion
}
