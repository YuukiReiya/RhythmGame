using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NGUI.Util
{
    /// <summary>
    /// 3DオブジェクトをNGUIよりも手前で表示するためのUtilスクリプト
    /// </summary>
    public class RenderFrontThanNGUI : MonoBehaviour
    {
        [SerializeField, Tooltip("NGUIの描画はRenderQueue3000+Depthで計算されるので、それより大きい値を入力すること!")] uint renderQueue = 4000;
        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}