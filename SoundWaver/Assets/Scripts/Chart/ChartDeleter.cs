using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using System.IO;
using System.Linq;
using DG.Tweening;

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
        private List<CheckBox> list;
        private uint number;

        //const param
        const uint c_WaitForSyncFrame = 10;

        private void Awake()
        {
            list = new List<CheckBox>();
        }

        public void LoadToDisplay()
        {
            //子が削除されてなければ削除
            if (grid.GetChildList().Count > 0)
            {
                foreach (var child in grid.GetChildList()) { Destroy(child.gameObject); }
            }

            //譜面番号の初期化
            number = 1;

            //並び替えの方法を指定
            grid.onCustomSort = (a, b) => { return ChartManager.Instance.ChartSort(a, b); };

            //プレハブリストの作成
            CreateList();

            //全選択チェックボックスの更新
            UpdateAllCheckBox();

            //スクロールバーを初期位置に戻す
            StartCoroutine(ScrollBarValueResetZero());

            //整列
            grid.Reposition();
        }

        public void SetupBackScrollTexture()
        {
            StartCoroutine(BackScrollTextureRoutine());
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
        GameObject Create(Chart chart)
        {
            var inst = Instantiate(prefab);
            inst.transform.parent = grid.transform;
            inst.transform.localScale = prefab.transform.localScale;
            CheckBox checkBox = inst.GetComponentInChildren<CheckBox>();
            if (!checkBox)
            {
#if UNITY_EDITOR
                Debug.LogError("prefabの子オブジェクトにCheckBoxがアタッチされていない。");
                Destroy(inst);
                return null;
#endif
            }
            ChartProxy proxy;
            if (!inst.TryGetComponent(out proxy) )
            {
                Destroy(inst);
                return null;
            }
            inst.name = chart.ResistName;
            proxy.SetupChart(chart, number++);

            list.Add(checkBox);
            checkBox.CallDisable();
            checkBox.onActiveFunc =
                () =>
                {
                    UpdateAllCheckBox();
                };
            checkBox.onDisableFunc =
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

            //プリセットファイルの譜面名を取得
            var presetChartsName = Define.c_PresetFilePath.Select(it => Path.GetFileName(it.Item2));
            
            //取得した譜面名のなかからプリセットでないものを取得
            charts = charts.Where(it => !presetChartsName.Contains(Path.GetFileName(it))).ToArray();

            //リストの初期化
            list.Clear();

            //作成
            Yuuki.FileIO.FileIO fileIO = new Yuuki.FileIO.FileIO();
            foreach (var it in charts)
            {
                var chart = JsonUtility.FromJson<Chart>(fileIO.GetContents(it));
                Create(chart);
            }
        }

        IEnumerator ScrollBarValueResetZero()
        {
            scrollBar.value = 1;
            for (int c = 0; c < c_WaitForSyncFrame; ++c) { yield return null; }
            DOTween.To(
                () => scrollBar.value,
                v => scrollBar.value = v,
                0.0f,
                tweenSpeed);
            yield break;
        }

        /// <summary>
        /// 全選択のチェックボックスの状態を更新
        /// </summary>
        void UpdateAllCheckBox()
        {
            bool isAllCheck = list.All(it => it.IsActive);

            //var a=list.FindAll(p=>p.IsActive)
            Debug.Log("a:" + allCheckBox.active.activeSelf);
            Debug.Log("b:" + allCheckBox.mismatch.activeSelf);


            //全選択されていたら
            if(isAllCheck)
            {
                allCheckBox.active.SetActive(true);
                allCheckBox.mismatch.SetActive(false);
            }
            else
            {
                //全部選択されていなかった場合
                if(list.All(it=>!it.IsActive))
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
                it.CallActive();
            }
        }

        public void OnAllDisable()
        {
            foreach (var it in list)
            {
                it.CallDisable();
            }

        }

        public void OnAllCheckBox()
        {
            //全部選択済みにする
            bool isActivate = !list.All(it => it.IsActive);
            //bool isActivate = !allCheckBox.active.activeSelf;//選択が交互
            //全部アクティブ化する
            if (isActivate)
            {
                Debug.Log("call1");
                OnAllActive();
            }
            //全部非アクティブ化する
            else
            {
                Debug.Log("call2");
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