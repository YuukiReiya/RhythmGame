using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.UI
{
    public class RadioButton : MonoBehaviour
    {
        //serialize param
        [SerializeField] private GameObject activeObj;
        [SerializeField] private GameObject diableObj;
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
            diableObj.SetActive(!isActive);
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
            diableObj.SetActive(!isActive);
            onDisableFunc?.Invoke();
        }
    }
}