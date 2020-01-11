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
using System.Threading.Tasks;
public class ChartManager : SingletonMonoBehaviour<ChartManager>
{
    //serialize param
    [System.Serializable]
    struct Prefabs
    {
        public GameObject clear;
        public GameObject notClear;
    }
    [Header("Prefabs")]
    [SerializeField] Prefabs prefabs;
    [Header("NGUI")]
    [SerializeField] private UIGrid grid;
    [Header("Scroll")]
    [SerializeField] private UIScrollBar scrollBar;
    [SerializeField] private float tweenSpeed = 0.3f;
    [SerializeField] UITexture backImage;
    [SerializeField] Color backImageColor;
    [Header("Image")]
    [SerializeField] private Texture2D noImage;
    [System.Serializable]
    struct ChartImageUI
    {
        public UITexture chartImage;
        public UITexture effectImage;
        /// <summary>
        /// 譜面タップ時のランダム色を反映させるテクスチャ
        /// </summary>
        public UITexture[] otherImages;
    }
    [SerializeField] private ChartImageUI chartImageUI;
    [System.Serializable]
    struct TapObjects
    {
        /// <summary>
        /// タップされたときに非アクティブ化するオブジェクト・アクティブ化するオブジェクト
        /// </summary>
        public GameObject[] disableObjects;
        public GameObject[] activeObjects;

    }
    [Header("Tap")]
    [SerializeField] TapObjects tapObjects;
    [System.Serializable]
    struct TapOnceProcces
    {
        //スコア関連
        public GameObject scoreObj;
        public float time;
        public Vector3 to;
    }
    [Header("Initial Tap Once Process")]
    [SerializeField] private TapOnceProcces onceProcces;
    [Header("Score")]
    [SerializeField] private UILabel score;
    [SerializeField] private GameObject wasClearObj;
    [SerializeField] private GameObject notClearObj;
    [Header("Refine param")]
    [SerializeField] private RadioButtonGroop chartGroop;
    [SerializeField] private RadioButtonGroop sortGroop;
    [SerializeField] private RadioButtonGroop orderGroop;
    [System.Serializable]
    struct SwitchingObject
    {
        public GameObject ImageObj;
        public GameObject DetailObj;
        public UITexture detailTexture;
        public UILabel Name;
        public UILabel NotesCount;
        public UILabel MaxComb;
        //score count
        public UILabel PerfectCount;
        public UILabel GreatCount;
        public UILabel GoodCount;
        public UILabel MissCount;
    }
    [Header("Switching chart panel")]
    [SerializeField] private SwitchingObject switchingObject;
    [SerializeField] private UIButton switchingButton;
#if UNITY_EDITORl
    #region BGM
    [System.Serializable]
    struct BGMObject
    {
        public AudioSource audioSource;
        public GameObject parent;
        public Vector3 direction;
        public UILabel name;
        public float tweenTime;
        public float dispTime;
        public IEnumerator routine;
    }
    [Header("BGM")]
    [SerializeField] BGMObject bgmObj;
    private IEnumerator dispScoreRoutine;
    private AudioClip BGMclip;
    #endregion
#endif
    //private param
    private uint number;
    private SortType sortType = SortType.Hierarchy;
    private ChartType chartType = ChartType.All;
    private bool isSortAsc = true;//並び順が昇順?
    System.Action onceCallFunction;//一度だけ呼ばれる関数

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

    //const param

    public void Setup()
    {
        //スコアの初期化
        onceProcces.scoreObj.SetActive(false);//しないと動かない

        //切り替えボタンOFF
        switchingButton.isEnabled = false;

        //グループ初期化
        chartGroop.Setup();
        sortGroop.Setup();
        orderGroop.Setup();

        //譜面パネルの初期化
        switchingObject.ImageObj.SetActive(true);
        switchingObject.DetailObj.SetActive(false);

        //一度だけ呼ばれる関数
        onceCallFunction +=
            () =>
            {
                //イメージの背景画像
                foreach (var it in chartImageUI.otherImages)
                {
                    if (!it.gameObject.activeSelf)
                    {
                        it.gameObject.SetActive(true);
                    }
                }

                //イメージの効果画像
                if (!chartImageUI.effectImage.gameObject.activeSelf)
                {
                    chartImageUI.effectImage.gameObject.SetActive(true);
                }

                //アクティブ化⇒非アクティブ化
                foreach (var it in tapObjects.disableObjects)
                {
                    if (it.activeSelf) { it.SetActive(false); }
                }

                //非アクティブ化⇒アクティブ化
                foreach (var it in tapObjects.activeObjects)
                {
                    if (!it.activeSelf) { it.SetActive(true); }
                }

                //切り替えボタンON(最初だけ有効)
                {
                    if (!switchingButton.isEnabled)
                    {
                        switchingButton.isEnabled = true;
                    }
                }
                //スコア出現
                SetupScoreObject();
            };
    }

