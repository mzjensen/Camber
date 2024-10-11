@echo off
setlocal

echo Starting to create package...

set "C3DVersion=2025"
set "DynamoVersion=3.2"
set "PackageName=Camber"
set "SourceDir=%CD%\..\..\build\Debug"
set "DestDir=%APPDATA%\Autodesk\C3D %C3DVersion%\Dynamo\%DynamoVersion%\packages\%PackageName%"

echo Source directory: %SourceDir%
echo Package directory: %DestDir%

if exist "%DestDir%" (
    echo Deleting existing package directory...
    rmdir /s /q "%DestDir%"
)

mkdir "%DestDir%\bin"
mkdir "%DestDir%\extra"

echo Copying built source files...
xcopy "%SourceDir%\*" "%DestDir%\bin" /s /i /y

echo Copying pkg.json...
copy "%~dp0pkg.json" "%DestDir%" /y

echo Copying extras...
copy "%~dp0Camber.Migrations.xml" "%DestDir%\extra" /y

echo Finished creating '%PackageName%' package

endlocal