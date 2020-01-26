namespace LibTotModMaker.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using LibTotModMaker.LanguageDetector;
    using LibTotModMaker.TransSheet;
    using SymFileUtils;

    public class TotTransDao
    {
        public static void LoadFromFolder(
            string folderPath,
            Dictionary<string, TotLanguageDetector.FileListEntry> fileEntries,
            TotTransInfo totTransInfo,
            string lang)
        {
            foreach (var fileEntry in fileEntries.Values)
            {
                if (!fileEntry.Language.Equals(lang, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var fileName = Path.Combine(folderPath, fileEntry.FileID + ".txt");
                LoadFormText(fileName, totTransInfo);
            }
        }

        public static void LoadFormText(string path, TotTransInfo totTransInfo)
        {
            TotTransFile totTransFile = new TotTransFile(path, true);
            totTransInfo.AddFile(path, totTransFile);

            using (StreamReader sr = new StreamReader(path, Encoding.UTF8))
            {
                while (sr.Peek() > -1)
                {
                    var line = sr.ReadLine();
                    //// ToDo: 字幕ファイルの処理機能を追加すること。
                    if (Path.GetFileNameWithoutExtension(path).Contains(".srt_"))
                    {
                        //// 字幕ファイル用パーサーで処理
                    }
                    else
                    {
                        //// Key=Value用パーサーで処理
                        var result = TotTextParser.Parse(line);
                        if (result.key == null)
                        {
                            //// skip
                        }
                        else
                        {
                            totTransFile.AddEntry(result.key, result.value);
                        }
                    }
                }
            }
        }

        public static void SaveToText(
            string folderPath,
            TotTransInfo totTransInfoEN,
            TotTransSheetInfo transSheetInfo,
            bool useMTrans,
            bool replace)
        {
            foreach (var totFileEN in totTransInfoEN.Items.Values)
            {
                var txtFileName = $"{totFileEN.FileID}.txt";
                txtFileName = Path.Combine(folderPath, txtFileName);
                if (!replace)
                {
                    if (File.Exists(txtFileName))
                    {
                        var msg = $"ファイルが既に存在します。" +
                            $"{Environment.NewLine}" +
                            $"\tFile({txtFileName})" +
                            $"{Environment.NewLine}" +
                            $"\t上書きする場合は '-r' オプションを指定してください。";
                        Console.WriteLine(msg);

                        continue;
                    }
                }

                //// UTF-8(BOMなし)
                SymFileUtils.SafeCreateDirectory(Path.GetDirectoryName(txtFileName));
                var enc = new UTF8Encoding(false);
                using (var sw = new StreamWriter(txtFileName, false, enc))
                {
                    foreach (var entryEN in totFileEN.Items.Values)
                    {
                        //// 翻訳
                        var entryJP = transSheetInfo.GetEntry(totFileEN.FileID, entryEN.Key);
                        string translatedText = entryJP.Translate(entryEN.Text, useMTrans);
                        sw.WriteLine($"{entryEN.Key}={translatedText}");
                    }
                }
            }
        }

        public static bool IsOmitFile(string path)
        {
            string[] blackList = new string[]
            {
                "COPYING",
                "current_branch",
                "current_version",
                "gameplayTemplatesData",
                "last_commit",
                "last_published_version",
                "last_tag",
                "LICENSE",
                "LineBreaking Following Characters",
                "LineBreaking Leading Characters",
                "listing*",
                "locomotionData",
                "master_revisions_count",
                "quests_test_basic_exploration_mode_human*",
                "PlayMakerAssemblies",
                "templatesData",
                "templatesDataForNormal",
                "templatesDataForPermadeath",
                "templatesDataForRPGlite",
                "usedUniqueIdsData",
            };

            var fileName = Path.GetFileNameWithoutExtension(path).ToLower();
            if (fileName.Contains(".srt"))
            {
                return true;
            }

            foreach (var f in blackList)
            {
                if (f.Contains("*"))
                {
                    var fileNamePattern = f.TrimEnd('*').ToLower();
                    if (fileName.ToLower().Contains(fileNamePattern))
                    {
                        return true;
                    }
                }
                else
                {
                    if (fileName.Equals(f, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
