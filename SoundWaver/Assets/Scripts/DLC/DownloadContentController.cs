using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Yuuki.MethodExpansions;
using Yuuki.FileIO;
using Common;
using System.IO;
namespace Game.DLC
{
    /// <summary>
    /// さもサーバーから落としているように見せるためのDLC拡張スクリプト
    /// </summary>
    public class DownloadContentController : MonoBehaviour
    {
        [Header("Time")]
        [SerializeField] private float checkContentTime = 0.1f;
        [SerializeField] private float downloadTime = 10.0f;
        [SerializeField] private float endOfDownloadWaitTime = 0.2f;
        [Header("Download")]
        [SerializeField] private AnimationCurve curve;
        [SerializeField] private UITexture processBar;
        [SerializeField] private UIWidget window;
        //const param
        private static readonly string[] c_DLCNames = { "展示会用拡張パック.vol1" };
        private static readonly string c_DLCPath = Application.streamingAssetsPath + Define.c_Delimiter + "DLC" + Define.c_Delimiter;
        private static readonly (string, string, string)[] c_DLCTuple = 
        {
            //君がくれたもの
            (
                    c_DLCPath+"Secret Base"+Define.c_Delimiter+"kimigakuretamono"+Define.c_MP3,
                    c_DLCPath+"Secret Base"+Define.c_Delimiter+"~Secret Base~ 君がくれたもの"+Define.c_JSON,
                    c_DLCPath+"Secret Base"+Define.c_Delimiter+"image"+Define.c_PNG
             ),
            //銀色飛行船
            (
                    c_DLCPath+"Silver airship"+Define.c_Delimiter+"銀色飛行船"+Define.c_MP3,
                    c_DLCPath+"Silver airship"+Define.c_Delimiter+"銀色飛行船"+Define.c_JSON,
                    c_DLCPath+"Silver airship"+Define.c_Delimiter+"銀色飛行船"+Define.c_PNG
            ),
            //紅蓮華
            (
                    c_DLCPath+"Gurenhua"+Define.c_Delimiter+"紅蓮華"+Define.c_MP3,
                    c_DLCPath+"Gurenhua"+Define.c_Delimiter+"紅蓮華"+Define.c_JSON,
                    c_DLCPath+"Gurenhua"+Define.c_Delimiter+"紅蓮華"+Define.c_PNG
            ),
            //前々前世
            (
                    c_DLCPath+"Two years ago"+Define.c_Delimiter+"前々前世"+Define.c_MP3,
                    c_DLCPath+"Two years ago"+Define.c_Delimiter+"前々前世"+Define.c_JSON,
                    c_DLCPath+"Two years ago"+Define.c_Delimiter+"前々前世"+Define.c_PNG
            ),
            //打ち上げ花火
            (
                    c_DLCPath+"Uchiagehanabi"+Define.c_Delimiter+"DAOKO  米津玄師 打上花火"+Define.c_MP3,
                    c_DLCPath+"Uchiagehanabi"+Define.c_Delimiter+"打上花火"+Define.c_JSON,
                    c_DLCPath+"Uchiagehanabi"+Define.c_Delimiter+"打ち上げ花火、下から見るか？横から見るか？"+Define.c_PNG
            ),
            //花火
            (
                    c_DLCPath+"Fireworks"+Define.c_Delimiter+"aiko-花火"+Define.c_MP3,
                    c_DLCPath+"Fireworks"+Define.c_Delimiter+"花火"+Define.c_JSON,
                    c_DLCPath+"Fireworks"+Define.c_Delimiter+"花火"+Define.c_PNG
            ),
            //ピースサイン 
            (
                    c_DLCPath+"Peace sign"+Define.c_Delimiter+"ピースサイン"+Define.c_MP3,
                    c_DLCPath+"Peace sign"+Define.c_Delimiter+"ピースサイン"+Define.c_JSON,
                    c_DLCPath+"Peace sign"+Define.c_Delimiter+"ピースサイン"+Define.c_PNG
            ),
        };
        // Start is called before the first frame update
        void Start()
        {

        }

        public void Open()
        {
            DialogController.Instance.Open(
                "追加コンテンツを確認します。\nよろしいですか？",
                () =>
                {
                    //プログレスバーをリセットする
                    processBar.fillAmount = 0;
                    this.DelayMethod(
                        () =>
                        {
                            var content = string.Empty;
                            foreach (var it in c_DLCNames)
                            {
                                content += "・" + it + "\n";
                            }
                            DialogController.Instance.Open(
                    content + "\n\n以上が見つかりました。\nダウンロードしますか？",
                    () =>
                    {
                        Download();
                    }, null);
                        },
                        checkContentTime * Application.targetFrameRate
                        );
                }, null);
        }

        private void Download()
        {
            window.gameObject.SetActive(true);
            StartCoroutine(DownloadRoutine());
        }

        private IEnumerator DownloadRoutine()
        {
            var st = Time.time;

            //実際のDL処理
            {
                yield return MainProcess();
            }
            while (Time.time < st + downloadTime)
            {
                var elapsedTime = Time.time - st;
                var ratio = elapsedTime / (downloadTime == 0 ? 1 : downloadTime);
                var value = curve.Evaluate(ratio);
                processBar.fillAmount = value;
                yield return null;
            }
            //DL終了タイミング
            processBar.fillAmount = 1;
            yield return new WaitForSeconds(endOfDownloadWaitTime);
            window.gameObject.SetActive(false);
            DialogController.Instance.Open(
                "DLCのダウンロードが終了しました。"
                );
            yield break;
        }

        private IEnumerator MainProcess()
        {
            //
            foreach(var it in c_DLCTuple)
            {
                var path = Define.c_ChartSaveDirectory + Define.c_Delimiter + Path.GetFileName(it.Item2);
                if(File.Exists(path))
                {
                    using (var req = UnityWebRequest.Get(Define.c_LocalFilePath + path))
                    {
                        yield return req.SendWebRequest();
                        if(req.isNetworkError||req.isHttpError)
                        {
                            Debug.LogError("DLCController.cs line106 UnityWebRequest isNetworkingError OR isHttpError\n" + req.error);
                            ErrorManager.Save();
                        }
                        else
                        {
                            //譜面データに置換
                            var musicFilePath = JsonUtility.FromJson<Chart>(req.downloadHandler.text).MusicFilePath;
                            //設定されているパスを比較
                            if (musicFilePath.Contains(Define.c_StreamingAssetsPath)) { continue; }//正しいのでスキップ
                        }
                    }
                }
                var chart = new Chart();
                //"譜面がない"もしくは"キャッシュが異なる"ので生成しなおす
                yield return SubProcess(path, it, chart);
                continue;
            }
        }

        private IEnumerator SubProcess(string path, (string, string, string) tuple, Chart chart)
        {
            FileIO io = new FileIO();
            var req = UnityWebRequest.Get(tuple.Item2);
            yield return req.SendWebRequest();
            chart = JsonUtility.FromJson<Chart>(req.downloadHandler.text);

            //中身
            //※プリセット扱いにはしない
            chart.MusicFilePath = tuple.Item1;
            chart.ImageFilePath = tuple.Item3;

            //作成
            io.CreateFile(
                path,
                JsonUtility.ToJson(chart),
                FileIO.FileIODesc.Overwrite
            );

        }
    }
}