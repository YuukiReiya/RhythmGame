using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Common;
using UnityEngine.Networking;
using Yuuki.MethodExpansions;
using UnityEngine.SceneManagement;

namespace Game
{
    /// <summary>
    /// ゲーム中に流れる楽曲管理
    /// </summary>
    public class GameMusic : Yuuki.SingletonMonoBehaviour<GameMusic>
    {
        //  serialize param

        //  private param
        private IEnumerator routine;
        //  accessor
        public AudioSource Source { get; set; }
        public AudioClip Clip { get; private set; }

        /// <summary>
        /// 楽曲をロードした後、再生する。
        /// </summary>
        /// <param name="path"></param>
        public void LoadAndPlayAudioClip(string path)
        {
            LoadAndFunction(
                path,
                () =>
                {
                    Source.clip = Clip;
                    Source.Play();
                }
                );
        }

        /// <summary>
        /// ロードに成功したら関数を実行する
        /// </summary>
        /// <param name="path"></param>
        /// <param name="successFunction"></param>
        public void LoadAndFunction(string path,CoroutineExpansion.CoroutineFinishedFunc successFunction)
        {
            ResetCoroutine();
            routine = LoadToAudioClip(path);
            this.StartCoroutine(routine, successFunction);
        }

        /// <summary>
        /// AudioClipに対象の音楽ファイルをロード
        /// 成功後Clipにアクセスすることで取得可能
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IEnumerator LoadToAudioClip(string path)
        {
            Clip = null;
            var type = GetAudioType(path);
            using (var request = UnityWebRequestMultimedia.GetAudioClip(path,type))
            {
                //リクエスト送信
                yield return request.SendWebRequest();
                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.LogError("GameMusic.cs line69: UnityWebRequest Error\n" + request.error);
                    ErrorManager.Save();
                    DialogController.Instance.Open(
                        DialogController.Type.Check,
                        "楽曲の読み込みに失敗しました。\nタイトルに戻ります。",
                        () => { SceneManager.LoadScene("Start"); }
                        );
                    yield break;
                }
                Clip = ((DownloadHandlerAudioClip)request.downloadHandler).audioClip;
            }
        }

        AudioType GetAudioType(string filePath)
        {
            var extension = Path.GetExtension(filePath);
            if (extension == Define.c_MP3) { return AudioType.MPEG; }
            else if(extension == Define.c_WAV) { return AudioType.WAV; }
            return AudioType.UNKNOWN;
        }

        void ResetCoroutine()
        {
            if (routine != null) { StopCoroutine(routine); }
            routine = null;
        }
    }
}