@rem
@rem Tower of Time ���{�ꉻ�x���c�[��
@rem ���{�ꉻMOD�쐬
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
