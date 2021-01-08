@echo off
:: ******************************************************************
:: help link http://docs.octopusdeploy.com/display/OD/Using+Octo.exe
:: ******************************************************************

::call build-pacakge-2.cmd PACKAGE_ID SERVICE_PATH ARTIFACT
::-------------------------------------------------------------------

set PACKAGE_ID=%1%
set SERVICE_PATH=%2%
set ARTIFACT=%3%
set VERSION_INFO=%4%

set OCTO_EXE=C:\tools\octopus\octo.exe
set OCTO_EXE_PARAMS=pack --id=%PACKAGE_ID% --version=%VERSION_INFO% --basePath=%SERVICE_PATH% --outFolder=%ARTIFACT% --overwrite --format=Zip

echo -------------------------------------------------------------------------
echo packaging %PACKAGE_ID% ... .. .
echo

echo -------------------------------------------------------------------------
%OCTO_EXE% %OCTO_EXE_PARAMS%
echo -------------------------------------------------------------------------