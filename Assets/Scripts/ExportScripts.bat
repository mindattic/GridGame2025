@echo off
setlocal enabledelayedexpansion

:: Setup
set OutputFile=ExportedScripts.txt

:: Delete old file
if exist "%OutputFile%" del "%OutputFile%"

:: Loop through all .cs files recursively
for /r %%f in (*.cs) do (
    rem Skip ExportedScripts.txt to avoid recursion
    if /I not "%%~nxf"=="%OutputFile%" (
        echo --- File: %%f --- >> "%OutputFile%"
        type "%%f" >> "%OutputFile%"
        echo. >> "%OutputFile%"
        echo. >> "%OutputFile%"
    )
)