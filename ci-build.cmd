@echo off

REM ci-build BUILD_NUMBER

set BUILD_NUMBER=%1%

set MAJOR_VERSION=1
set MINOR_VERSION=0
set REVISION=0
set VERSION_INFO="%MAJOR_VERSION%.%MINOR_VERSION%.%BUILD_NUMBER%.%REVISION%"
set BUILD=.\build\
set ARTIFACT=.\artifact\

::---------------------------------------------------
set CONSOLE_PUBLISH_PATH=.\publish\Acdc.Preprocessor.Console\
set CONSOLE_PACKAGE_ID=acdc-preprocessor-console
::---------------------------------------------------


set PACKAGE_NAME=%ARTIFACT%%CONSOLE_PACKAGE_ID%.%VERSION_INFO%.zip

echo ------------------------------------------------------------------------
echo build started
echo ------------------------------------------------------------------------

if exist %BUILD% (
	echo deleting old builds if any
	rd /s /q  %BUILD%
	echo done
)

if exist %ARTIFACT% (
	echo deleting old build artifacts if any
	del /q %ARTIFACT%*.zip
	echo done
)

call .\restore-dependencies.cmd
echo error %ERRORLEVEL%
if %errorlevel% neq 0 goto build_fail

echo ------------------------------------------------------------------------
echo

call .\build.cmd
echo error %ERRORLEVEL%
if %errorlevel% neq 0 goto build_fail

echo ------------------------------------------------------------------------
echo

call .\publish.cmd
echo error %ERRORLEVEL%
if %errorlevel% neq 0 goto build_fail

echo ------------------------------------------------------------------------
echo Unit testing started
call .\acdc-preprocessor-sonar.cmd
echo error %ERRORLEVEL%
if %errorlevel% neq 0 goto build_fail

echo ------------------------------------------------------------------------

echo
call .\sonarqube-scan.cmd
echo error %ERRORLEVEL%
if %errorlevel% neq 0 goto build_fail

echo ------------------------------------------------------------------------
echo

call .\build-package.cmd %CONSOLE_PACKAGE_ID% %CONSOLE_PUBLISH_PATH% %ARTIFACT% %VERSION_INFO%
echo error %ERRORLEVEL%
if %errorlevel% neq 0 goto build_fail

echo ------------------------------------------------------------------------
echo

call .\publish-package.cmd %PACKAGE_NAME%
if %errorlevel% neq 0 goto build_fail

exit /b

:build_fail

echo ------------------------------------------------------------------------
echo ************************** [  BUILD FAILURE  ] *************************
echo ------------------------------------------------------------------------

exit /b %errorlevel%