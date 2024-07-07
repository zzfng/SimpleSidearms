@echo off
SETLOCAL ENABLEDELAYEDEXPANSION
set project_dir=%1
set project_name=%2
set sepcial_version=%~3
set LOCALVARIABLES_bat=%project_dir%LOCALVARIABLES%sepcial_version%.bat
if not exist "%LOCALVARIABLES_bat%" (
	echo set LOCALVARIABLES_rimworld_dir=D:\SteamLibrary\steamapps\common\Rimworld\ >> %LOCALVARIABLES_bat%
	echo set LOCALVARIABLES_harmony_dll=%%LOCALVARIABLES_rimworld_dir%%\Mods\708455313\1.5\Assemblies\0Harmony.dll  >> %LOCALVARIABLES_bat%
	echo %LOCALVARIABLES_bat% created.
	notepad "%LOCALVARIABLES_bat%"
)
call %LOCALVARIABLES_bat%
set rimworld_dir=%LOCALVARIABLES_rimworld_dir%
set harmony_dll=%LOCALVARIABLES_harmony_dll%
if not exist %rimworld_dir% ( 
	echo %rimworld_dir% not exist	
	echo Needs to set the correct PATH in %LOCALVARIABLES_bat%
	exit /b 1
)
if not exist %harmony_dll% (
	echo %harmony_dll% not exist	
	echo Needs to set the correct PATH in %LOCALVARIABLES_bat%
	exit /b 1
)
set rwdll_dir=%rimworld_dir%RimWorldWin64_Data\Managed\
if not exist %rwdll_dir% ( 
	echo %rwdll_dir% not exist	
	exit /b 1
)
set dll_dir=%project_dir%bin\

xcopy /y /d /e /f "%harmony_dll%" %dll_dir% 
for %%d in (Assembly-CSharp.dll UnityEngine.dll UnityEngine.CoreModule.dll UnityEngine.IMGUIModule.dll UnityEngine.TextCoreModule.dll UnityEngine.TextRenderingModule.dll) do (
	xcopy /y /d /e /f "%rwdll_dir%%%d" %dll_dir% 
)