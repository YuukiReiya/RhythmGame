using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class Read : MonoBehaviour
{
    public List<float> values;
    void Start ()
    { }

    public void Execute (string path)
    {
        var file = new FileStream (path, FileMode.Open, FileAccess.Read);
        try
        {
            StreamReader sr = new StreamReader (file);
            while (sr.EndOfStream == false)
            {
                string line = sr.ReadLine ();
                values.Add (float.Parse (line));
            }
        }
        catch
        {

        }
        finally
        {

        }
    }

    // Update is called once per frame
    void Update ()
    {

    }
}