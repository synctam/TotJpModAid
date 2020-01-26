namespace LibTotModMaker.Game.Design
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public class TotGameDesignInfo
    {
        /// <summary>
        /// デザインエントリーの辞書。
        /// キーはデザインエントリーのID。
        /// </summary>
        public Dictionary<string, TotGameDesignEntry> Items { get; } =
            new Dictionary<string, TotGameDesignEntry>(StringComparer.OrdinalIgnoreCase);

        public void AddEntry(TotGameDesignEntry.NEntryType entryType, string id)
        {
            if (this.Items.ContainsKey(id))
            {
                throw new Exception($"Duplicate ID({id}), EntryType({entryType}).");
            }
            else
            {
                TotGameDesignEntry entry = new TotGameDesignEntry(entryType, id);
                this.Items.Add(entry.ID, entry);
            }
        }

        public TotGameDesignEntry GetEntry(string id)
        {
            foreach (var designEntry in this.Items.Values)
            {
                if (designEntry.ID.Equals(id, StringComparison.OrdinalIgnoreCase))
                {
                    return designEntry;
                }
            }

            throw new Exception($"Design entry not found. ID({id})");
        }

        /// <summary>
        /// デバッグ情報を返す。
        /// </summary>
        /// <param name="tab">タブ</param>
        /// <returns>デバッグ情報</returns>
        public string ToString(string tab)
        {
            var buff = new StringBuilder();

            foreach (var entry in this.Items.Values)
            {
                buff.Append(entry.ToString($"{tab}\t"));
            }

            return buff.ToString();
        }

        /// <summary>
        /// IDから話者キーを返す。
        /// </summary>
        /// <param name="id">キー</param>
        /// <returns>話者ID</returns>
        public TotGameDesignPhraseEntry GetSpeakerName(string id)
        {
            //// ID:  d__BjB1EluNkWWH2f5Ls2qVA_p_0
            ////      ~~~~~~~~~~~~~~~~~~~~~~~~   ~~~~~~~~~
            ////              KEY                 phraseID
            //// Key: d__BjB1EluNkWWH2f5Ls2qVA
            //// phraseID: 0
            const string pattern = @"_p_[0-9]+";
            if (Regex.IsMatch(id, pattern, RegexOptions.ECMAScript))
            {
                //// 話者情報を取得する。
                //// デザインIDを取得する。
                var r = new Regex(pattern);
                var designID = r.Replace(id, string.Empty);
                var m = r.Match(id);
                if (m.Success && (m.Captures.Count == 1))
                {
                    //// フレーズIDを取得する。
                    var phraseIdStr = m.Value.Replace("_p_", string.Empty);
                    int phraseID = 0;
                    if (int.TryParse(phraseIdStr, out phraseID))
                    {
                        //// デザインエントリーを取得する。
                        var entry = this.GetEntry(designID);
                        if (entry.Phrases.ContainsKey(phraseID))
                        {
                            //// フレースを取得する。
                            var phrase = entry.Phrases[phraseID];
                            //// フレーズから話者キーを返す。
                            return phrase;
                        }
                        else
                        {
                            //// 話者IDのデータが存在しない。
                            throw new Exception($"Phrase Key not found. Key({phraseID})");
                        }
                    }
                    else
                    {
                        //// フレーズIDが数値でない。
                        throw new Exception($"Number error: {phraseIdStr}");
                    }
                }
                else
                {
                    throw new Exception($"Matching error: Key({id})");
                }
            }
            else
            {
                //// phraseID を含まない場合は話者情報ではない。
                return null;
            }
        }
    }
}
