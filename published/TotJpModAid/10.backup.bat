@rem
@rem Tower of Time ���{�ꉻ�x���c�[��
@rem �I���W�i���̃A�Z�b�g���o�b�N�A�b�v����B
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
