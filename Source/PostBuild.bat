@echo off
SETLOCAL ENABLEDELAYEDEXPANSION

set project_dir=%1
set project_name=%2
set sepcial_version=%~3
set configuration=%4

call %project_dir%LOCALVARIABLES%sepcial_version%.bat
set rimworld_dir=%LOCALVARIABLES_rimworld_dir%
set harmony_dll=%LOCALVARIABLES_harmony_dll%
if not exist %rimworld_dir% ( 
echo %rimworld_dir% not exist	
exit /b 0
)
set mod_name=%project_name%
set mod_dir=%rimworld_dir%Mods\%mod_name%\

set solution_dir=%project_dir:~0,-7%
set source_dll=%project_dir%bin\%mod_name%.dll
set source_pdb=%project_dir%bin\%mod_name%.pdb
if not exist "%source_dll%" ( 
echo %source_dll% not exist	
exit /b 0
)

if "%special_version%"=="" (
	xcopy /y /d /e /f "%source_dll%" "%mod_dir%%sepcial_version%\Assemblies\"
	if %configuration%==Debug (
		xcopy /y /d /e /f "%source_pdb%" "%mod_dir%%sepcial_version%\Assemblies\"
	)
	if %configuration%==Release (
		xcopy /y /d /e /f "%source_dll%" "%solution_dir%%sepcial_version%\Assemblies\"
	)
) else (
	xcopy /y /d /e /f "%source_dll%" "%mod_dir%Assemblies\"
	if %configuration%==Debug (
		xcopy /y /d /e /f "%source_pdb%" "%mod_dir%Assemblies\"
	)
	if %configuration%==Release (
		xcopy /y /d /e /f "%source_dll%" "%solution_dir%Assemblies\"
	)
)
for %%d in (About Defs Languages Patches Textures) do (
	xcopy /i /y /e /d /f %solution_dir%%%d %mod_dir%%%d
)