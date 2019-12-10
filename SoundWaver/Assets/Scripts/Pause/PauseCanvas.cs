using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.UI
{
    public class PauseCanvas : MonoBehaviour
    {
        //serialize param
        [SerializeField] GameObject pausePanel;
        [SerializeField] GameObject laneColliders;
        [SerializeField] UIButton pauseButton;
        [SerializeField] UILabel chartName;
        //private param
        AudioSource source;
        //public param
        //accessor
        public GameObject LaneColliders { get { return laneColliders; } }
        public UIButton PauseButton { get { return pauseButton; } }
        public void Setup(AudioSource source)
        {
            this.source = source;
            chartName.text = ChartManager.Chart.ResistName;
            pausePanel.SetActive(false);
        }

        /// <summary>
        /// TODO:UnPause関数でn秒間待機してカウント表示からの再開？
        /// </summary>
        public void Pause()
        {
            pausePanel.SetActive(true);
            source.pitch = 0;
            //レーンのコライダーを切って、タッチ判定を通らないようにする
            laneColliders.SetActive(false);
            pauseButton.isEnabled = false;
        }

        public void UnPause()
        {
            pausePanel.SetActive(false);
            Countdown.Instance.Execute(
                Define.c_WaitTimeCount,
                () =>
                {
                    source.pitch = 1;
                    //レーンのコライダーを入れなおす
                    laneColliders.SetActive(true);
                    pauseButton.isEnabled = true;
                }
                );
        }
    }
}