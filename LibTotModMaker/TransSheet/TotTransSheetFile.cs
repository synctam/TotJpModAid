namespace LibTotModMaker.TransSheet
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class TotTransSheetFile
    {
        public TotTransSheetFile(string path)
        {
            //// ファイルのパスからファイル名を抽出し記録する。
            this.FileID = path;
        }

        /// <summary>
        /// エントリーの辞書。キーはEntry-Key。
        /// </summary>
        public Dictionary<string, TotTransSheetEntryBase> Items { get; } =
            new Dictionary<string, TotTransSheetEntryBase>();

        /// <summary>
        /// ファイル名(拡張子なし)。大文字小文字は変換せず、そのまま格納。
        /// </summary>
        public string FileID { get; } = string.Empty;

        /// <summary>
        /// エントリーの追加
        /// </summary>
        /// <param name="entry">エントリー</param>
        public void AddEntry(TotTransSheetEntryBase entry)
        {
            if (this.Items.ContainsKey(entry.Key))
            {
                throw new Exception($"Duplicate key({entry.Key}).");
            }
            else
            {
                this.Items.Add(entry.Key, entry);
            }
        }

        public override string ToString()
        {
            var buff = new StringBuilder();
            foreach (var itemPair in this.Items)
            {
                buff.AppendLine(itemPair.Value.ToString());
            }

            return buff.ToString();
        }

        public TotTransSheetEntryBase GetEntry(string key)
        {
            if (this.Items.ContainsKey(key))
            {
                return this.Items[key];
            }
            else
            {
                //// key が存在しない場合は、空のオブジェクトを返す。
                return new TotTransSheetEntry(key, string.Empty);
            }
        }
    }
}
