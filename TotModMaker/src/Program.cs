namespace TotModMaker
{
    using LibTotModMaker.LanguageDetector;
    using LibTotModMaker.Models;
    using LibTotModMaker.TransSheet;
    using MonoOptions;
    using S5mDebugTools;

    /// <summary>
    /// 日本語化MOD作成
    /// </summary>
    internal class Program
    {
        private static int Main(string[] args)
        {
            //// コマンドラインオプションの処理
            TOptions opt = new TOptions(args);
            if (opt.IsError)
            {
                TDebugUtils.Pause();
                return 1;
            }

            if (opt.Arges.Help)
            {
                opt.ShowUsage();

                TDebugUtils.Pause();
                return 1;
            }

            MakeMod(opt.Arges);

            TDebugUtils.Pause();
            return 0;
        }

        /// <summary>
        /// MODを作成する。
        /// </summary>
        /// <param name="opt">コマンドラインオプション</param>
        private static void MakeMod(TOptions.TArgs opt)
        {
            var fileListPath = opt.FileNameFileList;
            var dataFolderPath = opt.FolderDataPath;
            var modFolderPath = opt.FolderModPath;

            var langCode = opt.LanguageCode;
            var sheetPath = opt.FileNameSheet;

            var useMT = opt.UseMachineTrans;
            var replace = opt.UseReplace;

            //// 翻訳シートの読み込み
            var transSheet = TotTransSheetDao.LoadFromCsvSimple(sheetPath);

            //// ファイル一覧の富込み
            var totLanguageDetector = new TotLanguageDetector();
            totLanguageDetector.LoadFromFile(fileListPath);

            //// 原文を読み言語情報を作成する
            TotTransInfo totTransInfo = new TotTransInfo();
            TotTransDao.LoadFromFolder(
                dataFolderPath,
                totLanguageDetector.Items,
                totTransInfo,
                langCode);

            //// 日本語化MODの作成。
            TotTransDao.SaveToText(modFolderPath, totTransInfo, transSheet, useMT, replace);
        }
    }
}
