using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class CheckBox : MonoBehaviour
    {
        //serialize param
        [SerializeField] private GameObject activeObj;

        //private param
        private bool isActive = false;

        //public param
        public bool IsActive { get { return isActive; } }
        public System.Action onActiveFunc = null;
        public System.Action onDisableFunc = null;

        /// <summary>
        /// trueにする
        /// </summary>
        public void CallActive()
        {
            isActive = true;
            activeObj.SetActive(isActive);
            onActiveFunc?.Invoke();
        }

        /// <summary>
        /// falseにする
        /// ※OnDiableをオーバーライドしたくなかったから"On"=>"Call"で統一
        /// </summary>
        public void CallDisable()
        {
            isActive = false;
            activeObj.SetActive(isActive);
            onDisableFunc?.Invoke();
        }

        /// <summary>
        /// 逆にする
        /// </summary>
        public void CallSwitch()
        {
            isActive = !isActive;
            activeObj.SetActive(isActive);
            var action = isActive ? onActiveFunc : onDisableFunc;
            action?.Invoke();
        }
    }
}