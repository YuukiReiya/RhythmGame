using Common;
using System.IO;
using UnityEngine;
using Yuuki;
public class ChartManager : SingletonMonoBehaviour<ChartManager>
{
    //serialize param
    [SerializeField] private GameObject prefab;
    [SerializeField] private UIGrid grid;
    //private param
    public Chart Chart { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        //譜面リスト表示
        Display();
    }

    /// <summary>
    /// 楽曲(譜面)リストの表示
    /// </summary>
    void Display()
    {
        //譜面情報の取得
        var charts = Directory.GetFiles(Define.c_ChartSaveDirectory, "*" + Define.c_JSON);
        Yuuki.FileIO.FileIO fileIO = new Yuuki.FileIO.FileIO();
        foreach (var it in charts)
        {
            var chart = JsonUtility.FromJson<Chart>(fileIO.GetContents(it));
            Create(chart);
        }
        //整列
        grid.Reposition();
    }

    /// <summary>
    /// 譜面プレハブの作成
    /// </summary>
    /// <param name="chart"></param>
    /// <returns></returns>
    GameObject Create(Chart chart)
    {
        var inst = Instantiate(prefab);
        inst.transform.parent = grid.transform;
        inst.transform.localScale = prefab.transform.localScale;
        ChartProxy proxy;
        if (!inst.TryGetComponent<ChartProxy>(out proxy))
        {
            Destroy(inst);
            return null;
        }
        proxy.SetupChart(chart);
        return inst;
    }
}
