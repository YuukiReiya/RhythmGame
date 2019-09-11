using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class AMSReader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Read()
    {
        var file = new FileStream("specDatas.txt", FileMode.Open, FileAccess.Read);
        var sr = new StreamReader(file);
        List<float> buffer = new List<float>();
        while (sr.EndOfStream != true)
        {
            string line = sr.ReadLine();
            buffer.Add(float.Parse(string.IsNullOrEmpty(line) ? "0" : line));
        }

        //  リスト完成
        int bpm = 158;
        float inttime = 60 / bpm * 4;
        for(int i=0;i<buffer.Count;++i)
        {

        }

    }
}
