using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Yuuki;
public class ChartManager : SingletonMonoBehaviour<ChartManager>
{
    //serialize param
    [SerializeField] GameObject chartPrefab;
    [SerializeField] UIGrid grid;
    //private param

    //const param
    readonly string c_ChartPath = Application.streamingAssetsPath + "\\Charts";

    // Start is called before the first frame update
    void Start()
    {
        Create();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Create()
    {
        //var charts = Directory.GetFiles(c_ChartPath, Common.Define.c_JSON);
        var charts = Directory.GetFiles(c_ChartPath, "*" + Common.Define.c_JSON);
        foreach(var it in charts)
        Debug.Log(it);

        Yuuki.FileIO.FileIO io = new Yuuki.FileIO.FileIO();
        foreach(var chart in charts)
        {
            var inst = Instantiate(chartPrefab);
            inst.transform.parent = grid.transform;
            inst.transform.localScale = chartPrefab.transform.localScale;
            var data = JsonUtility.FromJson<Chart>(io.GetContents(chart));
            var proxy = inst.GetComponent<ChartProxy>();
            proxy.SetupChart(data);
        }
    }
}
