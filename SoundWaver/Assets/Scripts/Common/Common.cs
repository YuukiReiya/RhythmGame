using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common 
{
    public static class Define
    {
        /// <summary>
        /// レーン数
        /// </summary>
        public const uint c_LaneCount = 3;

        public const float c_PerfectTime = 0.033f;
        public const float c_GreatTime = 0.1f;
        public const float c_GoodTime = 0.2f;

        /// <summary>
        /// ローカルファイル指定用のパス
        /// </summary>
        public const string c_LocalFilePath = "file:///";
        
        /// <summary>
        /// MP3拡張子
        /// </summary>
        public const string c_MP3 = ".mp3";
        /// <summary>
        /// WAV拡張子
        /// </summary>
        public const string c_WAV = ".wav";
        /// <summary>
        /// .iniファイル名
        /// 読み込み先は"Application.persistentDataPath"
        /// </summary>
        public static readonly string c_SettingFilePath = Application.persistentDataPath + "\\" + Application.productName + ".ini";
    }
}
