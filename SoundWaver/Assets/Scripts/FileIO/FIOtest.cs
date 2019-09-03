using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class FIOtest : MonoBehaviour
{
    StreamWriter sw;
    public void Setup (string path)
    {
        sw = new StreamWriter (path, true);
    }

    public void Write (string elem)
    {
        sw.WriteLine (elem);
    }

    public void Shutdown ()
    {
        sw.Close ();
    }
}