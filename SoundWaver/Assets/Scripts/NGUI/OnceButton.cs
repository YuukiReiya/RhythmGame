using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace API.Util
{

    public class OnceButton : MonoBehaviour
    {
        [SerializeField] private UIButton button;

#if UNITY_EDITOR
        private void Reset()
        {
            button = GetComponent<UIButton>();
        }
#endif
        public void Execute()
        {
            button.disabledColor = button.pressed;
            button.isEnabled = false;
        }
    }

}