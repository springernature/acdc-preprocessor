@echo off

set DOTNET_EXE=dotnet
set FRAMEWORK_PARAM=--framework netcoreapp2.1

set CONSOLE_CSPROJ=.\src\Acdc.Preprocessor.Console\Acdc.Preprocessor.Console.csproj
set CONSOLE_OUTPUT_PARAM=--output "..\..\build\Acdc.Preprocessor.Console\\"
set CONSOLE_DOTNET_PARAMS=build %CONSOLE_CSPROJ% %CONSOLE_OUTPUT_PARAM% %FRAMEWORK_PARAM%

echo Building the Acdc Preprocessor Console project... .. .
%DOTNET_EXE% %CONSOLE_DOTNET_PARAMS%
