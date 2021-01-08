@echo off

set DOTNET_EXE=dotnet
set FRAMEWORK_PARAM=--framework netcoreapp3.1
set CONFIGURATION_PARAM=--configuration Release

set CONSOLE_CSPROJ=.\src\Acdc.Preprocessor.Console\Acdc.Preprocessor.Console.csproj
set CONSOLE_OUTPUT_PARAM=--output "..\..\publish\Acdc.Preprocessor.Console\\"

set CONSOLE_DOTNET_PARAMS=publish %CONSOLE_CSPROJ% %CONSOLE_OUTPUT_PARAM% %FRAMEWORK_PARAM% %CONFIGURATION_PARAM%

echo Publishing the AcdcPreprocessor Console project... .. .
%DOTNET_EXE% %CONSOLE_DOTNET_PARAMS%