using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using API.Util;
using Common;
using Game;
using Yuuki.FileIO;
using System.IO;
using DG.Tweening;
using UnityEngine.Networking;
#if UNITY_EDITOR
using Yuuki.MethodExpansions;
#endif
namespace Scenes
{
    public class ResultController : MonoBehaviour
    {
        //serialize param
        [SerializeField] private ResultScoreCanvas resultScoreCanvas;
        [Header("To Result")]
        [SerializeField] private ToResult resultDispObj;
        [System.Serializable]
        struct ToResult
        {
            public GameObject dispObj;
            public float tweenTime;
            public Vector2 endPos;
        }
        [System.Serializable]
        struct ChartData
        {
            public UITexture image;
            public UILabel NameLabel;
            public UILabel TimeLabel;
        }
        [Header("Chart Time")]
        [SerializeField] ChartData chartData;

        // Start is called before the first frame update
        void Start()
        {
            SaveClearData();
            SetupChartData();
            resultScoreCanvas.Setup();
            FadeController.Instance.EventQueue.Enqueue(
                () =>
                {
                    //フェード終了後にアニメーション
                    SetupToResultObject();
                    resultScoreCanvas.Execute();
                });
            FadeController.Instance.FadeOut(Define.c_FadeTime);
        }

        public void TransitionSelect()
        {
            FadeController.Instance.EventQueue.Enqueue(() => { UnityEngine.SceneManagement.SceneManager.LoadScene("SelectDev"); });
            FadeController.Instance.FadeIn(Define.c_FadeTime);
        }

        private void SaveClearData()
        {
            var chart = ChartManager.Chart;
            string path = string.Empty;

            //譜面ファイルのパスを求める

            //プリセット楽曲
            if(chart.isPreset)
            {
                //どのプリセットファイルなのか判定
                foreach (var preset in Define.c_PresetFilePath)
                {
                    //とりま音楽ファイルのパスで判定する
                    //TODO:なんとかしたい
                    if (preset.Item1 == chart.MusicFilePath)
                    {
                        path = Define.c_ChartSaveDirectory + Define.c_Delimiter + Path.GetFileName(preset.Item2);
                        break;
                    }
                    continue;
                }
            }
            //オリジナル楽曲
            else
            {
                path = Define.c_ChartSaveDirectory + Define.c_Delimiter + chart.ResistName + Define.c_JSON;
            }

            //パラメーターの更新
            chart.wasCleared = true;//クリア済みフラグON
            //ハイスコアが更新されれば保存
            if (chart.Score < Judge.score.Point)
            {
                chart.Score = Judge.score.Point;
                chart.ScoreCounts.Perfect = Judge.score.Perfect;
                chart.ScoreCounts.Great = Judge.score.Great;
                chart.ScoreCounts.Good = Judge.score.Good;
                chart.ScoreCounts.Miss = Judge.score.Miss;
                chart.Comb = Judge.score.MaxComb;
            }

            FileIO io = new FileIO();
            io.CreateFile(
                path,
                JsonUtility.ToJson(chart),
                FileIO.FileIODesc.Overwrite
                );
        }

        private void SetupToResultObject()
        {
            resultDispObj.dispObj.transform.DOLocalMove(resultDispObj.endPos, resultDispObj.tweenTime);
        }

        private void SetupChartData()
        {
            uint time = 0;
            AudioClip clip = null;
            uint minute = 0;
            uint second = 0;
#if UNITY_EDITOR
            if (GameMusic.Instance == null)
            {
                Debug.LogError("GameMusicのインスタンスが存在しません。");
                return;
            }
            if (!GameMusic.Instance.Clip)
            {
                //テストコード
                var path = Define.c_StreamingAssetsPath + Define.c_Delimiter + "Sounds\\" + Define.c_Delimiter + Path.GetFileNameWithoutExtension(Define.c_PresetFilePath[0].Item1) + Define.c_MP3;
                this.StartCoroutine(
                    GameMusic.Instance.LoadToAudioClip(path),
                    () =>
                    {
                        clip = GameMusic.Instance.Clip;
                        time = (uint)clip.length;
                        minute = time / 60;
                        second = time % 60;
                        chartData.TimeLabel.text = minute + "分" + second + "秒";
                    }
                    );
                return;
            }
#endif
            //画像
            StartCoroutine(LoadChartImage());
            //譜面名
            chartData.NameLabel.text = ChartManager.Chart.ResistName;
            //時間
            clip = GameMusic.Instance.Clip;
            time = (uint)clip.length;
            minute = time / 60;
            second = time % 60;
            chartData.TimeLabel.text = minute + "分" + string.Format("{0:D2}", second) + "秒";
        }

        private IEnumerator LoadChartImage()
        {
            using(var request = UnityWebRequestTexture.GetTexture(ChartManager.Chart.ImageFilePath))
            {
                yield return request.SendWebRequest();
                if (request.isNetworkError || request.isHttpError)
                {
                    yield break;
                }
                chartData.image.mainTexture = DownloadHandlerTexture.GetContent(request);
            }
            yield break;
        }
    }
}
