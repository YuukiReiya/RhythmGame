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
    [System.NonSerialized]
    public Color color;
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
        ChartManager.Chart = this.chart;
        //選択した譜面に更新
        ChartManager.Instance.SetImageEffectColor(color);
        ChartManager.Instance.UpdateChartPanel();
    }
#endregion
}
