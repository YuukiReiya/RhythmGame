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

        public void SetupMusicFile()
        {
            var filePath = FileManager.Instance.CurrentDirectory + "\\" + fileName.text;
            var ext = System.IO.Path.GetExtension(filePath);

            if(ext==Common.Define.c_MP3|| ext == Common.Define.c_WAV)
            {
                AutoMusicScoreFactor.Instance.SetupMusicName(fileName.text);
            }
            else
            {
                Debug.Log("拡張子が違います:" + ext);
            }
        }
    }
}