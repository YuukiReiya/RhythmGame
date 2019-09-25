using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace Yuuki.FileIO
{
    public class FileIO : SingletonMonoBehaviour<FileIO>
    {


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
        }

        public string[] GetSubFolders(string path)
        {
            return Directory.GetDirectories(path);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool CreateFile(string filePath,string[] contents)
        {
            //フォルダ作成
            string dirPath = Path.GetDirectoryName(filePath);
            if (dirPath != string.Empty && !Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }

            //既に存在
            if (File.Exists(filePath)) { return false; }
            //作成
            using (StreamWriter sw = new StreamWriter(filePath, true))
            {
                foreach (var it in contents) { sw.WriteLine(it); }
                sw.Close();
            }
            return true;
        }

        /// <summary>
        /// ディレクトリ作成
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns>既に存在すれば作成せずにfalse</returns>
        public bool CreateDirectory(string directoryPath)
        {
            if (Directory.Exists(directoryPath)) { return false; }
            if (Directory.CreateDirectory(directoryPath) == null) { return false; }
            return true;
        }

        public string Read(string filePath)
        {
            if (!File.Exists(filePath)) { return string.Empty; }
            string contents = string.Empty;
            using (var sr=new StreamReader(filePath))
            {
                contents = sr.ReadToEnd();
                sr.Close();
            }
            return contents;
        }
    }
}