using System.Collections;
using System;
using UnityEngine;
using Yuuki.MethodExpansions;

namespace Game
{
    /// <summary>
    /// カウントダウン用のスクリプト
    /// ポーズ解除(アンポーズ)とゲーム開始時で使いたかったのでクラス分け
    /// 
    /// シングルトンのがコスパがよかったのでシングルトンで!
    /// </summary>
    public class Countdown : Yuuki.SingletonMonoBehaviour<Countdown>
    {
        [SerializeField] float timePerSecond;//この秒数を一秒で計算
        [SerializeField] UIWidget widget;//ラベルの表示を制御するウィジェット(α値の制御で行う)
        [SerializeField] UILabel countLabel;//現在のカウントを表示するUILabel
        //accessor
        public UIWidget Widget { get { return widget; } }
        public void Execute(uint second,Action callback = null)
        {
            this.StartCoroutine(MainRoutine(second), () => { callback?.Invoke(); });
        }

        private IEnumerator MainRoutine(uint second)
        {
            widget.alpha = 1;
            uint count = 0;
            while (count < second)
            {
                //1秒経過時の処理
                //TODO:スプライトorラベル値変更
                ElapsedSecondProcess((second - count));
                var time = Time.time;
                while (Time.time < time + timePerSecond)
                {
                    yield return null;
                }
                count++;
            }
            //指定待機時間の終了
            widget.alpha = 0;
            countLabel.text = string.Empty;
            yield break;
        }

        /// <summary>
        /// 一秒経過時の処理
        /// </summary>
        /// <param name="currentCount"></param>
        private void ElapsedSecondProcess(uint count)
        {
            countLabel.text = (count).ToString();//引数から"1"引いた値を表示する
        }
    }
}