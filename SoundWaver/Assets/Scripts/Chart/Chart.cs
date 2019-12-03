[System.Serializable]
public struct Chart
{
    public string Title;
    /// <summary>
    /// 譜面データは一律同じ場所で連番で管理するようにする
    /// </summary>
    public string MusicFilePath;//楽曲ファイルが格納されているパス
    public string ImageFilePath;//イメージ画像が格納されているパス
    public uint BPM;
    public uint Comb;//ノーツカウント / コンボ
    public string ResistName;//登録名
    //public uint Number;//楽曲番号
    public uint Score;//スコア
    public bool wasCleared;//クリアしたか?
    public float Interval;
    [System.Serializable]
    public struct Note
    {
        public Note(float time, uint lane)
        {
            this.Time = time;
            this.LaneNumber = lane;
        }
        public float Time;
        public uint LaneNumber;
    }
    public Note[] Notes;

    [System.Serializable]
    public struct Counts
    {
        public uint Perfect;
        public uint Great;
        public uint Good;
        public uint Miss;
    }
    public Counts ScoreCounts;
}
