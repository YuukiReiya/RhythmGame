using UnityEngine;
namespace Common
{
    public struct IniFile
    {
        /// <summary>
        /// 譜面生成の際に参照する相対パス
        /// ※楽曲と画像で同じ参照でいい??必要なら分ける
        /// </summary>
        public string CurrentPath;

        public uint BGMVol;
        public uint SEVol;
        public uint NotesSpeed;
    }
}