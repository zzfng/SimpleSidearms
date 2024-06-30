@echo off
SETLOCAL ENABLEDELAYEDEXPANSION
set project_dir=%1
set project_name=%2
set sepcial_version=%~3

call %project_dir%LOCALVARIABLES%sepcial_version%.bat
set rimworld_dir=%LOCALVARIABLES_rimworld_dir%
set harmony_dll=%LOCALVARIABLES_harmony_dll%

set rwdll_dir=%rimworld_dir%RimWorldWin64_Data\Managed\
if not exist %rwdll_dir% ( 
	echo %rwdll_dir% not exist	
	exit /b 0
)
if not exist %harmony_dll% (
	echo %harmony_dll% not exist	
	exit /b 0
)
set dll_dir=%project_dir%bin\
xcopy /y /d /e /f "%harmony_dll%" %dll_dir% 
for %%d in (Assembly-CSharp.dll UnityEngine.dll UnityEngine.CoreModule.dll UnityEngine.IMGUIModule.dll UnityEngine.TextCoreModule.dll UnityEngine.TextRenderingModule.dll) do (
	xcopy /y /d /e /f "%rwdll_dir%%%d" %dll_dir% 
)