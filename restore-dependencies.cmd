@echo off

set SOLUTION_FILE=.\src\acdc-preprocessor.sln

set DOTNET_EXE=dotnet
set DOTNET_RESTORE_PARAM=restore

set NUGET_SOURCE_PARAM=--source "https://api.nuget.org/v3/index.json"
set MYGET_SOURCE_PARAM=--source "https://www.myget.org/F/aspnet-contrib/api/v3/index.json"

echo Restoring dependencies... .. .
%DOTNET_EXE% %DOTNET_RESTORE_PARAM% %SOLUTION_FILE% %NUGET_SOURCE_PARAM% %MYGET_SOURCE_PARAM%
