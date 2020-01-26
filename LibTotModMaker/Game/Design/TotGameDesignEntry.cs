namespace LibTotModMaker.Game.Design
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TotGameDesignEntry
    {
        public TotGameDesignEntry()
        {
        }

        public TotGameDesignEntry(NEntryType entryType, string id)
        {
            this.EntryType = entryType;
            this.ID = id;
        }

        public enum NEntryType
        {
            /// <summary>
            /// ジャーナル
            /// </summary>
            Journals,

            /// <summary>
            /// 台詞
            /// </summary>
            Dialogs,

            /// <summary>
            /// クエスト
            /// </summary>
            Quests,

            /// <summary>
            /// マップ
            /// </summary>
            Maps,

            /// <summary>
            /// イベント情報
            /// </summary>
            InfoEvents,

            /// <summary>
            /// メッセージ
            /// </summary>
            Messages,

            /// <summary>
            /// チュートリアル
            /// </summary>
            TutrialTips,

            /// <summary>
            /// 未設定
            /// </summary>
            Empty,
        }

        /// <summary>
        /// エントリータイプ
        /// </summary>
        public NEntryType EntryType { get; } = NEntryType.Empty;

        /// <summary>
        /// エントリーのキー
        /// </summary>
        public string ID { get; } = string.Empty;

        /// <summary>
        /// フレーズエントリーの辞書。
        /// キーは、フレーズエントリーのID。
        /// </summary>
        public Dictionary<long, TotGameDesignPhraseEntry> Phrases { get; } =
            new Dictionary<long, TotGameDesignPhraseEntry>();

        /// <summary>
        /// クエスト付加情報
        /// </summary>
        public List<string> TaskTemplates { get; } = new List<string>();

        /// <summary>
        /// Keyに該当するEntryTypeを返す。
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>EntryType</returns>
        public static NEntryType GetEntryType(string key)
        {
            if (key.StartsWith("j_"))
            {
                return TotGameDesignEntry.NEntryType.Journals;
            }
            else if (key.StartsWith("d_"))
            {
                return TotGameDesignEntry.NEntryType.Dialogs;
            }
            else if (key.StartsWith("q_"))
            {
                return TotGameDesignEntry.NEntryType.Quests;
            }
            else if (key.StartsWith("map_"))
            {
                return TotGameDesignEntry.NEntryType.Maps;
            }
            else if (key.StartsWith("i_"))
            {
                return TotGameDesignEntry.NEntryType.InfoEvents;
            }
            else if (key.StartsWith("m_"))
            {
                return TotGameDesignEntry.NEntryType.Messages;
            }
            else if (key.StartsWith("t_"))
            {
                return TotGameDesignEntry.NEntryType.TutrialTips;
            }
            else
            {
                return TotGameDesignEntry.NEntryType.Empty;
            }
        }

        public void AddPhraseEntry(long id, long? speakerType, string characterKey)
        {
            var phraseEntry = new TotGameDesignPhraseEntry(id, speakerType, characterKey);
            this.Phrases.Add(phraseEntry.ID, phraseEntry);
        }

        public void AddSubQuest(string subQuestID)
        {
            this.TaskTemplates.Add(subQuestID);
        }

        /// <summary>
        /// phraseEntryを追加する。
        /// </summary>
        /// <param name="phraseEntry">phraseEntry</param>
        public void AddPhraseEntry(TotGameDesignPhraseEntry phraseEntry)
        {
            if (this.Phrases.ContainsKey(phraseEntry.ID))
            {
                throw new Exception($"Duplicate ID({phraseEntry.ID})");
            }
            else
            {
                this.Phrases.Add(phraseEntry.ID, phraseEntry);
            }
        }

        public string ToString(string tab)
        {
            var buff = new StringBuilder();

            buff.AppendLine($"{tab}EntryType({this.EntryType}), ID({this.ID})");
            if (this.EntryType == NEntryType.Dialogs)
            {
                foreach (var phraseEntry in this.Phrases.Values)
                {
                    buff.Append(phraseEntry.ToString($"{tab}\t"));
                }
            }

            if (this.EntryType == NEntryType.Quests)
            {
                int no = 1;
                foreach (var subQuestID in this.TaskTemplates)
                {
                    buff.AppendLine($"{tab}\t[{no}] {subQuestID}");
                    no++;
                }
            }

            return buff.ToString();
        }
    }
}
