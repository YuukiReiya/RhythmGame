﻿using System.Collections;
using UnityEngine;
using Yuuki;
using API.Util;
using Common;
using UnityEngine.SceneManagement;
using System.IO;
using Game.UI;
namespace Game
{
    public class LoadController : SingletonMonoBehaviour<LoadController>
    {
        //serialize param
        [SerializeField] private Camera camera2D;

        private float startTime;

        // Start is called before the first frame update
        void Start()
        {
#if UNITY_EDITOR
            FixedAspectRatio.Setup(Define.c_FixedResolutionWidth, Define.c_FixedResolutionHeight);
#endif
            //アスペクト比変更
            FixedAspectRatio.FitToWidth2D(camera2D);

            startTime = Time.time;
            FadeController.Instance.FadeOut(Define.c_FadeTime);
            StartCoroutine(MainRoutine());
        }

        IEnumerator MainRoutine()
        {
            #region パラメータの初期化
            //ノーツの初期化
            Judge.Reset();
            NotesController.Instance.SetupNotesData();
            NotesController.Instance.SetupNoteSpeed();
            //ノーツの速度を設定し終わるまで待機
            yield return new WaitUntil(
                () => { return NotesController.Instance.IsEndOfNoteSpeedLoading; }
                );
            #endregion

            //クリップオブジェクトの生成まで待機
            yield return new WaitUntil(
                () => { return GameMusic.Instance.Clip; }
                );
            //楽曲ファイルのロード中は待機
            yield return new WaitWhile(
                () => { return GameMusic.Instance.Clip.loadState == AudioDataLoadState.Loading; }
                );
            //ロード状況によって切り替える
            switch (GameMusic.Instance.Clip.loadState)
            //switch(AudioDataLoadState.Failed)
            {
                //読み込み成功
                case AudioDataLoadState.Loaded: 
                    OnLoadSuccess();
                    break;
                //読み込み失敗
                case AudioDataLoadState.Failed:
                case AudioDataLoadState.Unloaded:
                    OnLoadFailed();
                    break;
            }
        }

        /// <summary>
        /// ロード成功処理
        /// </summary>
        private void OnLoadSuccess()
        {
            var time = Time.time;
            //フェード中にロードが完了してしまった場合
            if ((time - startTime) < Define.c_FadeTime)
            {
                SceneManager.LoadScene("GameDev");
            }
            //フェード終了後にロード完了
            else
            {
                FadeController.Instance.EventQueue.Enqueue(() => { SceneManager.LoadScene("GameDev"); });
                FadeController.Instance.FadeIn(Define.c_FadeTime);
            }
        }

        /// <summary>
        /// ロード失敗処理
        /// </summary>
        private void OnLoadFailed()
        {
            FadeController.Instance.Stop();
            DialogController.Instance.Open(
                "読み込みに失敗しました。\nタイトルに戻ります",
                () => {
                             FadeController.Instance.EventQueue.Enqueue(() => { SceneManager.LoadScene("StartDev"); });
                             FadeController.Instance.FadeIn(Define.c_FadeTime);
                          }
                );
        }
    }
}
