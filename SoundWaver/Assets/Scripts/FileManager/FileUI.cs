using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yuuki.FileManager
{
    public class FileUI : MonoBehaviour
    {
        [SerializeField] UILabel fileName;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Setup(string fileName)
        {
            this.fileName.text = fileName;
        }
    }
}