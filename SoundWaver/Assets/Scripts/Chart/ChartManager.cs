using Common;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Yuuki;
public class ChartManager : SingletonMonoBehaviour<ChartManager>
{
    //serialize param
    [SerializeField] private GameObject prefab;
    [SerializeField] private UIGrid grid;
    //private param
    public static Chart Chart { get; set; }

    /// <summary>
    /// 楽曲(譜面)リストの表示
    /// </summary>
    public void LoadToDisplay()
    {
        //PresetMusicLoad();
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
    /// 既存(プリセット)の楽曲ファイルの表示
    /// </summary>
    private void PresetMusicLoad()
    {
        //var path = System.IO.Path.Combine(Application.streamingAssetsPath, "test.txt");
        //var path = System.IO.Path.Combine(Application, "test.txt");
        var path = Define.c_PresetFilePath[0].Item2;
        StartCoroutine(get(path));
    }
    System.Collections.IEnumerator get(string filePath)
    {
        var www = UnityWebRequest.Get(filePath);
        yield return www.SendWebRequest();
        var ret = www.downloadHandler.text;
        DialogController.Instance.Open(ret);
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
