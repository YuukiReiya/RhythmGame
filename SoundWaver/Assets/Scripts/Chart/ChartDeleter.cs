#define USE_LINQ
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using System.IO;
#if USE_LINQ
using System.Linq;
#endif
using DG.Tweening;
using UnityEngine.Networking;
using Yuuki.MethodExpansions;

namespace Game.UI
{
    /// <summary>
    /// 譜面の削除担当
    /// </summary>
    public class ChartDeleter : MonoBehaviour
    {
        /// <summary>
        /// スクロール背景のテクスチャ
        /// </summary>
        [System.Serializable]
        struct BackScrollTexture
        {
            /// <summary>
            /// スクロールの"forground"で指定するとカラーのα値が書き換わってしまうので補正用
            /// </summary>
            public Color color;

            /// <summary>
            /// 補正するテクスチャ本体
            /// </summary>
            public UITexture texture;
        }

        //serialize param
        [SerializeField] GameObject parent;
        [SerializeField] GameObject prefab;
        [SerializeField] BackScrollTexture backScrollTexture;
        [Header("NGUI")]
        [SerializeField] private UIGrid grid;
        [SerializeField] private UIScrollBar scrollBar;
        [SerializeField] private float tweenSpeed = 0.3f;
        [System.Serializable]
        struct AllCheck
        {
            public GameObject active;
            public GameObject mismatch;//一つでも異なるものがあった場合
        }
        [Header("ALL")]
        [SerializeField] private AllCheck allCheckBox;
        //private param
        struct Item
        {
            public string filePath;
            public CheckBox checkBox;
        }
        private List<Item> list;
        private uint number;

        //const param

        private void Awake()
        {
            list = new List<Item>();
        }

        public void LoadToDisplay()
        {
            //背景色の再調整
            StartCoroutine(BackScrollTextureRoutine());

            //子が削除されてなければ削除
            if (grid.GetChildList().Count > 0) { DestroyScrollChildren(); }

            //譜面番号の初期化
            number = 1;

            //並び替えの方法を指定
            grid.onCustomSort = (a, b) => { return ChartManager.Instance.ChartSort(a, b); };

            //プレハブリストの作成
            CreateList();

            //全選択チェックボックスの更新
            UpdateAllCheckBox();

            //整列
            grid.Reposition();
          
            //スクロールバーを初期位置に戻す
            StartCoroutine(ScrollBarValueResetZero());
        }

        /// <summary>
        /// 上記関数の実処理
        /// </summary>
        /// <returns></returns>
        IEnumerator BackScrollTextureRoutine()
        {
            yield return new WaitForEndOfFrame();
            backScrollTexture.texture.color = backScrollTexture.color;
        }

        /// <summary>
        /// プレハブ生成
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        GameObject Create(Chart chart, string filePath)
        {
            var inst = Instantiate(prefab);
            inst.transform.parent = grid.transform;
            inst.transform.localScale = prefab.transform.localScale;
            Item item = new Item();
            item.checkBox = inst.GetComponentInChildren<CheckBox>();
#if UNITY_EDITOR
            if (!item.checkBox)
            {
                Debug.LogError("prefabの子オブジェクトにCheckBoxがアタッチされていない。");
                Destroy(inst);
                return null;
            }
#endif
            ChartProxy proxy;
            if (!inst.TryGetComponent(out proxy))
            {
                Destroy(inst);
                return null;
            }
            inst.name = chart.ResistName;
            proxy.SetupChart(chart, number++);
            item.filePath = filePath;

            list.Add(item);
            item.checkBox.CallDisable();
            item.checkBox.onActiveFunc =
                () =>
                {
                    UpdateAllCheckBox();
                };
            item.checkBox.onDisableFunc =
                () =>
                {
                    UpdateAllCheckBox();
                };
            return inst;
        }

