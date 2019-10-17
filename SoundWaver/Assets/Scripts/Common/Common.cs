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
        /// JSON拡張子
        /// </summary>
        public const string c_JSON = ".json";
        /// <summary>
        /// MP3拡張子
        /// </summary>
        public const string c_MP3 = ".mp3";
        /// <summary>
        /// WAV拡張子
        /// </summary>
        public const string c_WAV = ".wav";

#if UNITY_EDITOR
        public const string c_Delimiter = "\\";
        public static readonly string c_ChartSaveDirectory = Application.streamingAssetsPath + "\\Charts";
        public static readonly string c_SettingFilePath = Application.persistentDataPath + c_Delimiter + Application.productName + ".ini";
#elif UNITY_ANDROID
        /// <summary>
        /// ファイル(ディレクトリ)の区切り文字
        /// </summary>
        public const string c_Delimiter = "/";
        /// <summary>
        /// StreamingAssetsPathの代替アクセスパス
        /// R:読み取り専用
        /// </summary>
        private static string c_StreamingAssetsPath = "jar:file//" + Application.dataPath + "!/assets";
        /// <summary>
        /// .iniファイル名
        /// 読み込み先は"Application.persistentDataPath"
        /// </summary>
        public static readonly string c_SettingFilePath = Application.persistentDataPath + c_Delimiter + Application.productName + ".ini";
        /// <summary>
        /// 作成した譜面の保存ディレクトリ
        /// </summary>
        public static readonly string c_ChartSaveDirectory = Application.persistentDataPath + "/Charts";
#endif
    }
}
