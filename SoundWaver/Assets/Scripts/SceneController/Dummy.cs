using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Common;
using System.IO;
namespace Game.Setup
{
    public class Dummy : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(MainRoutine());
        }
        /// <summary>
        /// メインルーチン
        /// </summary>
        /// <returns></returns>
        IEnumerator MainRoutine()
        {
            Debug.Log("main routine in");
            //check
            foreach(var it in Define.c_PresetFilePath)
            {
                var path = Define.c_ChartSaveDirectory + Define.c_Delimiter + Path.GetFileName(it.Item2);
                //ない場合生成
                if (!File.Exists(path))
                {
                    Chart chart = new Chart();
                    yield return SubRoutine(path, it, chart);
                }
            }
            //遷移
            SceneManager.LoadScene("Start");
            yield break;
        }

        /// <summary>
        /// プリセット譜面.jsonから譜面を読み込み、書き出し。
        /// </summary>
        /// <param name="path">プリセットの譜面パス</param>
        /// <param name="chart">譜面</param>
        /// <returns></returns>
        IEnumerator SubRoutine(string path,(string,string) tuple, Chart chart)
        {
            Yuuki.FileIO.FileIO file = new Yuuki.FileIO.FileIO();
            var uwr = UnityWebRequest.Get(tuple.Item2);
            yield return uwr.SendWebRequest();
            Debug.Log("subroutine proccess:" + Time.deltaTime);
            chart = JsonUtility.FromJson<Chart>(uwr.downloadHandler.text);
            chart.FilePath = tuple.Item1;
            file.CreateFile(
                path,
                JsonUtility.ToJson(chart),
                Yuuki.FileIO.FileIO.FileIODesc.Overwrite
            );
        }
    }
}