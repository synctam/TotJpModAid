// ******************************************************************************
// Copyright (c) 2015-2019 synctam
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
// of the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace MonoOptions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Mono.Options;

    /// <summary>
    /// コマンドライン オプション
    /// </summary>
    public class TOptions
    {
        //// ******************************************************************************
        //// Property fields
        //// ******************************************************************************
        private TArgs args;
        private bool isError = false;
        private StringWriter errorMessage = new StringWriter();
        private OptionSet optionSet;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="arges">コマンドライン引数</param>
        public TOptions(string[] arges)
        {
            this.args = new TArgs();
            this.Settings(arges);
            if (this.IsError)
            {
                this.ShowErrorMessage();
                this.ShowUsage();
            }
            else
            {
                this.CheckOption();
                if (this.IsError)
                {
                    this.ShowErrorMessage();
                    this.ShowUsage();
                }
                else
                {
                    // skip
                }
            }
        }

        //// ******************************************************************************
        //// Property
        //// ******************************************************************************

        /// <summary>
        /// コマンドライン オプション
        /// </summary>
        public TArgs Arges { get { return this.args; } }

        /// <summary>
        /// コマンドライン オプションのエラー有無
        /// </summary>
        public bool IsError { get { return this.isError; } }

        /// <summary>
        /// エラーメッセージ
        /// </summary>
        public string ErrorMessage { get { return this.errorMessage.ToString(); } }

        /// <summary>
        /// Uasgeを表示する
        /// </summary>
        public void ShowUsage()
        {
            TextWriter writer = Console.Error;
            this.ShowUsage(writer);
        }

        /// <summary>
        /// Uasgeを表示する
        /// </summary>
        /// <param name="textWriter">出力先</param>
        public void ShowUsage(TextWriter textWriter)
        {
            var msg = new StringWriter();

            string exeName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);
            msg.WriteLine(string.Empty);
            msg.WriteLine($@"使い方：");
            msg.WriteLine($@"データフォルダーから翻訳シートを作成する。");
            msg.WriteLine($@"  usage: {exeName} -f <file list> -d <data folder> -s <trans sheet> -g <global file> [-l <lang code>] [-r]");
            msg.WriteLine($@"OPTIONS:");
            this.optionSet.WriteOptionDescriptions(msg);
            msg.WriteLine($@"Example:");
            msg.WriteLine($@"  (-d)データフォルダーから(-f)ファイル一覧と(-g)話者情報を使用し(-s)翻訳シートを作成する。");
            msg.WriteLine($@"    {exeName} -d data\en -f fileList.txt -g data\en\gameplayTemplatesData.txt -s TransSheet.csv");
            msg.WriteLine($@"終了コード:");
            msg.WriteLine($@" 0  正常終了");
            msg.WriteLine($@" 1  異常終了");
            msg.WriteLine();

            if (textWriter == null)
            {
                textWriter = Console.Error;
            }

            textWriter.Write(msg.ToString());
        }

        /// <summary>
        /// エラーメッセージ表示
        /// </summary>
        public void ShowErrorMessage()
        {
            TextWriter writer = Console.Error;
            this.ShowErrorMessage(writer);
        }

        /// <summary>
        /// エラーメッセージ表示
        /// </summary>
        /// <param name="textWriter">出力先</param>
        public void ShowErrorMessage(TextWriter textWriter)
        {
            if (textWriter == null)
            {
                textWriter = Console.Error;
            }

            textWriter.Write(this.ErrorMessage);
        }

        /// <summary>
        /// オプション文字の設定
        /// </summary>
        /// <param name="args">args</param>
        private void Settings(string[] args)
        {
            this.optionSet = new OptionSet()
            {
                { "f|file_list="     , this.args.FileNameFileListText , v => this.args.FileNameFileList = v},
                { "d|data="   , this.args.FolderNameDataFolderText , v => this.args.FolderNameDataFolder = v},
                { "l|lang="   , this.args.LanguageCodeText          , v => this.args.LanguageCode          = v},
                { "s|sheet="  , this.args.FileNameSheetText       , v => this.args.FileNameSheet       = v},
                { "g|global=" , this.args.FileNameGlobalText       , v => this.args.FileNameGlobal       = v},
                { "r"         , this.args.UseReplaceText          , v => this.args.UseReplace      = v != null},
                { "h|help"    , "ヘルプ"                          , v => this.args.Help            = v != null},
            };

            List<string> extra;
            try
            {
                extra = this.optionSet.Parse(args);
                if (extra.Count > 0)
                {
                    // 指定されたオプション以外のオプションが指定されていた場合、
                    // extra に格納される。
                    // 不明なオプションが指定された。
                    this.SetErrorMessage($"{Environment.NewLine}エラー：不明なオプションが指定されました。");
                    extra.ForEach(t => this.SetErrorMessage(t));
                    this.isError = true;
                }
            }
            catch (OptionException e)
            {
                ////パースに失敗した場合OptionExceptionを発生させる
                this.SetErrorMessage(e.Message);
                this.isError = true;
            }
        }

        /// <summary>
        /// オプションのチェック
        /// </summary>
        private void CheckOption()
        {
            //// -h
            if (this.Arges.Help)
            {
                this.SetErrorMessage();
                this.isError = false;
                return;
            }

            if (this.IsErrorFileListPath())
            {
                return;
            }

            if (this.IsErrorDataFolderPath())
            {
                return;
            }

            if (this.IsErrorLanguageCode())
            {
                return;
            }

            if (this.IsErrorTransSheetFile())
            {
                return;
            }

            if (this.IsErrorGlobalFilePath())
            {
                return;
            }

            this.isError = false;
            return;
        }

        /// <summary>
        /// (-f) ファイル一覧の確認
        /// </summary>
        /// <returns>エラーの有無</returns>
        private bool IsErrorFileListPath()
        {
            if (string.IsNullOrWhiteSpace(this.Arges.FileNameFileList))
            {
                this.SetErrorMessage(
                    $@"{Environment.NewLine}" +
                    $"エラー：(-f)ファイル一覧のパスを指定してください。");
                this.isError = true;

                return true;
            }

            if (!File.Exists(this.Arges.FileNameFileList))
            {
                this.SetErrorMessage(
                    $"{Environment.NewLine}" +
                    $@"エラー：(-f)ファイル一覧が見つかりません。" +
                    $"{Environment.NewLine}" +
                    $@"({Path.GetFullPath(this.Arges.FileNameFileList)})");
                this.isError = true;

                return true;
            }

            return false;
        }

        /// <summary>
        /// (-d) 言語データフォルダーの確認
        /// </summary>
        /// <returns>エラーの有無</returns>
        private bool IsErrorDataFolderPath()
        {
            if (string.IsNullOrWhiteSpace(this.Arges.FolderNameDataFolder))
            {
                this.SetErrorMessage(
                    $@"{Environment.NewLine}" +
                    $"エラー：(-d)原文のデータフォルダーのパスを指定してください。");
                this.isError = true;

                return true;
            }
            else
            {
                if (!Directory.Exists(this.Arges.FolderNameDataFolder))
                {
                    this.SetErrorMessage(
                        $@"{Environment.NewLine}" +
                        $"エラー：(-d)原文のデータフォルダーが見つかりません。" +
                        $"{Environment.NewLine}" +
                        $"({Path.GetFullPath(this.Arges.FolderNameDataFolder)})");
                    this.isError = true;

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// (-l) 言語コードの確認
        /// </summary>
        /// <returns>エラーの有無</returns>
        private bool IsErrorLanguageCode()
        {
            if (string.IsNullOrWhiteSpace(this.args.LanguageCode))
            {
                //// 省略時は英語。
                this.args.LanguageCode = "en";
            }

            switch (this.args.LanguageCode)
            {
                case "en":
                    //// ok
                    break;
                default:
                    this.SetErrorMessage(
                        $@"{Environment.NewLine}" +
                        $"エラー：(-l)言語コードが存在しません。" +
                        $"{Environment.NewLine}" +
                        $"言語コード({Path.GetFullPath(this.Arges.LanguageCode)})");
                    this.isError = true;

                    return true;
            }

            return false;
        }

        /// <summary>
        /// (-s) 翻訳シートの確認
        /// </summary>
        /// <returns>エラーの有無</returns>
        private bool IsErrorTransSheetFile()
        {
            if (string.IsNullOrWhiteSpace(this.Arges.FileNameSheet))
            {
                this.SetErrorMessage($@"{Environment.NewLine}エラー：(-s)翻訳シートファイルのパスを指定してください。");
                this.isError = true;

                return true;
            }

            if (File.Exists(this.Arges.FileNameSheet) && !this.args.UseReplace)
            {
                this.SetErrorMessage(
                    $"{Environment.NewLine}" +
                    $@"エラー：(-s)翻訳シートファイルが既に存在します。{Environment.NewLine}" +
                    $@"({Path.GetFullPath(this.Arges.FileNameSheet)}){Environment.NewLine}" +
                    $@"上書きする場合は '-r' オプションを指定してください。");
                this.isError = true;

                return true;
            }

            return false;
        }

        /// <summary>
        /// (-g) ファイル一覧の確認
        /// </summary>
        /// <returns>エラーの有無</returns>
        private bool IsErrorGlobalFilePath()
        {
            if (string.IsNullOrWhiteSpace(this.Arges.FileNameGlobal))
            {
                this.SetErrorMessage(
                    $@"{Environment.NewLine}" +
                    $"エラー：(-g)話者情報ファイルのパスを指定してください。");
                this.isError = true;

                return true;
            }

            if (!File.Exists(this.Arges.FileNameGlobal))
            {
                this.SetErrorMessage(
                    $"{Environment.NewLine}" +
                    $@"エラー：(-g)話者情報ファイルが見つかりません。" +
                    $"{Environment.NewLine}" +
                    $@"({Path.GetFullPath(this.Arges.FileNameGlobal)})");
                this.isError = true;

                return true;
            }

            return false;
        }

        private void SetErrorMessage(string errorMessage = null)
        {
            if (errorMessage != null)
            {
                this.errorMessage.WriteLine(errorMessage);
            }
        }

        /// <summary>
        /// オプション項目
        /// </summary>
        public class TArgs
        {
            public string FileNameFileList { get; internal set; }

            public string FileNameFileListText { get; internal set; } =
                $"ファイル一覧のパス。";

            public string FolderNameDataFolder { get; internal set; }

            public string FolderNameDataFolderText { get; internal set; } =
                $"原文が格納されているデータフォルダーのパス。";

            public string LanguageCode { get; internal set; }

            public string LanguageCodeText { get; internal set; } =
                $"言語コード（省略時は'en'）。" +
                $"{Environment.NewLine}" +
                $"指定可能な言語コード" +
                $"{Environment.NewLine}" +
                $"en:english";

            public string FileNameSheet { get; set; }

            public string FileNameSheetText { get; set; } =
                "CSV形式の翻訳シートのパス。";

            public string FileNameGlobal { get; internal set; }

            public string FileNameGlobalText { get; internal set; } =
                $"話者情報ファイルのパス。";

            public bool UseReplace { get; internal set; }

            public string UseReplaceText { get; internal set; } =
                $"翻訳シートが既に存在する場合はを上書きする。";

            public bool Help { get; set; }
        }
    }
}
