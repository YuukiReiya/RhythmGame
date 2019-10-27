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
            Parent.SetActive(false);
        }
    }
    #endregion
    //serialize param
    //[Header("")]
    //[SerializeField] private UITexture fadeImage;
    [Header("Check Dialog")]
    [SerializeField] private CheckDialog checkDialog;
    //private param
    //pubic param
  
    public void Open(string message, System.Action callback = null)
    {
        checkDialog.Message.text = message;
        checkDialog.Callback = callback;
        checkDialog.Open();
    }

    public void Close()
    {
        checkDialog.Close();
    }
}
