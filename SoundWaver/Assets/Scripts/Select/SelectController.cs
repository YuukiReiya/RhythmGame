using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yuuki;

namespace Game
{
    //TODO:シングルトンの必要性...
    public class SelectController : SingletonMonoBehaviour<SelectController>
    {

        // Start is called before the first frame update
        void Start()
        {
            //楽曲リストの表示
            ChartManager.Instance.LoadToDisplay();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}