@rem
@rem Tower of Time 日本語化支援ツール
@rem 翻訳シートの作成
@rem

@SET PATH=tools;%PATH%

@set yyyy=%date:~0,4%
@set mm=%date:~5,2%
@set dd=%date:~8,2%
 
@set time2=%time: =0%
 
@set hh=%time2:~0,2%
@set mn=%time2:~3,2%
@set ss=%time2:~6,2%
 
@set DATE_TIME=%yyyy%.%mm%.%dd%_%hh%.%mn%.%ss%

TotSheetMaker.exe ^
	-f "csv\FileList(1.4.3.11839).csv" ^
	-d "org\Unity_Assets_Files\resources" ^
	-s "csv\TotTransSheet_%DATE_TIME%.csv" ^
	-g "org\Unity_Assets_Files\resources\gameplayTemplatesData.txt" ^
	-r

@pause
@exit /b
