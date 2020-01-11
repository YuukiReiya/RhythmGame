using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
namespace Game
{
    public class DialogController : Yuuki.SingletonMonoBehaviour<DialogController>
    {

        #region 構造体_宣言
        interface Dialog { }
        [System.Serializable]
        struct CheckDialog : Dialog
        {
            public GameObject Parent;
            public UILabel Message;
            public System.Action Callback;

            public void Open()
            {
                Parent.SetActive(true);
            }
            public void Close()
            {
                Callback?.Invoke();
                Callback = null;
                Parent.SetActive(false);
            }
        }
        [System.Serializable]
        struct YesNoDialog : Dialog
        {
            public GameObject Parent;
            public UILabel Message;
            public System.Action EndOfFunc;
            public System.Action YesOfFunc;
            public System.Action NoOfFunc;

            public void Open()
            {
                Parent.SetActive(true);
            }
            public void Yes()
            {
                YesOfFunc?.Invoke();
                Close();
            }
            public void No()
            {
                NoOfFunc?.Invoke();
                Close();
            }
            private void Close()
            {
                EndOfFunc?.Invoke();
                //Reset
                YesOfFunc = null;
                NoOfFunc = null;
                EndOfFunc = null;
                Parent.SetActive(false);
            }
        }
        #endregion

        //serialize param
        //[Header("")]
        //[SerializeField] private UITexture fadeImage;
        [Header("Camera")]
        [SerializeField] private Camera camera;
        [Header("Check Dialog")]
        [SerializeField] private CheckDialog checkDialog;
        [SerializeField] private YesNoDialog yesNoDialog;
        //private param
        //pubic param

        protected override void Awake()
        {
            base.Awake();
            //MEMO:現状Dummy.csの方が早く呼び出されるが、Order等で入れ替えることも考慮し判定しておく
            if (FixedAspectRatio.Aspect != (Define.c_FixedResolutionHeight / Define.c_FixedResolutionWidth))
            {
                FixedAspectRatio.Setup(Define.c_FixedResolutionWidth, Define.c_FixedResolutionHeight);
            }
            FixedAspectRatio.FitToWidth2D(camera);
            checkDialog.Parent.gameObject.SetActive(false);
            yesNoDialog.Parent.gameObject.SetActive(false);
        }

        public void Open(string message, System.Action callback = null)
        {
            checkDialog.Message.text = message;
            checkDialog.Callback = callback;
            checkDialog.Open();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="yes">yes選択時の処理</param>
        /// <param name="no">no選択時の処理</param>
        /// <param name="end">どちらかが選択された時の処理(共通)</param>
        public void Open(string message, System.Action yes, System.Action no, System.Action end = null)
        {
            yesNoDialog.Message.text = message;
            yesNoDialog.YesOfFunc = yes;
            yesNoDialog.NoOfFunc = no;
            yesNoDialog.EndOfFunc = end;
            yesNoDialog.Open();
        }

        #region Callback(インスペクタから指定するため参照は"0")
        public void Check()
        {
            checkDialog.Close();
        }

        public void Yes()
        {
            yesNoDialog.Yes();
        }

        public void No()
        {
            yesNoDialog.No();
        }
        #endregion
    }
}