using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Common 
{
    public static class Define
    {
        public const float c_FadeTime = 0.25f;
        /// <summary>
        /// ゲーム開始時とアンポーズ時の待機時間(整数で)
        /// ※1秒に割り当てられる時間はCountdwonスクリプトのインスペクタからどうぞ!
        /// </summary>
        public const uint c_WaitTimeCount = 3;

        public const uint c_MaxVolume = 10;
        public const uint c_MinVolume = 0;
        public const uint c_MaxNoteSpeed = 10;
        public const uint c_MinNoteSpeed = 1;
        public static readonly string c_InitialCurrentPath = Application.persistentDataPath;
        public const uint c_InitialVol = 10;
        public const uint c_InitialNotesSpeed = 6;

        /// <summary>
        /// レーン数
        /// </summary>
        public const uint c_LaneCount = 3;

        public const float c_PerfectTime = 0.033f;
        public const float c_GreatTime = 0.1f;
        public const float c_GoodTime = 0.2f;
        /// <summary>
        /// 判定時の加算スコアポイント
        /// </summary>
        public const uint c_PerfectAddPoint = 500;
        public const uint c_GreatAddPoint = 100;
        public const uint c_GoodAddPoint = 50;
        public const uint c_MissAddPoint = 0;
        #region プリセットファイルのパス
        /// <summary>
        /// プリセットファイルのパス
        /// item1:楽曲パス
        /// item2:譜面パス
        /// item3:イメージパス
        /// </summary>
        public static readonly (string, string,string)[] c_PresetFilePath = 
            { 
                //ハルジオン
                (
                    Application.streamingAssetsPath+c_Delimiter+ "Sounds"+c_Delimiter + "short_song_kei_harujion"+c_MP3,
                    Application.streamingAssetsPath+c_Delimiter+ "PresetCharts"+c_Delimiter + "Harujion"+c_JSON,
                    Application.streamingAssetsPath+c_Delimiter+"Image"+c_Delimiter+"Harujion"+c_PNG
                ),
                //シャイニングスター
                (
                    Application.streamingAssetsPath+c_Delimiter+ "Sounds"+c_Delimiter + "short_song_shiho_shining_star"+c_MP3,
                    Application.streamingAssetsPath+c_Delimiter+ "PresetCharts"+c_Delimiter + "ShiningStar"+c_JSON,
                    Application.streamingAssetsPath+c_Delimiter+"Image"+c_Delimiter+"ShiningStar"+c_PNG
                ),
        };
        #endregion

        /// <summary>
        /// ローカルファイル指定用のパス
        /// </summary>
        public const string c_LocalFilePath = "file:///";
        /// <summary>
        /// JSON拡張子
        /// </summary>
        public const string c_JSON = ".json";
        public const string c_PNG = ".png";
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
        public static string c_StreamingAssetsPath = Application.streamingAssetsPath;
        public static readonly string c_ChartSaveDirectory = c_StreamingAssetsPath + "\\Charts";
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
        public static string c_StreamingAssetsPath = "jar:file://" + Application.dataPath + "!/assets";
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
