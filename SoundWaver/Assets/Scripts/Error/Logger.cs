using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yuuki;
namespace API.Log
{
    /// <summary>
    /// UnityAPIのコールバック処理の登録
    /// </summary>
    public class Logger : SingletonMonoBehaviour<Logger>
    {
        /// <summary>
        /// ゲーム中断時にログを記録
        /// </summary>
        /// <param name="pause"></param>
        private void OnApplicationPause(bool pause)
        {
            ErrorManager.Save();
        }

        /// <summary>
        /// ゲーム終了時にログを記録
        /// </summary>
        private void OnApplicationQuit()
        {
            ErrorManager.Save();
        }
    }
}