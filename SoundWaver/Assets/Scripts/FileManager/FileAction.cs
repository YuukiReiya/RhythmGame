using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yuuki.FileManager
{
    public class FileAction : MonoBehaviour
    {
        [SerializeField]public UILabel fileName;
        public void SetupName(string fileName)
        {
            this.fileName.text = fileName;
        }

        public void SetupCurrentDirectory()
        {
            var currentDirectory = FileManager.Instance.CurrentDirectory + "\\" + fileName.text;
            FileManager.Instance.UpdateCurrentDirectories(currentDirectory);
            FileManager.Instance.Display();
        }
    }
}