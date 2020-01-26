namespace TotSheetMaker
{
    using LibTotModMaker.Game.Design;
    using LibTotModMaker.LanguageDetector;
    using LibTotModMaker.Models;
    using LibTotModMaker.TransSheet;
    using MonoOptions;
    using S5mDebugTools;

    /// <summary>
    /// 翻訳シート作成
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

            MakeSheet(opt.Arges);

            TDebugUtils.Pause();
            return 0;
        }

        /// <summary>
        /// 翻訳シートを作成する。
        /// </summary>
        /// <param name="opt">コマンドラインオプション</param>
        private static void MakeSheet(TOptions.TArgs opt)
        {
            var fileListPath = opt.FileNameFileList;
            var dataFolderPath = opt.FolderNameDataFolder;
            var langCode = opt.LanguageCode;
            var sheetPath = opt.FileNameSheet;
            var designDataPath = opt.FileNameGlobal;

            //// 付加情報の読み込み
            var designInfo = TotGameDesignDao.LoadFromFile(designDataPath);

            var totLanguageDetector = new TotLanguageDetector();
            //// ファイル一覧は、修正済みの確定版を使用する。
            totLanguageDetector.LoadFromFile(fileListPath);
            TotTransInfo totTransInfo = new TotTransInfo();
            var lang = langCode;
            //// データフォルダーから言語情報を読み込む。
            TotTransDao.LoadFromFolder(
                dataFolderPath,
                totLanguageDetector.Items,
                totTransInfo,
                lang);
            //// 翻訳シートを保存する。
            TotTransSheetDao.SaveToCsv(totTransInfo, designInfo, sheetPath);
        }
    }
}
