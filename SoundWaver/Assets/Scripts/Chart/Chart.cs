[System.Serializable]
public struct Chart
{
    public string Title;
    /// <summary>
    /// 譜面データは一律同じ場所で連番で管理するようにする
    /// </summary>
    public string FilePath;//楽曲ファイルが格納されているパス
    public uint BPM;
    public uint Comb;//ノーツカウント / コンボ
    //public string ResistName;//登録名
    //public uint Number;//楽曲番号
    public float Interval;
    //TODO:小節、拍子を入れる構造体に置換
    //[System.Serializable]
    //public struct Info
    //{
    //    public uint s;
    //    public uint b;
    //}
    //public Info[] isb;
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
}
