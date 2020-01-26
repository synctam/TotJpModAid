namespace LibTotModMaker.TransSheet
{
    /// <summary>
    /// 翻訳シートエントリー
    /// </summary>
    public class TotTransSheetEntry : TotTransSheetEntryBase
    {
        public TotTransSheetEntry() { }

        public TotTransSheetEntry(string key, string text)
            : base(key, text)
        { }

        public int No { get; set; } = 0;

        public string Speaker { get; set; }

        public string SpeakerType { get; set; }

        public string EntryType { get; set; }

        public override string ToString()
        {
            return $"Key({this.Key}) Text({this.English})";
        }
    }
}
