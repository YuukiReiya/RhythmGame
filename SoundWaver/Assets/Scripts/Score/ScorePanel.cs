using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    /// <summary>
    /// ゲーム中に表示されるスコア表示のUI
    /// </summary>
    public class ScorePanel : MonoBehaviour
    {
        [SerializeField] UILabel scoreLabel;

        public void Setup()
        {
            StartCoroutine(MainRoutine());
        }

        private IEnumerator MainRoutine()
        {
            while (true)
            {
                scoreLabel.text = Judge.score.Point.ToString();
                yield return null;
            }
        }
    }
}