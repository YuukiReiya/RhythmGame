using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using Yuuki.MethodExpansions;
public class ResultScoreCanvas : MonoBehaviour
{
    [System.Serializable]
    struct ScoreLabels
    {
        public UILabel Perfect;
        public UILabel Great;
        public UILabel Good;
        public UILabel Miss;
        public UILabel MaxComb;
        public UILabel TotalScore;
        public UILabel HighScore;
    }
    [SerializeField] private ScoreLabels labels;
    [System.Serializable]
    struct SlotParam
    {
        public float SecondPerTime;
    }
    [SerializeField] SlotParam slotParam;

    [System.Serializable]
    struct ScoreTextures
    {
        public UITexture Perfect;
        public UITexture Great;
        public UITexture Good;
        public UITexture Miss;
        public UITexture MaxComb;
        public Vector3 MoveDirection;
        public float MovePt;
        public float MoveTime;
        public float AlphaTime;
    }
    public bool isAnimSkip;
    [SerializeField] ScoreTextures textures;

    //const param
    const string c_InitialLabelValue = "";

    public void Setup()
    {
        //ラベル
        labels.Perfect.text = c_InitialLabelValue;
        labels.Great.text = c_InitialLabelValue;
        labels.Good.text = c_InitialLabelValue;
        labels.Miss.text = c_InitialLabelValue;
        labels.MaxComb.text = c_InitialLabelValue;
        labels.TotalScore.text = c_InitialLabelValue;
        labels.HighScore.text = ChartManager.Chart.Score.ToString();
        //テクスチャ
        {
            isAnimSkip = false;
            var perfect = textures.Perfect;
            var great = textures.Great;
            var good = textures.Good;
            var miss = textures.Miss;
            var maxComb = textures.MaxComb;
            //alpha
            perfect.alpha = 0;
            great.alpha = 0;
            good.alpha = 0;
            miss.alpha = 0;
            maxComb.alpha = 0;

            //offset
            var offset = textures.MoveDirection.normalized * textures.MovePt;
            //pos
            perfect.transform.localPosition += offset;
            great.transform.localPosition += offset;
            good.transform.localPosition += offset;
            miss.transform.localPosition += offset;
            maxComb.transform.localPosition += offset;
            //フロートイン処理
            //    this.StartCoroutine(
            //        FloatInTexturesRoutine(perfect, perfect.transform.localPosition, perfect.transform.localPosition - offset),
            //        () =>
            //        {
            //            this.StartCoroutine(
            //                FloatInTexturesRoutine(great, great.transform.localPosition, great.transform.localPosition - offset),
            //                () =>
            //                {
            //                    this.StartCoroutine(
            //                        FloatInTexturesRoutine(good, good.transform.localPosition, good.transform.localPosition - offset),
            //                        () =>
            //                        {
            //                            this.StartCoroutine(
            //                                FloatInTexturesRoutine(miss, miss.transform.localPosition, miss.transform.localPosition - offset),
            //                                () =>
            //                                {
            //                                    this.StartCoroutine(FloatInTexturesRoutine(textures.MaxComb, maxComb.transform.localPosition, maxComb.transform.localPosition - offset));
            //                                }
            //                                );
            //                        }
            //                        );
            //                }
            //                );
            //        }
            //        );

            //パーフェクト
            this.StartCoroutine(
                AnimationRoutine(perfect, labels.Perfect, offset, Judge.score.Perfect, 1),
                () =>
                {
                    //グレイト
                    this.StartCoroutine(
                        AnimationRoutine(great, labels.Great, offset, Judge.score.Great, 1),
                        () =>
                        {
                            //グッド
                            this.StartCoroutine(
                                AnimationRoutine(good, labels.Good, offset, Judge.score.Good, 1),
                                () =>
                                {
                                    //ミス
                                    this.StartCoroutine(
                                        AnimationRoutine(miss, labels.Miss, offset, Judge.score.Miss, 1),
                                        () =>
                                        {
                                            //最大コンボ
                                            this.StartCoroutine(
                                                AnimationRoutine(maxComb, labels.MaxComb, offset, Judge.score.MaxComb, 1),
                                                () =>
                                                {
                                                    isAnimSkip = false;
                                                    //総計スコア
                                                    this.StartCoroutine(
                                                        SlotCountRoutine(labels.TotalScore, Judge.score.Point, 10),
                                                        //アニメーション終了時の処理
                                                        () =>
                                                        {

                                                        }
                                                        );
                                                });
                                        });
                                });
                        });
                });
        }
    }

    #region OnTap
    public void AnimationSkip()
    {
        isAnimSkip = true;
    }
    #endregion

    private IEnumerator AnimationRoutine(UITexture tex, UILabel label, Vector3 floatinOffset,uint endCount,uint add)
    {
        yield return FloatInTexturesRoutine(tex, tex.transform.localPosition, tex.transform.localPosition - floatinOffset);
        yield return SlotCountRoutine(label, endCount, add);
    }

    private IEnumerator FloatInTexturesRoutine(UITexture tex, Vector3 fromPos, Vector3 toPos)
    {
        var start = Time.time;
        while (true)
        {
            var elapsedTime = Time.time - start;
            //移動
            var moveRatio = elapsedTime / (textures.MoveTime != 0 ? textures.MoveTime : 1);
            var pos = Vector3.Lerp(fromPos, toPos, moveRatio);
            tex.transform.localPosition = pos;

            //色
            var alphaRatio = elapsedTime / (textures.AlphaTime != 0 ? textures.AlphaTime : 1);
            var alpha = Mathf.Lerp(0, 1, alphaRatio);
            tex.alpha = alpha;

            //フラグが立ったら抜ける
            //※画面タップによるスキップ用
            if (isAnimSkip) { break; }

            //移動と色の同条件が満たされたら抜ける
            if (moveRatio >= 1 && alphaRatio >= 1) { break; }
            yield return null;
        }
        //補正
        tex.alpha = 1;
        tex.transform.localPosition = toPos;
        isAnimSkip = false;

        yield break;
    }

    private IEnumerator SlotCountRoutine(UILabel label,uint retCount,uint addVal)
    {
        uint val = uint.Parse(label.text);
        while(true)
        {
            yield return new WaitForSeconds(slotParam.SecondPerTime);
            if (val >= retCount) { break; }
            if (isAnimSkip) { break; }
            val+=addVal;
            label.text = val.ToString();
        }
        val = retCount;
        label.text = val.ToString();
        yield break;
    }

    private uint GetDigits(uint number)
    {
        return (number == 0) ? 1 : ((uint)Mathf.Log10(number) + 1);
    }
}
