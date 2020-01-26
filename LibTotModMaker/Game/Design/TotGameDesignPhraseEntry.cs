namespace LibTotModMaker.Game.Design
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TotGameDesignPhraseEntry
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="speakerType">speakerType</param>
        /// <param name="characterKey">characterKey</param>
        public TotGameDesignPhraseEntry(long id, long? speakerType, string characterKey)
        {
            this.ID = id;
            this.SpeakerType = speakerType;
            this.CharacterKey = characterKey;
        }

        /// <summary>
        /// フレーズD
        /// </summary>
        public long ID { get; } = 0;

        /// <summary>
        /// 話者タイプ
        /// </summary>
        public long? SpeakerType { get; } = 0;

        /// <summary>
        /// 話者キー
        /// heroes.txt から話者名を取得できる。
        /// </summary>
        public string CharacterKey { get; } = string.Empty;

        public string ToString(string tab)
        {
            var buff = new StringBuilder();

            buff.AppendLine(
                $"{tab}ID({this.ID}) " +
                $"SpeakerType({this.SpeakerType}) " +
                $"CharacterKey({this.CharacterKey})");

            return buff.ToString();
        }
    }
}
