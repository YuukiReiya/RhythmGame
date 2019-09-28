using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Yuuki.FileIO;
using TMPro;

public class FIOtest : MonoBehaviour
{
    public TextMeshProUGUI uGUI;
    public UILabel label;
    private void Start()
    {
    }
    private void Update()
    {
    }

    public void Execute()
    {
        uGUI.text = "";
        if (File.Exists(label.text))
        {
            uGUI.text = true.ToString();
        }
        else
        {
            uGUI.text = false.ToString();
        }
        label.text = "/storage/emulated/0/Android/data/com.Yuuki.MyS/files/";
    }

    [ContextMenu("b")]
    public void B()
    {
        var b = FileIO.GetContents(label.text);
        uGUI.text = b;
        label.text = "/storage/emulated/0/Android/data/com.Yuuki.MyS/files/";
        Debug.Log(b);
    }
}