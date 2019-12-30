using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using API.Util;
using Common;
using Game.UI;
namespace Game
{
    public class SelectController : MonoBehaviour
    {
        //serialize param
        [SerializeField] private GameObject refinePanel;
        [SerializeField] private ChartDeleter deletePanel;
        //privtae param

        //public param

        // Start is called before the first frame update
        void Start()
        {
            //初期化
            ChartManager.Instance.Setup();

            //絞り込みパネルの非表示
            CloseRefine();

            //削除パネルの非表示
            CloseDelete();

            //楽曲リストの表示
            FadeController.Instance.FadeOut(Common.Define.c_FadeTime);
            ChartManager.Instance.LoadToDisplay();
        }

        public void TransitionGame()
        {
            FadeController.Instance.EventQueue.Enqueue(() => { SceneManager.LoadScene("Load"); });
            FadeController.Instance.FadeIn(Common.Define.c_FadeTime);
        }

        public void TransitionStart()
        {
            FadeController.Instance.EventQueue.Enqueue(() => { SceneManager.LoadScene("StartDev"); });
            FadeController.Instance.FadeIn(Common.Define.c_FadeTime);
        }

        public void OpenRefine()
        {
            refinePanel.SetActive(true);
        }

        public void CloseRefine()
        {
            refinePanel.SetActive(false);
        }

        public void OpenDelete()
        {
            deletePanel.gameObject.SetActive(true);
        }

        public void CloseDelete()
        {
            deletePanel.gameObject.SetActive(false);
            deletePanel.DestroyScrollChildren();
        }

        /// <summary>
        /// ゲーム開始
        /// </summary>
        public void Play()
        {
            FadeController.Instance.EventQueue.Enqueue(
             () =>
                {
                    SceneManager.LoadScene("Load");
                    //MEMO:シーン遷移間にコルーチンを回すので、DontDestroyObjectでStartCoroutineをする必要がある
                    var chart = ChartManager.Chart;
                    GameMusic.Instance.StartCoroutine(GameMusic.Instance.LoadToAudioClip(chart.MusicFilePath));
                }
            );
            FadeController.Instance.FadeIn(Define.c_FadeTime);
        }
    }
}