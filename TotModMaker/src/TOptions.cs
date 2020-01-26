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
            msg.WriteLine($@"日本語化MODを作成する。");
            msg.WriteLine(
                $@"  usage: {exeName} -f <file list> -d <original data folder> -o <translated data folder>" +
                $@" -s <Trans Sheet> [-l <lang code>] [-m] [-r]");
            msg.WriteLine($@"OPTIONS:");
            this.optionSet.WriteOptionDescriptions(msg);
            msg.WriteLine($@"Example:");
            msg.WriteLine($@"  (-s)翻訳シートと(-d)原文のデータフォルダーから(-o)翻訳済みデータを格納するフォルダーにMODを作成する。");
            msg.WriteLine($@"    {exeName} -f fileList.txt -d data\en -s TransSheet.csv -o data\jp");

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
                { "f|file_list=" , this.args.FileNameFileListText     , v => this.args.FileNameFileList = v},
                { "d|data="      , this.args.FolderNameDataFolderText , v => this.args.FolderDataPath   = v},
                { "o|mod="       , this.args.FolderNameModFolderText  , v => this.args.FolderModPath    = v},
                { "l|lang="      , this.args.LanguageCodeText         , v => this.args.LanguageCode     = v},
                { "s|sheet="     , this.args.FileNameSheetText        , v => this.args.FileNameSheet    = v},
                { "m"            , this.args.UseMachineTransText      , v => this.args.UseMachineTrans  = v != null},
                { "r"            , this.args.UseReplaceText           , v => this.args.UseReplace       = v != null},
                { "h|help"       , "ヘルプ"                           , v => this.args.Help             = v != null},
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

            if (this.IsErrorModFolderPath())
            {
                return;
            }

            if (this.IsErrorLanguageCode())
            {
                return;
            }

            if (this.IsErrorSheetFile())
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
        /// (-d) データフォルダーの確認
        /// </summary>
        /// <returns>エラーの有無</returns>
        private bool IsErrorDataFolderPath()
        {
            if (string.IsNullOrWhiteSpace(this.Arges.FolderDataPath))
            {
                this.SetErrorMessage(
                    $"{Environment.NewLine}エラー：" +
                    $"(-d)原文のデータフォルダーのパスを指定してください。");
                this.isError = true;

                return true;
            }
            else
            {
                if (!Directory.Exists(this.Arges.FolderDataPath))
                {
                    this.SetErrorMessage(
                        $"{Environment.NewLine}エラー：" +
                        $"(-d)原文のデータフォルダーがありません。" +
                        $"{Environment.NewLine}({Path.GetFullPath(this.Arges.FolderDataPath)})");
                    this.isError = true;

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// (-o) 翻訳されたデータファイルを格納するフォルダーの確認
        /// </summary>
        /// <returns>翻訳シートの存在有無</returns>
        private bool IsErrorModFolderPath()
        {
            if (string.IsNullOrWhiteSpace(this.Arges.FolderModPath))
            {
                this.SetErrorMessage(
                    $"{Environment.NewLine}エラー：" +
                    $"(-o)翻訳されたされたデータファイルを格納するフォルダーのパスを指定してください。");
                this.isError = true;

                return true;
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
        /// (-s)翻訳シートの有無を確認
        /// </summary>
        /// <returns>翻訳シートの存在有無</returns>
        private bool IsErrorSheetFile()
        {
            if (string.IsNullOrWhiteSpace(this.Arges.FileNameSheet))
            {
                this.SetErrorMessage(
                    $@"{Environment.NewLine}エラー：(-s)翻訳シートのパスを指定してください。");
                this.isError = true;

                return true;
            }
            else
            {
                if (!File.Exists(this.Arges.FileNameSheet))
                {
                    this.SetErrorMessage(
                        $"{Environment.NewLine}エラー：(-s)翻訳シートが見つかりません。" +
                        $"{Environment.NewLine}({Path.GetFullPath(this.Arges.FileNameSheet)})");
                    this.isError = true;

                    return true;
                }
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

            public string FolderDataPath { get; internal set; }

            public string FolderNameDataFolderText { get; internal set; } =
                $"原文が格納されているデータフォルダーのパス。";

            public string FolderModPath { get; internal set; }

            public string FolderNameModFolderText { get; internal set; } =
                "翻訳されたデータフォルダーのパスを指定する。";

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

            public bool UseMachineTrans { get; internal set; }

            public string UseMachineTransText { get; internal set; } =
                $"有志翻訳がない場合は機械翻訳を使用する。";

            public bool UseReplace { get; internal set; }

            public string UseReplaceText { get; internal set; } =
                $"出力用データファイルが既に存在する場合はを上書きする。";

            public bool Help { get; set; }
        }
    }
}
