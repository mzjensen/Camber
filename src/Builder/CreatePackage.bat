@echo off
setlocal

echo Starting to create package...

set "BuildConfig=Debug"
set "C3DVersion=2025"
set "DynamoVersion=3.2"
set "PackageName=Camber"
set "CamberSourceDir=%CD%\..\Camber\bin\%BuildConfig%"
set "CamberUISourceDir=%CD%\..\CamberUI\bin\%BuildConfig%"
set "AssistantSourceDir=%CD%\..\MigrationAssistant\bin\%BuildConfig%"
set "DestDir=%APPDATA%\Autodesk\C3D %C3DVersion%\Dynamo\%DynamoVersion%\packages\%PackageName%"

echo Camber source directory: %CamberSourceDir%
echo Camber UI source directory: %CamberUISourceDir%
echo Migration assistant source directory: %AssistantSourceDir%
echo Package directory: %DestDir%

if exist "%DestDir%" (
    echo Deleting existing package directory...
    rmdir /s /q "%DestDir%"
)

mkdir "%DestDir%\bin"
mkdir "%DestDir%\extra\MigrationAssistant"

echo Copying built source files...
xcopy "%CamberSourceDir%\*" "%DestDir%\bin" /s /i /y
xcopy "%CamberUISourceDir%\*" "%DestDir%\bin" /s /i /y

echo Copying pkg.json...
copy "%~dp0pkg.json" "%DestDir%" /y

echo Copying extras...
xcopy "%AssistantSourceDir%\*" "%DestDir%\extra\MigrationAssistant" /s /i /y

echo Finished creating '%PackageName%' package

endlocal