    /// <summary>
    /// 楽曲(譜面)リストの表示
    /// </summary>
    public void LoadToDisplay()
    {
        //背景イメージ色の再調整
        StartCoroutine(SetupBackImageColorRoutine());

        //子が削除されてなければ削除
        if (grid.GetChildList().Count > 0) { DestroyCharts(); }

        //譜面番号の初期化
        number = 1;

        //並び替えの方法を指定
        grid.onCustomSort = (a, b) => { return ChartSort(a, b); };

        //譜面情報の取得
        CreateList();

        //整列
        grid.Reposition();

        #region スクロールバー関連
        {
            //スクロールバーを初期位置に戻す
            //StartCoroutine(ScrollBarValueResetZero());
            scrollBar.value = 1;
            DOTween.To(
                () => scrollBar.value,
                v => scrollBar.value = v,
                0.0f,
                tweenSpeed);
        }
        #endregion

    }

    /// <summary>
    /// 譜面プレハブの作成
    /// </summary>
    /// <param name="chart"></param>
    /// <returns></returns>
    GameObject Create(Chart chart)
    {
        var prefab = chart.wasCleared ? prefabs.clear : prefabs.notClear;
        var inst = Instantiate(prefab);
        //TODO:NGUITool.AddChildでスケール問題を防げるらしい
        //現状できてるし、変にテスト増やしたくないので気が向いたらやる
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
        //色の設定
        //TODO:暖色限定にしたり、そこら辺はとりま考えない。。。
        proxy.color = new Color(Random.value, Random.value, Random.value);
        return inst;
    }

    /// <summary>
    /// カスタムソート
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public int ChartSort(Transform a, Transform b)
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

