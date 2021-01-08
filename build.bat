@echo off

REM Build release
set FrameworkVersion=v4.0.30319
set FrameworkDir=%SystemRoot%\Microsoft.NET\Framework

if exist "%SystemRoot%\Microsoft.NET\Framework64" (
  set FrameworkDir=%SystemRoot%\Microsoft.NET\Framework64
)

"%FrameworkDir%\%FrameworkVersion%\msbuild.exe" /T:Clean;Build;Publish /p:Configuration=Release /p:VisualStudioVersion=12.0 /p:AspnetCompilerPath="C:\windows\Microsoft.NET\Framework64\v4.0.30319" /p:PrecompileBeforePublish=true /p:OutputPath="obj\Release" .\src\acdc-preprocessor.sln