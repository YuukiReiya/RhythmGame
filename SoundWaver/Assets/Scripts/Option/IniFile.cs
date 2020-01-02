using System.Collections.Generic;
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

        /// <summary>
        /// ノーツ速度の対応リスト
        /// UIの10段階表記を実際の値に対応付ける。
        /// ※定数でいいが実機で遊びながら調整するため変数として用意している。
        /// 
        /// TODO:もう少し綺麗にしたい
        /// </summary>
        public float[] NotesSpeedList;

        public void Setup()
        {
            this.CurrentPath = Define.c_InitialCurrentPath;
            this.BGMVol = Define.c_InitialVol;
            this.SEVol = Define.c_InitialVol;
            this.NotesSpeed = Define.c_InitialNotesSpeed;
            //TODO:モバイル端末でLinqは使えないので、自前で求める
            Queue<float> queue = new Queue<float>();
            foreach(var it in Define.c_NotesSpeedList) { queue.Enqueue(it.Item3); }
            NotesSpeedList = queue.ToArray();
        }
    }
}