namespace LibTotModMaker.TransSheet
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using CsvHelper;
    using LibTotModMaker.Game.Design;
    using LibTotModMaker.Models;

    public class TotTransSheetDao
    {
        public static void SaveToCsv(
            TotTransInfo totTransInfo,
            TotGameDesignInfo designInfo,
            string path,
            Encoding enc = null)
        {
            if (enc == null)
            {
                enc = Encoding.UTF8;
            }

            using (var writer = new CsvWriter(new StreamWriter(path, false, enc)))
            {
                writer.Configuration.RegisterClassMap<CsvMapper>();
                writer.WriteHeader<TotTransSheetEntry>();
                writer.NextRecord();

                foreach (var totFile in totTransInfo.Items.Values)
                {
                    int no = 1;
                    foreach (var entry in totFile.Items.Values)
                    {
                        if (IsInvalid(entry.Text))
                        {
                            //// 原文のテキストが空白の場合は無視。
                            continue;
                        }

                        var data = new TotTransSheetEntry(entry.Key, entry.Text);

                        data.FileID = totFile.FileID;
                        data.No = no;
                        var phraseEntry = designInfo.GetSpeakerName(entry.Key);
                        if (phraseEntry == null)
                        {
                            data.Speaker = string.Empty;
                            data.SpeakerType = string.Empty;
                        }
                        else
                        {
                            data.Speaker = phraseEntry.CharacterKey;
                            data.SpeakerType = $"{phraseEntry.SpeakerType}";
                        }

                        data.Japanese = string.Empty;
                        var entryType = TotGameDesignEntry.GetEntryType(entry.Key);
                        if (entryType == TotGameDesignEntry.NEntryType.Empty)
                        {
                            data.EntryType = string.Empty;
                        }
                        else
                        {
                            data.EntryType = entryType.ToString();
                        }

                        writer.WriteRecord(data);
                        writer.NextRecord();
                        no++;
                    }
                }
            }
        }

        public static TotTransSheetInfo LoadFromCsv(string path, Encoding enc = null)
        {
            if (enc == null)
            {
                enc = Encoding.UTF8;
            }

            using (var reader = new StreamReader(path, enc))
            {
                using (var csv = new CsvReader(reader))
                {
                    csv.Configuration.Delimiter = ",";
                    csv.Configuration.HasHeaderRecord = true;
                    csv.Configuration.RegisterClassMap<CsvMapper>();
                    var records = csv.GetRecords<TotTransSheetEntry>();

                    var totTransSheetInfo = new TotTransSheetInfo();
                    foreach (var record in records)
                    {
                        var treanlatedText =
                            GetTranslatedText(record.English, record.Japanese, record.MachineTranslation);
                        totTransSheetInfo.AddEntry(record.FileID, record);
                    }

                    return totTransSheetInfo;
                }
            }
        }

        public static TotTransSheetInfo LoadFromCsvSimple(string path, Encoding enc = null)
        {
            if (enc == null)
            {
                enc = Encoding.UTF8;
            }

            using (var reader = new StreamReader(path, enc))
            {
                using (var csv = new CsvReader(reader))
                {
                    csv.Configuration.Delimiter = ",";
                    csv.Configuration.HasHeaderRecord = true;
                    csv.Configuration.RegisterClassMap<CsvMapperSimple>();
                    var records = csv.GetRecords<TotTransSheetEntrySimple>();

                    var totTransSheetInfo = new TotTransSheetInfo();
                    foreach (var record in records)
                    {
                        var treanlatedText =
                            GetTranslatedText(record.English, record.Japanese, record.MachineTranslation);
                        totTransSheetInfo.AddEntry(record.FileID, record);
                    }

                    return totTransSheetInfo;
                }
            }
        }

        private static bool IsInvalid(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return true;
            }

            if (!text.Contains(' ') && text.Contains('_'))
            {
                return true;
            }

            return false;
        }

        private static string GetTranslatedText(string english, string japanese, string googleTrans)
        {
            if (string.IsNullOrWhiteSpace(japanese) && string.IsNullOrWhiteSpace(googleTrans))
            {
                return english;
            }
            else if (!string.IsNullOrWhiteSpace(japanese) && string.IsNullOrWhiteSpace(googleTrans))
            {
                return japanese;
            }
            else if (string.IsNullOrWhiteSpace(japanese) && !string.IsNullOrWhiteSpace(googleTrans))
            {
                if (googleTrans.Contains("{"))
                {
                    //// 変数を含むテキストは除外する。
                    return english;
                }
                else
                {
                    return googleTrans;
                }
            }
            else if (!string.IsNullOrWhiteSpace(japanese) && !string.IsNullOrWhiteSpace(googleTrans))
            {
                return japanese;
            }
            else
            {
                throw new Exception("unknown error.");
            }
        }

        /// <summary>
        /// 格納ルール ：マッピングルール(一行目を列名とした場合は列名で定義することができる。)
        /// </summary>
        public class CsvMapper : CsvHelper.Configuration.ClassMap<TotTransSheetEntry>
        {
            public CsvMapper()
            {
                // 出力時の列の順番は指定した順となる。
                this.Map(x => x.FileID).Name("[[*FileID*]]");
                this.Map(x => x.Key).Name("[[*Key*]]");
                this.Map(x => x.EntryType).Name("[[EntryType]]");
                this.Map(x => x.No).Name("[[No]]");
                this.Map(x => x.Speaker).Name("[[Speaker]]");
                this.Map(x => x.SpeakerType).Name("[[SpeakerType]]");
                this.Map(x => x.English).Name("[[*English*]]");
                this.Map(x => x.Japanese).Name("[[*Japanese*]]");
                this.Map(x => x.MachineTranslation).Name("[[*MT*]]");
            }
        }

        /// <summary>
        /// 格納ルール ：マッピングルール(一行目を列名とした場合は列名で定義することができる。)
        /// </summary>
        public class CsvMapperSimple : CsvHelper.Configuration.ClassMap<TotTransSheetEntrySimple>
        {
            public CsvMapperSimple()
            {
                // 出力時の列の順番は指定した順となる。
                this.Map(x => x.FileID).Name("[[*FileID*]]");
                this.Map(x => x.Key).Name("[[*Key*]]");
                this.Map(x => x.English).Name("[[*English*]]");
                this.Map(x => x.Japanese).Name("[[*Japanese*]]");
                this.Map(x => x.MachineTranslation).Name("[[*MT*]]");
            }
        }
    }
}
