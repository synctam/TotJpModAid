namespace LibTotModMaker.TransSheet
{
    using System.Collections.Generic;

    public class TotTransSheetInfo
    {
        /// <summary>
        /// TotFileの辞書。
        /// キーはファイル名(拡張子なし)、全て小文字。
        /// </summary>
        public Dictionary<string, TotTransSheetFile> Items { get; } = new Dictionary<string, TotTransSheetFile>();

        public void AddEntry(string fileID, TotTransSheetEntryBase record)
        {
            if (this.Items.ContainsKey(fileID))
            {
                var transSheetFile = this.Items[fileID];
                transSheetFile.AddEntry(record);
            }
            else
            {
                var transSheetFile = new TotTransSheetFile(fileID);
                this.Items.Add(transSheetFile.FileID, transSheetFile);
                transSheetFile.AddEntry(record);
            }
        }

        public TotTransSheetEntryBase GetEntry(string fileID, string key)
        {
            if (this.Items.ContainsKey(fileID))
            {
                var transFile = this.Items[fileID];
                var result = transFile.GetEntry(key);
                return result;
            }
            else
            {
                var result = new TotTransSheetEntry(key, string.Empty);
                return result;
            }
        }
    }
}
