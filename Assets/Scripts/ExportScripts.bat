@echo off
setlocal enabledelayedexpansion

:: Delete the existing ExportedScripts.txt file if it exists
if exist ExportedScripts.txt del ExportedScripts.txt

:: Append all .cs files recursively to ExportedScripts.txt
for /r %%f in (*.cs) do type "%%f" >> ExportedScripts.txt
