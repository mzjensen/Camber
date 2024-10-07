@echo off
setlocal

echo Starting to generate migrations XML...

cd /d "%~dp0"

python GenerateMigrationsXML.py

echo Finished generating migrations XML file

endlocal
