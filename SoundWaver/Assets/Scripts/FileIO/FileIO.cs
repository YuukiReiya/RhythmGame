using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace Yuuki.FileIO
{
    public class FileIO : MonoBehaviour
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

    }
}