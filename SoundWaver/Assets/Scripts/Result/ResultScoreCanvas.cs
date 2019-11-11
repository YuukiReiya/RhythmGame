using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
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
    }
    [SerializeField] private ScoreLabels labels;

    public void Setup()
    {
        labels.Perfect.text = Judge.score.Perfect.ToString();
        labels.Great.text = Judge.score.Great.ToString();
        labels.Good.text = Judge.score.Good.ToString();
        labels.Miss.text = Judge.score.Miss.ToString();
        labels.MaxComb.text = Judge.score.MaxComb.ToString();
    }
}
