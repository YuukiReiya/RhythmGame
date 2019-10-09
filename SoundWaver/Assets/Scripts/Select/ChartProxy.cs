using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

public class ChartProxy : MonoBehaviour
{
    //serialize param
    [SerializeField] UILabel title;
    //private param
    Chart chart;
    //public param

    private void Reset()
    {
        title = GetComponentInChildren<UILabel>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupChart(Chart chart)
    {
        this.chart = chart;
        title.text = chart.Title;
    }
    #region ボタン処理
    public void OnTap()
    {
        this.StartCoroutine(GameMusic.Instance.LoadToAudioClip(chart.FilePath));
    }
    #endregion
}
