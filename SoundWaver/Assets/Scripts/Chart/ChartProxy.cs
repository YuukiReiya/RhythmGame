using Game;
using UnityEngine;
using API.Util;
using Common;
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
        this.title.text = chart.Title;
    }
    #region ボタン処理
    public void OnTap()
    {
        FadeController.Instance.EventQueue.Enqueue(
            () =>
            {
                this.StartCoroutine(GameMusic.Instance.LoadToAudioClip(chart.FilePath));
                //TODO:代入前に初期化したい?
                ChartManager.Chart = this.chart;
                SceneManager.LoadScene("Load");
            }
            );
        FadeController.Instance.FadeIn(Define.c_FadeTime);
    }
    #endregion
}
