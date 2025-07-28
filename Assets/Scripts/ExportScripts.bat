@echo off
setlocal enabledelayedexpansion

set "OutputFile=ExportedScripts.txt"

if exist "%OutputFile%" del "%OutputFile%"

:: Collect all .cs file paths into a temporary list
set "FileList=files.txt"
if exist "%FileList%" del "%FileList%"

for /r %%f in (*.cs) do (
    echo %%f>> "%FileList%"
)

:: Call PowerShell ONCE to do all the heavy lifting
powershell -NoProfile -Command ^
  "$out = '%OutputFile%';" ^
  "Get-Content '%FileList%' | ForEach-Object {" ^
  "  Add-Content -Path $out -Value ('--- File: ' + $_ + ' ---') -Encoding UTF8;" ^
  "  Get-Content $_ | Add-Content -Path $out -Encoding UTF8;" ^
  "  Add-Content -Path $out -Value '' -Encoding UTF8;" ^
  "  Add-Content -Path $out -Value '' -Encoding UTF8;" ^
  "}"


:: Clean up
del "%FileList%"
