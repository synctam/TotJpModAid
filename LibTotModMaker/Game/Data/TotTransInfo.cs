namespace LibTotModMaker.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TotTransInfo
    {
        /// <summary>
        /// TotFileの辞書。
        /// キーはファイル名(拡張子なし)、全て小文字。
        /// </summary>
        public Dictionary<string, TotTransFile> Items { get; } = new Dictionary<string, TotTransFile>();

        /// <summary>
        /// ファイルを追加する。
        /// </summary>
        /// <param name="path">ファイルのパス</param>
        /// <param name="totFile">totFile</param>
        public void AddFile(string path, TotTransFile totFile)
        {
            var fileName = path;
            var key = fileName.ToLower();
            if (this.Items.ContainsKey(key))
            {
                throw new Exception($"Duplicate file name({fileName}).");
            }
            else
            {
                this.Items.Add(key, totFile);
            }
        }

        public void AddEntry(string path, string key, string value, bool isFullPath)
        {
            var fileName = path;

            if (this.Items.ContainsKey(fileName))
            {
                var totFile = this.Items[fileName];
                totFile.AddEntry(key, value);
            }
            else
            {
                var totFile = new TotTransFile(path, isFullPath);
                this.Items.Add(fileName, totFile);
                totFile.AddEntry(key, value);
            }
        }

        public override string ToString()
        {
            StringBuilder buff = new StringBuilder();
            foreach (var itemPair in this.Items)
            {
                buff.AppendLine($"----------------------------");
                buff.AppendLine($"File({itemPair.Key})");
                buff.AppendLine($"----------------------------");
                buff.Append($"{itemPair.Value.ToString()}");
            }

            return buff.ToString();
        }

        public TotTransEntry GetEntry(string fileID, TotTransEntry entry)
        {
            if (this.Items.ContainsKey(fileID))
            {
                var transFile = this.Items[fileID];
                var result = transFile.GetEntry(entry.Key);
                return result;
            }
            else
            {
                var result = new TotTransEntry(entry.Key, string.Empty);
                return result;
            }
        }
    }
}
