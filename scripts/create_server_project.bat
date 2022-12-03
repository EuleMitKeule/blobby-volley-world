@echo off

mkdir "%~dp0SERVER"
mklink /D "%~dp0SERVER\Assets" "%~dp0Assets"
mklink /D "%~dp0SERVER\Packages" "%~dp0Packages"
mklink /D "%~dp0SERVER\ProjectSettings" "%~dp0ProjectSettings"
mklink /D "%~dp0SERVER\UserSettings" "%~dp0UserSettings"

pause