namespace LibTotModMaker.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TotTransFile
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="path">ファイルのパス</param>
        /// <param name="isFullPath">パスの形式</param>
        public TotTransFile(string path, bool isFullPath)
        {
            //// ファイルのパスからファイル名を抽出し記録する。
            if (isFullPath)
            {
                this.FileID = Path.GetFileNameWithoutExtension(path);
            }
            else
            {
                this.FileID = path;
            }
        }

        /// <summary>
        /// エントリーの辞書。キーはEntry-Key。
        /// </summary>
        public Dictionary<string, TotTransEntry> Items { get; } = new Dictionary<string, TotTransEntry>();

        /// <summary>
        /// ファイル名(拡張子なし)。大文字小文字は変換せず、そのまま格納。
        /// </summary>
        public string FileID { get; } = string.Empty;

        /// <summary>
        /// エントリーの追加
        /// </summary>
        /// <param name="key">キー</param>
        /// <param name="text">テキスト</param>
        public void AddEntry(string key, string text)
        {
            if (this.Items.ContainsKey(key))
            {
                throw new Exception($"Duplicate key({key}).");
            }
            else
            {
                var entry = new TotTransEntry(key, text);
                this.Items.Add(key, entry);
            }
        }

        public override string ToString()
        {
            StringBuilder buff = new StringBuilder();
            foreach (var itemPair in this.Items)
            {
                buff.AppendLine(itemPair.Value.ToString());
            }

            return buff.ToString();
        }

        public TotTransEntry GetEntry(string key)
        {
            if (this.Items.ContainsKey(key))
            {
                return this.Items[key];
            }
            else
            {
                //// key が存在しない場合は、空のオブジェクトを返す。
                return new TotTransEntry(key, string.Empty);
            }
        }
    }
}
