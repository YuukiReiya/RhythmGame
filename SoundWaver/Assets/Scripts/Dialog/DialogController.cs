using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogController : Yuuki.SingletonMonoBehaviour<DialogController>
{
    #region 構造体_宣言
    [System.Serializable]
    struct CheckDialog
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
    struct YesNoDialog
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
    public enum Type
    {
        Check,
        YesNo,
    }

    //serialize param
    //[Header("")]
    //[SerializeField] private UITexture fadeImage;
    [Header("Check Dialog")]
    [SerializeField] private CheckDialog checkDialog;
    [SerializeField] private YesNoDialog yesNoDialog;
    //private param
    Type type;
    //pubic param

    public void Open(Type type, string message, System.Action callback = null)
    {
        this.type = type;
        checkDialog.Message.text = message;
        checkDialog.Callback = callback;
        checkDialog.Open();
    }

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
}
