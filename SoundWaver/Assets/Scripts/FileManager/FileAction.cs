using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using System.IO;
using Game;

namespace Yuuki.FileManager
{
    public class FileAction : MonoBehaviour
    {
        //serialize param
        [SerializeField] private UILabel fileName;
        public void SetupName(string fileName)
        {
            this.fileName.text = fileName;
        }

        public void SetupCurrentDirectory()
        {
            var currentDirectory = FileManager.Instance.CurrentDirectory + Define.c_Delimiter + fileName.text;
            FileManager.Instance.UpdateCurrentDirectories(currentDirectory);
            FileManager.Instance.Display();
        }

        public void SetupMusicFile()
        {
            //  拡張子を判定
            if (Path.GetExtension(fileName.text) == Define.c_MP3)
            {
                AutoMusicScoreFactor.Instance.SetupMusic(fileName.text);
            }
            else
            {
                DialogController.Instance.Open("ファイルが認識できません。\n対応拡張子は\".mp3\"のみです");
            }
        }

        public void SetupImageFile()
        {
            //  拡張子を判定
            if (Path.GetExtension(fileName.text) == Define.c_PNG)
            {
                AutoMusicScoreFactor.Instance.SetupImage(fileName.text);
            }
            else
            {
                DialogController.Instance.Open("ファイルが認識できません。\n対応拡張子は\".png\"のみです");
            }
        }
    }
}