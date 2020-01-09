using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NGUIのUILabelを用いたハイパーリンク展開用補助スクリプト
/// </summary>
namespace Game.UI
{
    public class OpenURLUtil : MonoBehaviour
    {
        [SerializeField] UILabel label;
        public void OpenLink()
        {
            var link = label.GetUrlAtPosition(UICamera.lastWorldPosition);
            Application.OpenURL(link);
        }

#if UNITY_EDITOR
        private void Reset()
        {
            label = GetComponent<UILabel>();
        }
#endif
    }
}