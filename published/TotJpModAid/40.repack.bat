@rem
@rem Tower of Time 日本語化支援ツール
@rem 日本語化MODを UnityEX でインポートする
@rem

@SET PATH=tools;%PATH%

@choice /m "Re-Pack resource "
@if %errorlevel% equ 1 goto execute
@if %errorlevel% equ 2 goto CANCEL
@goto NORMAL

:execute
@copy /y org\assets\resources.assets ..\TowerOfTime_Data
@if not "%ERRORLEVEL%"  == "0" GOTO ERROR

UnityEX.exe import ..\TowerOfTime_Data\resources.assets
@if not "%ERRORLEVEL%"  == "0" GOTO ERROR

@rem UnityEX.exe import ..\TowerOfTime_Data\sharedassets0.assets
@rem @if not "%ERRORLEVEL%"  == "0" GOTO ERROR

@goto NORMAL

@rem *******************************************************************************
:ERROR
@echo;
@echo ********************************
@echo リパックに失敗しました。
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