    void CreateList()
    {
        //譜面ファイルのパス格納配列
        string[] charts = null;

        //全取得
        charts = Directory.GetFiles(Define.c_ChartSaveDirectory, "*" + Define.c_JSON);

        switch (chartType)
        {
            //全て
            case ChartType.All: break;
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
    /// スクロールバーのvalueをゼロに初期化
    /// </summary>
    /// <returns></returns>
    IEnumerator ScrollBarValueResetZero()
    {
        scrollBar.value = 1;
        DOTween.To(
            () => scrollBar.value,
            v => scrollBar.value = v,
            0.0f,
            tweenSpeed);
        yield break;
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

    public void UpdateChartPanel()
    {
        //最初の一度だけ呼ばれる関数
        onceCallFunction?.Invoke();
        onceCallFunction = null;

        //スコア設定
        UpdateScore();

        //イメージ画像の表示
        //Android上での画像表示時にも問題がないか確認すること！
        var path = Chart.ImageFilePath.Replace(Define.c_LocalFilePath, "");
        if (File.Exists(path))
        {
            StartCoroutine(LoadChartImageRoutine(Chart.ImageFilePath));
        }
        else
        {
            //該当テクスチャ割り当て
            chartImageUI.chartImage.mainTexture = noImage;

            //詳細画面表示時の後ろにも割り当てる
            //TODO:暇なら直す
            switchingObject.detailTexture.mainTexture = noImage;
        }
    }

    public void SetImageEffectColor(Color cr)
    {
        foreach (var it in chartImageUI.otherImages)
        {
            var color = it.color;
            color = cr;
            color.a = it.color.a;
            it.color = color;
        }
    }

#if UNITY_EDITOR
    #region マルチスレッド
    private async Task LoadChartImageTaskAsync(string path)
    {
        Texture2D tex = null;
        //イメージファイルのロード
        var request = UnityWebRequestTexture.GetTexture(path);
        {
            Debug.Log("load image A:" + System.Threading.Thread.CurrentThread.ManagedThreadId);
            await request.SendWebRequest();

            Debug.Log("load image B:" + System.Threading.Thread.CurrentThread.ManagedThreadId);

            if (request.isNetworkError || request.isHttpError)
            {
#if UNITY_EDITOR
                Debug.LogError("path:\n" + path);
                Debug.LogError("Error:" + request.error);
#endif
                chartImageUI.chartImage.mainTexture = noImage;
                ErrorManager.Save();
                tex = noImage;
            }
            else
            {
                tex = DownloadHandlerTexture.GetContent(request);
                //chartImageUI.chartImage.mainTexture = DownloadHandlerTexture.GetContent(request);
            }
        }
        //該当テクスチャ割り当て
        chartImageUI.chartImage.mainTexture = tex;

        //詳細画面表示時の後ろにも割り当てる
        //TODO:暇なら直す
        switchingObject.detailTexture.mainTexture = tex;
    }
    #endregion
#endif
    IEnumerator LoadChartImageRoutine(string path)
    {
        Texture2D tex = null;
        //イメージファイルのロード
        using (var www = UnityWebRequestTexture.GetTexture(path))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
#if UNITY_EDITOR
                Debug.LogError("path:\n" + path);
                Debug.LogError("Error:" + www.error);
#endif
                chartImageUI.chartImage.mainTexture = noImage;
                ErrorManager.Save();
                tex = noImage;
            }
            else
            {
                tex = DownloadHandlerTexture.GetContent(www);
                //chartImageUI.chartImage.mainTexture = DownloadHandlerTexture.GetContent(www);
            }
        }
        //該当テクスチャ割り当て
        chartImageUI.chartImage.mainTexture = tex;

        //詳細画面表示時の後ろにも割り当てる
        //TODO:暇なら直す
        switchingObject.detailTexture.mainTexture = tex;
        yield break;
    }

    IEnumerator SetupBackImageColorRoutine()
    {
        yield return new WaitForEndOfFrame();
        backImage.color = backImageColor;
    }

    void SetupScoreObject()
    {
        if (onceProcces.scoreObj.activeSelf) { return; }
        onceProcces.scoreObj.SetActive(true);
        onceProcces.scoreObj.transform.DOLocalMove
            (
            onceProcces.to,
            onceProcces.time
            );
    }

    public void UpdateScore()
    {
        wasClearObj.SetActive(Chart.wasCleared);
        notClearObj.SetActive(!Chart.wasCleared);
        if (Chart.wasCleared)
        {
            score.text = Chart.Score.ToString();
        }

        //裏面
        switchingObject.Name.text = Chart.ResistName;//名前
        switchingObject.MaxComb.text = Chart.wasCleared ? Chart.Comb.ToString() : "0";//コンボ
        switchingObject.NotesCount.text = Chart.Notes.Length.ToString();//ノーツ数
        switchingObject.PerfectCount.text = Chart.ScoreCounts.Perfect.ToString();
        switchingObject.GreatCount.text = Chart.ScoreCounts.Great.ToString();
        switchingObject.GoodCount.text = Chart.ScoreCounts.Good.ToString();
        switchingObject.MissCount.text = Chart.ScoreCounts.Miss.ToString();
    }

    public void SwitchingChartPanel()
    {
        bool isImageActive = switchingObject.ImageObj.activeSelf;
        switchingObject.ImageObj.SetActive(!isImageActive);
        switchingObject.DetailObj.SetActive(isImageActive);
    }

#if Coroutine
#region コルーチン
    public void PlayBGM()
    {
        StartCoroutine(LoadBGMRoutine());
    }
    IEnumerator LoadBGMRoutine()
    {
        AudioClip clip = null;
        var path = Chart.MusicFilePath;
        bgmObj.name.text = "読み込み中";
        using (var uwr=UnityWebRequestMultimedia.GetAudioClip(path,AudioType.MPEG))
        {
            yield return uwr.SendWebRequest();
            //エラー
            if (uwr.isNetworkError || uwr.isHttpError)
            {
#if UNITY_EDITOR
                Debug.LogError("path:\n" + path);
                Debug.LogError("Error:" + uwr.error);
#endif
                ErrorManager.Save();
                clip = null;
                bgmObj.name.text = "not found music";
            }
            //読み込みOK
            else
            {
                clip = DownloadHandlerAudioClip.GetContent(uwr);
                bgmObj.name.text = Path.GetFileNameWithoutExtension(Chart.MusicFilePath);
            }
        }
        //割り当てと再生
        bgmObj.audioSource.clip = clip;
        if (bgmObj.audioSource.clip)
        {
            bgmObj.audioSource.Play();
        }
        yield break;
    }
#endregion
#endif
#if MultiThread
#region マルチスレッド

    public async void PlayBGM()
    {
        LoadBGMTaskAsync();
        await
            Task.Run(async () =>
            {
                 //LoadBGMTaskAsync();
            });
    }

    public async Task LoadBGMTaskAsync()
    {
        var path = Chart.MusicFilePath;
        bgmObj.name.text = "読み込み中";
        Debug.Log("load bgm A:" + System.Threading.Thread.CurrentThread.ManagedThreadId);
        var request = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.MPEG);
        await request.SendWebRequest();
        Debug.Log("load bgm B:" + System.Threading.Thread.CurrentThread.ManagedThreadId);
        //エラー
        if (request.isNetworkError || request.isHttpError)
        {
#if UNITY_EDITOR
            Debug.LogError("path:\n" + path);
            Debug.LogError("Error:" + request.error);
#endif
            ErrorManager.Save();
            BGMclip = null;
            bgmObj.name.text = "not found music";
        }
        else
        {
            BGMclip = DownloadHandlerAudioClip.GetContent(request);
            bgmObj.name.text = Path.GetFileNameWithoutExtension(Chart.MusicFilePath);
        }
        //割り当てと再生
        bgmObj.audioSource.clip = BGMclip;
        if (bgmObj.audioSource.clip)
        {
            Debug.Log("ok");
            bgmObj.audioSource.Play();
        }
    }
#endregion
#endif
}