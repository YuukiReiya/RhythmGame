using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using System;
using count =System.UInt32;

namespace Game
{
    /// <summary>
    /// 判定処理
    /// </summary>
    public class Judge
    {
        //決定表で用いる疑似テーブル
        private static readonly Dictionary<ScoreEffectCanvas.Judge, float> c_Pair = new Dictionary<ScoreEffectCanvas.Judge, float>() {
            { ScoreEffectCanvas.Judge.PERFECT,Define.c_PerfectTime },
            { ScoreEffectCanvas.Judge.GREAT,Define.c_GreatTime },
            { ScoreEffectCanvas.Judge.GOOD,Define.c_GoodTime },
        };

        public struct Score
        {
            public count Comb;
            public count MaxComb;
            public count Perfect;
            public count Great;
            public count Good;
            public count Miss;
        }
        public static Score score;

        public static void Reset()
        {
            score.Comb = 0;
            score.MaxComb = 0;
            score.Perfect = 0;
            score.Great = 0;
            score.Good = 0;
            score.Miss = 0;
        }

        /// <summary>
        /// 実行処理
        /// </summary>
        /// <param name="time">判定時間</param>
        public static void Execute(float time)
        {
            var absTime = Mathf.Abs(time);
            //決定表のどれにも当てはまらなければ"MISS"にするための初期化
            ScoreEffectCanvas.Judge ret = ScoreEffectCanvas.Judge.MISS;
            //C#版 自作決定表
            foreach(var it in c_Pair)
            {
                if (absTime <= it.Value)
                {
                    ret = it.Key;
                    break;
                }
            }

            //カウント
            CountUp(ret);

            //コンボ
            switch (ret)
            {
                //"PERFECT"と"GREAT"なら加算
                case ScoreEffectCanvas.Judge.PERFECT:
                case ScoreEffectCanvas.Judge.GREAT:
                    score.Comb++;
                    CombEffectCanvas.Instance.Execute(score.Comb);
                    break;
                //"GOOD"又は"MISS"ならリセット
                case ScoreEffectCanvas.Judge.GOOD:
                case ScoreEffectCanvas.Judge.MISS:
                    score.Comb = 0;
                    CombEffectCanvas.Instance.Stop();
                    break;
            }

            //最大コンボ
            score.MaxComb = score.Comb > score.MaxComb ? score.Comb : score.MaxComb;

            //判定エフェクト
            ScoreEffectCanvas.Instance.StartScoreEffect(ret);
        }
        private static void CountUp(ScoreEffectCanvas.Judge judge)
        {
            switch (judge)
            {
                case ScoreEffectCanvas.Judge.PERFECT:score.Perfect++;break;
                case ScoreEffectCanvas.Judge.GREAT:score.Great++;break;
                case ScoreEffectCanvas.Judge.GOOD:score.Good++;break;
                case ScoreEffectCanvas.Judge.MISS:score.Miss++;break;
            }
        }
    }
}