@rem
@rem Tower of Time 日本語化支援ツール
@rem オリジナルのアセットをバックアップする。
@rem

@choice /m "backup"
@if %errorlevel% equ 1 goto execute
@if %errorlevel% equ 2 goto quit
@goto quit


:execute
copy /y ..\TowerOfTime_Data\resources.assets org\assets

:quit

@pause
@exit /b
