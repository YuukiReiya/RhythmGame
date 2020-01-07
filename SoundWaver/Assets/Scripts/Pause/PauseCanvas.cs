using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using Game.Audio;

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
        public System.Action pauseAction;
        public System.Action unPauseAction;
        //accessor
        public GameObject LaneColliders { get { return laneColliders; } }
        public UIButton PauseButton { get { return pauseButton; } }
        public void Setup(AudioSource source,System.Action pauseAction,System.Action unPauseAction)
        {
            this.source = source;
            chartName.text = ChartManager.Chart.ResistName;
            pausePanel.SetActive(false);
            this.pauseAction = pauseAction;
            this.unPauseAction = unPauseAction;
        }

        /// <summary>
        /// TODO:UnPause関数でn秒間待機してカウント表示からの再開？
        /// </summary>
        public void Pause()
        {
            AudioManager.Instance.PlaySE("Pause_Open");
            pausePanel.SetActive(true);
            source.pitch = 0;
            //レーンのコライダーを切って、タッチ判定を通らないようにする
            laneColliders.SetActive(false);
            pauseButton.isEnabled = false;

            //コールバック
            pauseAction?.Invoke();
        }

        public void UnPause()
        {
            AudioManager.Instance.PlaySE("Pause_Close");
            pausePanel.SetActive(false);
            Countdown.Instance.Execute(
                Define.c_WaitTimeCount,
                () =>
                {
                    source.pitch = 1;
                    //レーンのコライダーを入れなおす
                    laneColliders.SetActive(true);
                    pauseButton.isEnabled = true;

                    //コールバック
                    unPauseAction?.Invoke();
                }
                );
        }
    }
}