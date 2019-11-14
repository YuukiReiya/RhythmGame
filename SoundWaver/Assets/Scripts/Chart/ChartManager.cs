using Common;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using Yuuki;
public class ChartManager : SingletonMonoBehaviour<ChartManager>
{
    //serialize param
    [SerializeField] private GameObject prefab;
    [Header("NGUI")]
    [SerializeField] private UIGrid grid;
    [Header("Scroll")]
    [SerializeField] private UIScrollBar scrollBar;
    [System.Serializable]struct ScrollBarUI
    {
        public UITexture barTexture;
        public UITexture trgTexture;
        public UITexture frameTexture;
    }
    [SerializeField] private ScrollBarUI scrollBarUI;
    //private param
    uint number;
    SortType sortType = SortType.Hierarchy;
    bool isSortAsc = true;//並び順が昇順?
    public static Chart Chart { get; set; }
    //public param
    public enum SortType
    {
        Hierarchy,
        Name,
    }

    /// <summary>
    /// 楽曲(譜面)リストの表示
    /// </summary>
    public void LoadToDisplay()
    {
        //譜面番号の初期化
        number = 1;

        //並び替えの方法を指定
        grid.onCustomSort = (a, b) => { return ChartSort(a, b); };
        
        //譜面情報の取得
        var charts = Directory.GetFiles(Define.c_ChartSaveDirectory, "*" + Define.c_JSON);
        Yuuki.FileIO.FileIO fileIO = new Yuuki.FileIO.FileIO();
        foreach (var it in charts)
        {
            var chart = JsonUtility.FromJson<Chart>(fileIO.GetContents(it));
            Create(chart);
        }
        #region スクロールバー
        //フレームをトグルのアルファ値に合わせ、表示を合わせる
        {
            StartCoroutine(ScrollBarFrameAlphaSync());
        }
        #endregion
        //整列
        grid.Reposition();
        //スクロールバーを初期位置に
        scrollBar.value = 0;
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
        inst.name = chart.ResistName;
        proxy.SetupChart(chart, number++);
        return inst;
    }

    /// <summary>
    /// カスタムソート
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    int ChartSort(Transform a,Transform b)
    {
        int ret = 0;
        switch (sortType)
        {
            //ヒエラルキー順
            case SortType.Hierarchy:
                ret = a.GetSiblingIndex().CompareTo(b.GetSiblingIndex());
                break;
            //名前順
            case SortType.Name:
                ret = a.name.CompareTo(b.name);
                break;
            //エラーを吐いてデフォルト(ヒエラルキー)順にソート
            default:
                Debug.LogError("SortType is invlid value!");
                goto case SortType.Hierarchy;
        }
        //昇順/降順の判定に対応
        return isSortAsc ? ret : -ret;
    }

    /// <summary>
    /// スクロールバーのフレームのカラーをトグルに合わせる
    /// スクロールバーのフレームは別途同期させる必要があるため、専用の処理を追加
    /// </summary>
    /// <returns></returns>
    IEnumerator ScrollBarFrameAlphaSync()
    {
        //スクロールバーのバーとトグルのアルファ値はUpdateの後で更新されるため、
        //同期用にコルーチンで実装
        yield return new WaitForEndOfFrame();
        var cr = scrollBarUI.frameTexture.color;
        cr.a = scrollBarUI.barTexture.alpha;
        scrollBarUI.frameTexture.color = cr;
    }
}
