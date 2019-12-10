using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using API.Util;
using Common;
using Game;
using Yuuki.FileIO;
using System.IO;
namespace Scenes
{
    public class ResultController : MonoBehaviour
    {
        //serialize param
        [SerializeField] private ResultScoreCanvas resultScoreCanvas;
        // Start is called before the first frame update
        void Start()
        {
            resultScoreCanvas.Setup();
            SaveClearData();
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
    }
}
