using System.Collections;
using UnityEngine;
using Yuuki;
using API.Util;
using Common;
using UnityEngine.SceneManagement;
namespace Game
{

    public class LoadController : SingletonMonoBehaviour<LoadController>
    {
        //serialize param
        private float startTime;

        // Start is called before the first frame update
        void Start()
        {
            startTime = Time.time;
            FadeController.Instance.FadeOut(Define.c_FadeTime);
            StartCoroutine(MainRoutine());
        }

        IEnumerator MainRoutine()
        {
            //yield return new WaitForSeconds(1.0f);
            //楽曲ファイルのロード中は待機
            yield return new WaitWhile(
                () => { return GameMusic.Instance.Clip.loadState != AudioDataLoadState.Loaded; }
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
                SceneManager.LoadScene("Game");
            }
            //フェード終了後にロード完了
            else
            {
                FadeController.Instance.EventQueue.Enqueue(() => { UnityEngine.SceneManagement.SceneManager.LoadScene("Game"); });
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
                "楽曲が見つかりません。\nタイトルに戻ります",
                () => {
                             FadeController.Instance.EventQueue.Enqueue(() => { SceneManager.LoadScene("Start"); });
                             FadeController.Instance.FadeIn(Define.c_FadeTime);
                          }
                );
        }
    }
}
