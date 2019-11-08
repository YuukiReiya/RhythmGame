using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogController : Yuuki.SingletonMonoBehaviour<DialogController>
{

    #region 構造体_宣言
    interface Dialog { }
    [System.Serializable]
    struct CheckDialog:Dialog
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
    struct YesNoDialog:Dialog
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
    //pubic param

    public void Open(Type type, string message, System.Action callback = null)
    {
        switch (type)
        {
            case Type.Check:
                checkDialog.Message.text = message;
                checkDialog.Callback = callback;
                checkDialog.Open();
                break;
            case Type.YesNo:
                yesNoDialog.Message.text = message;
                yesNoDialog.EndOfFunc = callback;
                yesNoDialog.Open();
                break;
            default:
                break;
        }
    }

    public void Open(string message,System.Action yes,System.Action no,System.Action end = null)
    {
        yesNoDialog.Message.text = message;
        yesNoDialog.YesOfFunc = yes;
        yesNoDialog.NoOfFunc = no;
        yesNoDialog.EndOfFunc = end;
        yesNoDialog.Open();
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