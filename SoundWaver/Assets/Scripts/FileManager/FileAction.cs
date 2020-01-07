using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using System.IO;
using Game;
using Game.Audio;
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
            AudioManager.Instance.PlaySE("Folder_FileManager");
            var currentDirectory = FileManager.Instance.CurrentDirectory + Define.c_Delimiter + fileName.text;
            FileManager.Instance.UpdateCurrentDirectories(currentDirectory);
            FileManager.Instance.Display();
        }

        public void SetupMusicFile()
        {
            //  拡張子を判定
            if (Path.GetExtension(fileName.text) == Define.c_MP3)
            {
                AudioManager.Instance.PlaySE("File_OK");
                AutoMusicScoreFactor.Instance.SetupMusic(fileName.text);
            }
            else
            {
                AudioManager.Instance.PlaySE("File_Error");
                DialogController.Instance.Open("ファイルが認識できません。\n対応拡張子は\".mp3\"のみです");
            }
        }

        public void SetupImageFile()
        {
            //  拡張子を判定
            if (Path.GetExtension(fileName.text) == Define.c_PNG)
            {
                AudioManager.Instance.PlaySE("File_OK");
                AutoMusicScoreFactor.Instance.SetupImage(fileName.text);
            }
            else
            {
                AudioManager.Instance.PlaySE("File_Error");
                DialogController.Instance.Open("ファイルが認識できません。\n対応拡張子は\".png\"のみです");
            }
        }
    }
}