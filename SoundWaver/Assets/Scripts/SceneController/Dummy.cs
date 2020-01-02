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
        private void Awake()
        {
            ErrorManager.Setup();
        }
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
            //プリセットの譜面ファイルの確認
            foreach(var it in Define.c_PresetFilePath)
            {
                //読み込むパス
                var path = Define.c_ChartSaveDirectory + Define.c_Delimiter + Path.GetFileName(it.Item2);
                //check
                if (File.Exists(path))
                {
                    using (var www = UnityWebRequest.Get(path))
                    {
                        //通信終了まで待機
                        yield return www.SendWebRequest();
                        if (www.isNetworkError || www.isHttpError)
                        {
                            Debug.LogError("Dummy.cs line41 UnityWebRequest isNetworkingError OR isHttpError\n" + www.error);
                            ErrorManager.Save();
                        }
                        else
                        {
                            //譜面データに置換
                            var musicFilePath = JsonUtility.FromJson<Chart>(www.downloadHandler.text).MusicFilePath;
                            //設定されているパスを比較
                            if (musicFilePath.Contains(Define.c_StreamingAssetsPath)) { continue; }//正しいのでスキップ
                        }
                    }
                }
                var chart = new Chart();
                //"譜面がない"もしくは"キャッシュが異なる"ので生成しなおす
                yield return SubRoutine(path, it, chart);
                continue;
            }
            //遷移
            SceneManager.LoadScene("StartDev");
            yield break;
        }

        /// <summary>
        /// プリセット譜面.jsonから譜面を読み込み、書き出し。
        /// </summary>
        /// <param name="path">プリセットの譜面パス</param>
        /// <param name="chart">譜面</param>
        /// <returns></returns>
        IEnumerator SubRoutine(string path, (string, string, string) tuple, Chart chart)
        {
            //プリセットの読み込み
            Yuuki.FileIO.FileIO file = new Yuuki.FileIO.FileIO();
            var uwr = UnityWebRequest.Get(tuple.Item2);
            yield return uwr.SendWebRequest();
            chart = JsonUtility.FromJson<Chart>(uwr.downloadHandler.text);

            //中身
            chart.MusicFilePath = tuple.Item1;
            chart.ImageFilePath = tuple.Item3;
            chart.isPreset = true;//プリセットの譜面であるという証明

            //作成
            file.CreateFile(
                path,
                JsonUtility.ToJson(chart),
                Yuuki.FileIO.FileIO.FileIODesc.Overwrite
            );
        }
    }
}