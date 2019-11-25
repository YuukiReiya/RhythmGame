using Common;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using Yuuki;
using System.Linq;
using Game.UI;
using DG.Tweening;

public class ChartManager : SingletonMonoBehaviour<ChartManager>
{
    //serialize param
    [SerializeField] private GameObject prefab;
    [Header("NGUI")]
    [SerializeField] private UIGrid grid;
    [Header("Scroll")]
    [SerializeField] private UIScrollBar scrollBar;
    [SerializeField]private float tweenSpeed=0.3f;
    [System.Serializable] struct ScrollBarUI
    {
        public UITexture barTexture;
        public UITexture trgTexture;
        public UITexture frameTexture;
    }
    [SerializeField] private ScrollBarUI scrollBarUI;

    //private param
    private uint number;
    private SortType sortType = SortType.Hierarchy;
    private ChartType chartType = ChartType.All;
    private bool isSortAsc = true;//並び順が昇順?

    //public param
    public static Chart Chart { get; set; }
    public enum SortType
    {
        Hierarchy,
        Name,
    }
    public enum ChartType
    {
        All,//全て
        Preset,//プリセットのみ
        Original,//オリジナルのみ
    }
    [Header("Refine param")]
    public RadioButtonGroop chartGroop;
    public RadioButtonGroop sortGroop;
    public RadioButtonGroop orderGroop;

    /// <summary>
    /// 楽曲(譜面)リストの表示
    /// </summary>
    public void LoadToDisplay()
    {
        //子が削除されてなければ削除
        if (grid.GetChildList().Count > 0) { DestroyCharts(); }

        //譜面番号の初期化
        number = 1;

        //並び替えの方法を指定
        grid.onCustomSort = (a, b) => { return ChartSort(a, b); };

        //譜面情報の取得
        CreateCharts();

        #region スクロールバー関連
        {
            //フレームをトグルのアルファ値に合わせ、表示を合わせる
            StartCoroutine(ScrollBarFrameAlphaSync());
            //スクロールバーを初期位置に戻す
            StartCoroutine(ScrollBarValueResetZero());
        }
        #endregion

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
    int ChartSort(Transform a, Transform b)
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

    void CreateCharts()
    {
        //譜面ファイルのパス格納配列
        string[] charts = null;

        //全取得
        charts = Directory.GetFiles(Define.c_ChartSaveDirectory, "*" + Define.c_JSON);

        switch (chartType)
        {
            //全て
            case ChartType.All:break;
            //プリセットのみ
            case ChartType.Preset:
                {
                    //プリセットファイルの譜面名を取得
                    var presetChartsName = Define.c_PresetFilePath.Select(it => Path.GetFileName(it.Item2));
                    //取得した譜面名のなかからプリセットのもののみを取得
                    charts = charts.Where(it => presetChartsName.Contains(Path.GetFileName(it))).ToArray();
                }
                break;
            //オリジナルのみ
            case ChartType.Original:
                {
                    //プリセットファイルの譜面名を取得
                    var presetChartsName = Define.c_PresetFilePath.Select(it => Path.GetFileName(it.Item2));
                    //取得した譜面名のなかからプリセットでないものを取得
                    charts = charts.Where(it => !presetChartsName.Contains(Path.GetFileName(it))).ToArray();
                }
                break;
            //エラーを吐いてデフォルト(全て)作成
            default:
                Debug.LogError("ChartType is invlid value!");
                goto case ChartType.All;
        }

        //作成
        Yuuki.FileIO.FileIO fileIO = new Yuuki.FileIO.FileIO();
        foreach (var it in charts)
        {
            var chart = JsonUtility.FromJson<Chart>(fileIO.GetContents(it));
            Create(chart);
        }

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

    /// <summary>
    /// スクロールバーのvalueをゼロに初期化
    /// </summary>
    /// <returns></returns>
    IEnumerator ScrollBarValueResetZero()
    {
        //スクロールバーの"value"を"0"に初期化する
        //"LoadToDisplay"内に置いていたが、初回呼び出し時のみ値が"0.7..."とおかしくなるため別に同期を実装する羽目に。
        //ちなみに"EndOfFrame","null"の場合強制的に戻しているのが見えてしまうので"FixedUpdate"の後という謎のタイミングで呼び出している。
        // yield return new WaitForFixedUpdate();
        //yield return null;
        scrollBar.value = 1;
        int w = 3;
        for(int c = 0; c < w; ++c) { yield return null; }
        //yield return new WaitForEndOfFrame();
        DOTween.To(
            () => scrollBar.value,
            v => scrollBar.value = v,
            0.0f,
            tweenSpeed);
        //yield return new WaitUntil(() => { return scrollBar.value != 0; });
        //yield return null;
        yield break;
        //scrollBar.value = 0;
    }

    /// <summary>
    /// 絞り込み情報の更新
    /// 
    /// enumの変数をキャストして使っているので、インスペクター上で設定した
    /// "Item"構造体の"element"番号とenumの数値が違っていたらおかしくなるので注意！！
    /// (↑読み取り専用のタプルで作ると、結局キャストでよくね？となったので結果この形に落ち着いた。)
    /// </summary>
    public void UpdateRefine()
    {
        //楽曲タイプ
        for (uint i = 0; i < chartGroop.Buttons.Length; ++i)
        {
            var item = chartGroop.Buttons[i];
            if (chartGroop.isActiveButton != item) { continue; }
            chartType = (ChartType)i;//キャスト
        }
        //並び順
        for (uint i = 0; i < sortGroop.Buttons.Length; ++i)
        {
            var item = sortGroop.Buttons[i];
            if (sortGroop.isActiveButton != item) { continue; }
            sortType = (SortType)i;//キャスト
        }

        //昇順/降順
        {
            //ここはboolだから"if"にしとく。←こっちのがわかりやすい
            const int c_Asc = 0;//昇順時の配列(element)に対応した番号(インデックス)

            //ラジオボタンで2種しかないため、↓がfalseの時、降順が確定する
            isSortAsc = orderGroop.Buttons[c_Asc].IsActive;
        }
    }

    /// <summary>
    /// gridに接続している子オブジェクトの全削除
    /// </summary>
    public void DestroyCharts()
    {
        //子オブジェクトの削除
        foreach (var child in grid.GetChildList()) { Destroy(child.gameObject); }
    }
}