# Tower of Time

Tower of Time 日本語化支援ツール

使い方については、こちらのブログ記事を御覧ください。  
「[synctam: Tower of Time 日本語化支援ツールの使い方](https://synctam.blogspot.com/2020/01/tower-of-time.html)」



## TotSheetMaker
```使い方：
データフォルダーから翻訳シートを作成する。
  usage: TotSheetMaker.exe -f <file list> -d <data folder> -s <trans sheet> -g <global file> [-l <lang code>] [-r]
OPTIONS:
  -f, --file_list=VALUE      ファイル一覧のパス。
  -d, --data=VALUE           原文が格納されているデータフォルダーのパス。
  -l, --lang=VALUE           言語コード（省略時は'en'）。
                               指定可能な言語コード
                               en:english
  -s, --sheet=VALUE          CSV形式の翻訳シートのパス。
  -g, --global=VALUE         話者情報ファイルのパス。
  -r                         翻訳シートが既に存在する場合はを上書きする。
  -h, --help                 ヘルプ
Example:
  (-d)データフォルダーから(-f)ファイル一覧と(-g)話者情報を使用し(-s)翻訳シートを作成する。
    TotSheetMaker.exe -d data\en -f fileList.txt -g data\en\gameplayTemplatesData.txt -s TransSheet.csv
終了コード:
 0  正常終了
 1  異常終了
```


## TotJpModMaker
```使い方：
日本語化MODを作成する。
  usage: TotModMaker.exe -f <file list> -d <original data folder> -o <translated data folder> -s <Trans Sheet> [-l <lang code>] [-m] [-r]
OPTIONS:
  -f, --file_list=VALUE      ファイル一覧のパス。
  -d, --data=VALUE           原文が格納されているデータフォルダーのパス。
  -o, --mod=VALUE            翻訳されたデータフォルダーのパスを指定する。
  -l, --lang=VALUE           言語コード（省略時は'en'）。
                               指定可能な言語コード
                               en:english
  -s, --sheet=VALUE          CSV形式の翻訳シートのパス。
  -m                         有志翻訳がない場合は機械翻訳を使用する。
  -r                         出力用データファイルが既に存在する場合はを上書きする。
  -h, --help                 ヘルプ
Example:
  (-s)翻訳シートと(-d)原文のデータフォルダーから(-o)翻訳済みデータを格納するフォルダーにMODを作成する。
    TotModMaker.exe -f fileList.txt -d data\en -s TransSheet.csv -o data\jp
終了コード:
 0  正常終了
 1  異常終了
```
