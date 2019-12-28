using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Common;
using Yuuki.MethodExpansions;
using Game;
namespace Yuuki.FileManager
{
    /// <summary>
    /// 開発末期でごり押し感。。。
    /// リファインはしっかり(´・ω・`)
    /// </summary>
    public class FileManager : SingletonMonoBehaviour<FileManager>
    {
        [System.Serializable]
        struct FileTypePrefabs
        {
            public GameObject folder;
            public GameObject musicFile;
            public GameObject imageFile;
        }
        [Header("FileIO Parameter")]
        //serialize param
        [SerializeField] FileTypePrefabs prefabs;
        //[SerializeField] UIGrid grid;
        //[SerializeField] UIScrollBar scrollBar;
        [System.Serializable]
        struct Param
        {
            public GameObject obj;
            public UIGrid grid;
            public UIScrollBar scrollBar;
            public UILabel currentDirectoryUI;
            [System.NonSerialized] public string currentDirectory;
        }
        [SerializeField] Param musicFileManager;
        [SerializeField] Param imageFileManager;
        [Header("Animation")]
        [SerializeField, Tooltip("スケーリングするUIの親オブジェクト")] GameObject scaleParentObj;
        [SerializeField, Range(0, 3)] float scaleSec;

        //private param
        IEnumerator routine;
        UIGrid grid { 
            get
            {
                switch (Mode)
                {
                    case SelectMode.None:
                        {
#if UNITY_EDITOR
                            Debug.LogError("予期せぬエラー\n" +
                                "FileManager.cs line46 grid {get}");
#endif
                        }
                        break;
                    case SelectMode.Music: return musicFileManager.grid;
                    case SelectMode.Image:return imageFileManager.grid;
                }
                return musicFileManager.grid;
            }
        }
        UIScrollBar scrollBar
        {
            get
            {
                switch (Mode)
                {
                    case SelectMode.None:
                        {
#if UNITY_EDITOR
                            Debug.LogError("予期せぬエラー\n" +
                                "FileManager.cs line66 grid {get}");
#endif
                        }
                        break;
                    case SelectMode.Music: return musicFileManager.scrollBar;
                    case SelectMode.Image: return imageFileManager.scrollBar;
                }
                return musicFileManager.scrollBar;

            }
        }

        //public param
        //  FileAction通知用です。
        //  ごり押しですよー( ;∀;)
        enum SelectMode
        {
            None,
            Music,   //楽曲指定
            Image,  //画像指定
        }
        SelectMode Mode { get; set; }


        //accessor
        public string CurrentDirectory { get; private set; }

        // Start is called before the first frame update
        void Start()
        {
            Setup();
            //Open();
        }

        private void Setup()
        {
            Mode = SelectMode.None;
            if (musicFileManager.obj.activeSelf)
            {
                musicFileManager.obj.SetActive(false);
            }
            if (imageFileManager.obj.activeSelf)
            {
                imageFileManager.obj.SetActive(false);
            }
        }

        /// <summary>
        /// カレントディレクトリをデフォルトパスに戻す
        /// </summary>
        public void SetDefaultDirectories()
        {
            UpdateCurrentDirectories(Application.persistentDataPath);
            Display();
        }

        public void SaveDirectories()
        {
            DialogController.Instance.Open(
                "現在のディレクトリを\n保存しますか?",
                () =>
                {
                    var fileIO = new FileIO.FileIO();
                    //カレントディレクトリの情報保存
                    fileIO.CreateFile(Define.c_SettingFilePath, CurrentDirectory, FileIO.FileIO.FileIODesc.Overwrite);
                    DialogController.Instance.Open("現在のディレクトリを\n保存しました。");
                },
                null
                );
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
            //  カレントディレクトリの表示
            {
                if (Mode != SelectMode.None)
                {
                    var param = Mode == SelectMode.Music ? musicFileManager : imageFileManager;
                    //1階層上のディレクトリ + 現フォルダ
                    var upDir = Path.GetDirectoryName(CurrentDirectory);
                    if (!Directory.Exists(upDir))
                    {
                        upDir = string.Empty;
                    }
                    else
                    {
                        upDir = Path.GetFileName(upDir);
                    }
                    param.currentDirectoryUI.text = upDir + Define.c_Delimiter + Path.GetFileName(CurrentDirectory);
                }
            }

            //並び替え
            scrollBar.value = 0;//スクロールバーを先頭に
        }

        public void Display()
        {
            if (Mode == SelectMode.None)
            {
                //エラー処理
                return;
            }
            var filePrefab = Mode == SelectMode.Music ? prefabs.musicFile:prefabs.imageFile;

            //  フォルダ
            foreach (var it in Directory.GetDirectories(CurrentDirectory))
            {
                var name = Path.GetFileName(it);
                Create(prefabs.folder, name);
            }
            //  ファイル
            foreach (var it in Directory.GetFiles(CurrentDirectory))
            {
                var name = Path.GetFileName(it);
                var prefab = GetFilePrefab(Path.GetExtension(it));
                Create(filePrefab, name);
            }
            //scrollBar.value = 0;
            grid.Reposition();//整列
            //UpdateCollider();//コライダー再設定
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
            return prefabs.musicFile;
        }

        public void MoveupDirectory()
        {
            var upDirectory = Path.GetDirectoryName(CurrentDirectory);
            if (!Directory.Exists(upDirectory)) { return; }
            CurrentDirectory = upDirectory;
            UpdateCurrentDirectories(upDirectory);
            Display();
        }

        #region 古い
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
        #endregion

        public void OpenMusic()
        {
            
            Mode = SelectMode.Music;
            var param = musicFileManager;
            param.obj.SetActive(true);
            //  .iniファイルに設定されているディレクトリ参照 (.json形式あたりの取得に直す)
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
        }

        public void OpenImage()
        {
            Mode = SelectMode.Image;
            var param = imageFileManager;
            param.obj.SetActive(true);
            //  .iniファイルに設定されているディレクトリ参照 (.json形式あたりの取得に直す)
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
        }

        public void Close()
        {
            //if (routine != null) { return; }
            //routine = ScalingRoutine(scaleParentObj, Vector3.one, Vector3.zero, scaleSec);
            //var fileIO = new FileIO.FileIO();
            ////カレントディレクトリの情報保存
            //fileIO.CreateFile(Define.c_SettingFilePath, CurrentDirectory, FileIO.FileIO.FileIODesc.Overwrite);
            //
            //this.StartCoroutine(routine, () => { routine = null; });

            if (Mode == SelectMode.None) 
            {
#if UNITY_EDITOR
                if (musicFileManager.obj.activeSelf)
                {
                    musicFileManager.obj.SetActive(false);
                }
                if (imageFileManager.obj.activeSelf)
                {
                    imageFileManager.obj.SetActive(false);
                }
#endif
                return; 
            }
            var param = Mode == SelectMode.Music ? musicFileManager : imageFileManager;
            param.obj.SetActive(false);
            Mode = SelectMode.None;
        }

        IEnumerator ScalingRoutine(GameObject obj, Vector3 from, Vector3 to, float sec)
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
