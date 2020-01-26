namespace LibTotModMaker.LanguageDetector
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Text;
    using CsvHelper;
    using LanguageDetection;
    using LibTotModMaker.Models;

    /// <summary>
    /// 言語自動判別
    /// </summary>
    public class TotLanguageDetector
    {
        private readonly ReadOnlyCollection<string> defaultLanguages =
            Array.AsReadOnly(new string[] { "en", "de", "pl", "ru", "fr", "tr", "zh-tw", "zh-cn" });

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="langs">自動解析する言語の配列</param>
        public TotLanguageDetector()
        {
            //// 言語コード「ISO 639-1コード一覧」
            //// https://ja.wikipedia.org/wiki/ISO_639-1%E3%82%B3%E3%83%BC%E3%83%89%E4%B8%80%E8%A6%A7
            //// 「ISO 639言語コード - CyberLibrarian」
            //// https://www.asahi-net.or.jp/~ax2s-kmtn/ref/iso639.html
            //// "en" 英語
            //// "de" ドイツ語
            //// "pl" ポーランド語
            //// "ru" ロシア語
            //// "fr" フランス語
            //// "tr" トルコ語
            //// "zh-tw" 中国語（繁体字）
            //// "zh-cn" 中国語（簡体字）

            this.LanguageList.AddRange(this.defaultLanguages);
        }

        public TotLanguageDetector(string lang)
        {
            this.LanguageList.Add(lang);
        }

        public TotLanguageDetector(string[] languages)
        {
            this.LanguageList.AddRange(languages);
        }

        /// <summary>
        /// 自動解析する言語の配列
        /// </summary>
        public List<string> LanguageList { get; } = new List<string>();

        /// <summary>
        /// ファイルエントリーの辞書。
        /// キーはファイル名。
        /// </summary>
        public Dictionary<string, FileListEntry> Items { get; } =
            new Dictionary<string, FileListEntry>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 指定したフォルダー内にあるファイルの言語を自動判別する。
        /// </summary>
        /// <param name="folderPath">フォルダー</param>
        public void DetectLanguageFromFolder(string folderPath)
        {
            //// フォルダー内のファイル一覧を作成する。
            IEnumerable<string> files = System.IO.Directory.EnumerateFiles(
                folderPath, "*.txt", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                if (TotTransDao.IsOmitFile(file))
                {
                    //// 翻訳対象外のファイルは無視する。
                    continue;
                }

                //// データ読込処理を使用してファイルのデータを読み込む。
                var transInfo = new TotTransInfo();
                TotTransDao.LoadFormText(file, transInfo);
                //// 読み込んだデータから翻訳対象のテキストをbuffに格納する。
                var buff = new StringBuilder();
                foreach (var transFile in transInfo.Items.Values)
                {
                    foreach (var entry in transFile.Items.Values)
                    {
                        buff.AppendLine(entry.Text);
                    }
                }

                //// 翻訳対象テキストの言語を判別する。
                var fileEntry = this.DetectLanguageFromText(buff.ToString(), file);
                if (!string.IsNullOrWhiteSpace(fileEntry.Language))
                {
                    //// 判別できたファイルエントリーを辞書に登録する。
                    var fileName = Path.GetFileName(file);
                    this.Items.Add(fileName, fileEntry);
                }
            }
        }

        /// <summary>
        /// 指定されたテキストの言語を自動判別し、言語名を返す。
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <param name="path">ファイルのパス</param>
        /// <returns>言語名</returns>
        public FileListEntry DetectLanguageFromText(string text, string path)
        {
            var detector = new LanguageDetector();
            detector.AddLanguages(this.LanguageList.ToArray());
            //// 処理毎に判別結果が同じになるよう、シードを設定する。
            detector.RandomSeed = 1;

            var lang = string.Empty;
            double currentProbability = 0f;
            //// テキストの言語を識別する。
            var it = detector.DetectAll(text);
            foreach (var l in it)
            {
                if (currentProbability < l.Probability)
                {
                    //// 識別確率の高い項目を採用する。
                    lang = l.Language;
                    //// 識別確率を記憶する。
                    currentProbability = l.Probability;
                }
            }

            var fileID = Path.GetFileNameWithoutExtension(path);
            var result = new FileListEntry(fileID, lang, currentProbability);

            return result;
        }

        /// <summary>
        /// 翻訳対象ファイルの一覧を読み込むを
        /// </summary>
        /// <param name="path">翻訳対象ファイル一覧のパス</param>
        public void LoadFromFile(string path)
        {
            var enc = Encoding.UTF8;

            using (var reader = new StreamReader(path, enc))
            {
                using (var csv = new CsvReader(reader))
                {
                    csv.Configuration.Delimiter = ",";
                    csv.Configuration.HasHeaderRecord = true;
                    csv.Configuration.RegisterClassMap<CsvMapper>();
                    var records = csv.GetRecords<FileListEntry>();

                    this.Items.Clear();
                    foreach (var record in records)
                    {
                        this.Items.Add(record.FileID, record);
                    }
                }
            }
        }

        /// <summary>
        /// 自動判別したファイル情報をCSV形式で保存する。
        /// </summary>
        /// <param name="path">CSV形式のファイルのパス</param>
        public void SaveToCsv(string path)
        {
            using (var writer = new CsvWriter(new StreamWriter(path, false, Encoding.UTF8)))
            {
                writer.Configuration.RegisterClassMap<CsvMapper>();
                writer.WriteHeader<FileListEntry>();
                writer.NextRecord();

                var sortedDict = this.Items.OrderBy(a => a.Value.FileID);
                foreach (var entryPair in sortedDict)
                {
                    writer.WriteRecord(entryPair.Value);
                    writer.NextRecord();
                }
            }
        }

        public List<FileListEntry> GetFileListByLanguage(string langCode)
        {
            List<FileListEntry> result = new List<FileListEntry>();
            foreach (var fileEntry in this.Items.Values)
            {
                if (fileEntry.Language.Equals(langCode, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(fileEntry);
                }
            }

            return result;
        }

        public override string ToString()
        {
            var buff = new StringBuilder();

            foreach (var entry in this.Items.Values)
            {
                buff.AppendLine($"File({entry.FileID}), Lang({entry.Language})");
            }

            return buff.ToString();
        }

        /// <summary>
        /// ファイルエントリー
        /// </summary>
        public class FileListEntry
        {
            public FileListEntry() { }

            public FileListEntry(string fileName, string language, double probability)
            {
                this.FileID = fileName;
                this.Language = language;
                this.Probability = probability;
            }

            public string FileID { get; set; } = string.Empty;

            public string Language { get; set; } = string.Empty;

            public double Probability { get; set; } = 0.0f;
        }

        /// <summary>
        /// 格納ルール ：マッピングルール(一行目を列名とした場合は列名で定義することができる。)
        /// </summary>
        public class CsvMapper : CsvHelper.Configuration.ClassMap<FileListEntry>
        {
            public CsvMapper()
            {
                // 出力時の列の順番は指定した順となる。
                this.Map(x => x.FileID).Name("[[FileName]]");
                this.Map(x => x.Language).Name("[[Language]]");
                this.Map(x => x.Probability).Name("[[Probability]]");
            }
        }
    }
}
