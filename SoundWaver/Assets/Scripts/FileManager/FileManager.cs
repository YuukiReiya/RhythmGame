using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        //serialize param
        [SerializeField] FileTypePrefabs prefabs;
        [SerializeField] UIGrid grid;
        //private param
        //public param
        //accessor
        public string CurrentDirectory { get; private set; }

        // Start is called before the first frame update
        void Start()
        {
            Test();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void UpdateCollider()
        {
            NGUITools.UpdateWidgetCollider(grid.gameObject);
        }

        GameObject Create(GameObject prefab, string labelName)
        {
            var inst = Instantiate(prefab);
            inst.transform.parent = grid.transform;
            inst.transform.localScale = prefab.transform.localScale;
            FileUI ui;
            if (!inst.TryGetComponent<FileUI>(out ui))
            {
                Destroy(inst);
                return null;
            }
            ui.Setup(labelName);
            return inst;
        }

        void Test()
        {

            foreach(var it in System.IO.Directory.GetDirectories(System.IO.Directory.GetCurrentDirectory()))
            {
                Debug.Log(it);
                var name = System.IO.Path.GetFileName(it);
                Debug.Log(name);
                Create(prefabs.folder,name);
            }
            grid.Reposition();
        }
    }
}