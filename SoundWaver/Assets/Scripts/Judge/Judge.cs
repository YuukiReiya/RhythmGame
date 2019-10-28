using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using System;

namespace Game
{
    /// <summary>
    /// 判定処理
    /// </summary>
    public class Judge
    {
        //決定表で用いる疑似テーブル
        private static readonly Dictionary<ScoreController.Judge, float> c_Pair = new Dictionary<ScoreController.Judge, float>() {
            { ScoreController.Judge.PERFECT,Define.c_PerfectTime },
            { ScoreController.Judge.GREAT,Define.c_GreatTime },
            { ScoreController.Judge.GOOD,Define.c_GoodTime },
        };

        /// <summary>
        /// 実行処理
        /// </summary>
        /// <param name="time">判定時間</param>
        public static void Execute(float time)
        {
            var absTime = Mathf.Abs(time);
            //決定表のどれにも当てはまらなければ"MISS"にするための初期化
            ScoreController.Judge ret = ScoreController.Judge.MISS;
            //C#版 自作決定表
            foreach(var it in c_Pair)
            {
                if (absTime <= it.Value)
                {
                    ret = it.Key;
                    break;
                }
            }
            //コンボ
            switch(ret)
            {
                //"PERFECT"と"GREAT"なら加算
                case ScoreController.Judge.PERFECT:
                case ScoreController.Judge.GREAT:
                    GameController.Instance.Comb++;
                    CombEffectCanvas.Instance.Execute(GameController.Instance.Comb);
                    break;
                //"GOOD"又は"MISS"ならリセット
                case ScoreController.Judge.GOOD:
                case ScoreController.Judge.MISS:
                    GameController.Instance.Comb = 0;
                    CombEffectCanvas.Instance.Stop();
                    break;
            }
            //判定エフェクト
            ScoreController.Instance.StartScoreEffect(ret);
        }
    }
}