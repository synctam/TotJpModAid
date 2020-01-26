@rem
@rem Tower of Time 日本語化支援ツール
@rem 日本語化MOD作成
@rem

@SET PATH=tools;%PATH%

TotModMaker.exe ^
	-f "csv\FileList(1.4.3.11839).csv" ^
	-d "org\Unity_Assets_Files\resources" ^
	-o "..\TowerOfTime_Data\Unity_Assets_Files\resources" ^
	-s "csv\TotTransSheet - TotTransSheet.csv" ^
	-r

@pause
@exit /b
