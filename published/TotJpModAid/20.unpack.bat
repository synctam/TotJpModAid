@rem
@rem Tower of Time 日本語化支援ツール
@rem UnityEX でエクスポートする
@rem

@SET PATH=tools;%PATH%

@choice /m "unpack resources "
@if %errorlevel% equ 1 goto execute
@if %errorlevel% equ 2 goto CANCEL
@goto NORMAL

:execute
@copy /y org\assets\resources.assets ..\TowerOfTime_Data
@if not "%ERRORLEVEL%"  == "0" GOTO ERROR

UnityEX.exe export ..\TowerOfTime_Data\resources.assets -p org -t txt
@if not "%ERRORLEVEL%"  == "0" GOTO ERROR

@goto NORMAL

@rem *******************************************************************************
:ERROR
@echo;
@echo ********************************
@echo アンパックに失敗しました。
@echo ********************************
@echo;
@goto FINAL

@rem *******************************************************************************
:NORMAL
@echo;
@goto FINAL

@rem *******************************************************************************
:CANCEL
@echo 処理をキャンセルしました。
@goto FINAL

@rem *******************************************************************************
:FINAL
@pause
@exit /b
