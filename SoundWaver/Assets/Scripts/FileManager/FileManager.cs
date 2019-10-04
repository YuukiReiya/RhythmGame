using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Common;
using Yuuki.MethodExpansions;
namespace Yuuki.FileManager
{
    public class FileManager : SingletonMonoBehaviour<FileManager>
    {
        [System.Serializable]
        struct FileTypePrefabs
        {
            public GameObject folder;
            public GameObject file;
        }
        [Header("FileIO Parameter")]
        //serialize param
        [SerializeField] FileTypePrefabs prefabs;
        [SerializeField] UIGrid grid;
        [SerializeField] UIScrollBar scrollBar;
        [Header("Animation")]
        [SerializeField,Tooltip("スケーリングするUIの親オブジェクト")] GameObject scaleParentObj;
        [SerializeField,Range(0,3)] float scaleSec;

        //private param
        IEnumerator routine;
        //public param
        //accessor
        public string CurrentDirectory { get; private set; }

        // Start is called before the first frame update
        void Start()
        {
            Open();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void UpdateCollider()
        {
            NGUITools.UpdateWidgetCollider(grid.gameObject);
            BoxCollider col;
            if (grid.TryGetComponent<BoxCollider>(out col))
            {
                var pos = col.center;
                pos.z = 1;
                col.center = pos;
            }
        }

        GameObject Create(GameObject prefab, string labelName)
        {
            var inst = Instantiate(prefab);
            inst.transform.parent = grid.transform;
            inst.transform.localScale = prefab.transform.localScale;
            FileAction ui;
            if (!inst.TryGetComponent<FileAction>(out ui))
            {
                Destroy(inst);
                return null;
            }
            ui.SetupName(labelName);
            return inst;
        }

        /// <summary>
        /// カレントディレクトリの設定
        /// ※ディレクトリが無ければ処理しない
        /// </summary>
        /// <param name="currentPath"></param>
        public void UpdateCurrentDirectories(string currentPath)
        {
            if (!Directory.Exists(currentPath)) { return; }
            this.CurrentDirectory = currentPath;
            var children = grid.GetChildList();
            //子オブジェクト削除
            for (int i = 0; i < children.Count; ++i)
            {
                var child = children[i];
                child.parent = null;
                Destroy(child.gameObject);
            }

            //並び替え
            scrollBar.value = 0;//スクロールバーを先頭に
        }

        public void Display()
        {
            //  フォルダ
            foreach(var it in Directory.GetDirectories(CurrentDirectory))
            {
                var name = Path.GetFileName(it);
                Create(prefabs.folder,name);
            }
            //  ファイル
            foreach (var it in Directory.GetFiles(CurrentDirectory))
            {
                var name = Path.GetFileName(it);
                var prefab = GetFilePrefab(Path.GetExtension(it));
                Create(prefab, name);
            }
            grid.Reposition();//整列
            UpdateCollider();//コライダー再設定
        }

        /// <summary>
        /// 拡張子に合わせてファイルのスプライトを変更する
        /// TODO:AssetBundle等でファイルをロードすべき？
        /// </summary>
        /// <param name="extention"></param>
        /// <returns></returns>
        GameObject GetFilePrefab(string extention)
        {
            //
            return prefabs.file;
        }

        public void MoveupDirectory()
        {
            var upDirectory = Path.GetDirectoryName(CurrentDirectory);
            if (!Directory.Exists(upDirectory)) { return; }
            CurrentDirectory = upDirectory;
            UpdateCurrentDirectories(upDirectory);
            Display();
        }

        public void Open()
        {
            if (routine != null) { return; }
            //表示済みなら処理しない
            //if (scaleParentObj.transform.localScale == Vector3.one) { return; }

            //.iniファイルに設定されているディレクトリを参照
            var fileIO = new FileIO.FileIO();
            if (!File.Exists(Define.c_SettingFilePath))
            {
                //無いので作る
                fileIO.CreateFile(Define.c_SettingFilePath, Application.persistentDataPath);
            }
            var currentDirectory = fileIO.GetContents(Define.c_SettingFilePath);
            CurrentDirectory = currentDirectory;
            UpdateCurrentDirectories(CurrentDirectory);
            Display();
            routine = ScalingRoutine(scaleParentObj, Vector3.zero, Vector3.one, scaleSec);
            this.StartCoroutine(routine, () => { routine = null; });
        }

        public void Close()
        {
            if (routine != null) { return; }
            routine = ScalingRoutine(scaleParentObj, Vector3.one, Vector3.zero, scaleSec);
            var fileIO = new FileIO.FileIO();
            //カレントディレクトリの情報保存
            fileIO.CreateFile(Define.c_SettingFilePath, CurrentDirectory, true);

            this.StartCoroutine(routine, () => { routine = null; });
        }

        IEnumerator ScalingRoutine(GameObject obj,Vector3 from,Vector3 to,float sec)
        {
            float time = Time.time;
            while (Time.time < time + sec)
            {
                float rate = sec > 0.0f ? (Time.time - time) / sec : 1.0f;
                var scale = Vector3.Lerp(from, to, rate);
                obj.transform.localScale = scale;
                yield return null;
            }
            obj.transform.localScale = to;
        }

#if UNITY_EDITOR
        /// <summary>
        /// エディタ上でClose処理を呼びだしたい
        /// </summary>
        [ContextMenu("Close UI")]
        void CloseOnEdtior()
        {
            if (routine != null) { return; }
            routine = ScalingRoutine(scaleParentObj, Vector3.one, Vector3.zero, 0);
            this.StartCoroutine(routine, () => { routine = null; });
        }
#endif
    }
}