        /// <summary>
        /// プレハブリストの作成
        /// プリセットファイルは削除の対象外
        /// </summary>
        void CreateList()
        {
            //譜面ファイルのパス格納配列
            string[] charts = null;

            //全取得
            charts = Directory.GetFiles(Define.c_ChartSaveDirectory, "*" + Define.c_JSON);

#if USE_LINQ
            //プリセットファイルの譜面名(拡張子を除く)を取得
            var presetChartsName = Define.c_PresetFilePath.Select(it => Path.GetFileNameWithoutExtension(it.Item2));
            
            //取得した譜面名のなかからプリセットでないものを取得
            charts = charts.Where(it => !presetChartsName.Contains(Path.GetFileNameWithoutExtension(it))).ToArray();
#else
            //プリセットファイルの取得
            var presetChartsName = Define.c_PresetFilePath.Select(it => Path.GetFileNameWithoutExtension(it.Item2));

#endif

            //譜面を選択している場合
            if (ChartManager.Chart.ResistName != string.Empty)
            {
                //現在選択中の譜面は削除リストから外す => 選択中の譜面以外を取得
                //※("登録名 = ファイル名")で判定
                charts = charts.Where(
                    it => Path.GetFileNameWithoutExtension(it) != ChartManager.Chart.ResistName
                    ).ToArray();
            }

            //リストの初期化
            list.Clear();

            //作成
            Yuuki.FileIO.FileIO fileIO = new Yuuki.FileIO.FileIO();
            foreach (var it in charts)
            {
                var chart = JsonUtility.FromJson<Chart>(fileIO.GetContents(it));
                Create(chart, it);
            }
        }

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

        public void DestroyScrollChildren()
        {
            //子が削除されてなければ削除
            foreach (var child in grid.GetChildList()) { Destroy(child.gameObject); }
        }

        /// <summary>
        /// チェックを付けた譜面の削除処理
        /// </summary>
        public void CheckDeleteCharts()
        {
            var deleteCharts = list.Where(it => it.checkBox.IsActive);
            if (deleteCharts.Count() > 0)
            {
                DialogController.Instance.Open(
                    "選択した譜面を削除します。\nよろしいですか?",
                    () =>
                    {
                        this.StartCoroutine(
                            DeleteExecuteRoutine(),
                            () => { LoadToDisplay(); }
                            );
                    },
                    null
                    );
            }
            else
            {
                DialogController.Instance.Open("譜面が選択されていません。");
            }
        }
        IEnumerator DeleteExecuteRoutine()
        {
            //譜面の削除
            var deleteCharts = list.Where(it => it.checkBox.IsActive);
            foreach (var it in deleteCharts)
            {
                using (var uwr = UnityWebRequest.Get(Define.c_LocalFilePath + it.filePath))
                {
                    yield return uwr.SendWebRequest();
                    if (uwr.isNetworkError || uwr.isHttpError)
                    {
                        Debug.LogError("ChartDeleter.cs line248 UnityWebRequest");
                        ErrorManager.Save();
                        continue;
                    }
                    File.Delete(it.filePath);
                }
            }

            //更新データの再表示
            LoadToDisplay();
            ChartManager.Instance.LoadToDisplay();
            DialogController.Instance.Open("選択した譜面は削除されました。");
            yield return new WaitForEndOfFrame();
        }

        /// <summary>
        /// 全選択のチェックボックスの状態を更新
        /// </summary>
        void UpdateAllCheckBox()
        {
            bool isAllCheck = list.All(it => it.checkBox.IsActive);

            //全選択されていたら
            if(isAllCheck)
            {
                allCheckBox.active.SetActive(true);
                allCheckBox.mismatch.SetActive(false);
            }
            else
            {
                //全部選択されていなかった場合
                if(list.All(it=>!it.checkBox.IsActive))
                {
                    allCheckBox.active.SetActive(false);
                    allCheckBox.mismatch.SetActive(false);
                }
                //一つでも異なっているものがあれば
                else
                {
                    allCheckBox.active.SetActive(false);
                    allCheckBox.mismatch.SetActive(true);
                }
            }
        }

        public void OnAllActive()
        {
            foreach(var it in list)
            {
                it.checkBox.CallActive();
            }
        }

        public void OnAllDisable()
        {
            foreach (var it in list)
            {
                it.checkBox.CallDisable();
            }

        }

        public void OnAllCheckBox()
        {
            //全部選択済みにする
            bool isActivate = !list.All(it => it.checkBox.IsActive);
            //bool isActivate = !allCheckBox.active.activeSelf;//選択が交互
            //全部アクティブ化する
            if (isActivate)
            {
                OnAllActive();
            }
            //全部非アクティブ化する
            else
            {
                OnAllDisable();
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Setup Back Scroll Texture color")]
        void SetupBackScrollTextureColor()
        {
            backScrollTexture.color = backScrollTexture.texture.color;
        }
#endif
    }
}