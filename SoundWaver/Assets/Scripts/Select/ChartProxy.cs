using Game;
using UnityEngine;

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
        this.StartCoroutine(GameMusic.Instance.LoadToAudioClip(chart.FilePath));
        ChartManager.Instance.Chart = this.chart;
    }
    #endregion
}
