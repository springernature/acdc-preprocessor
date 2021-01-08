@echo off
:: ********************************************************
:: help link http://docs.octopusdeploy.com/display/OD/Using+Octo.exe
:: ********************************************************

::call publish-pacakge PACKAGE_NAME
::--------------------------------------------------------------------

set PACKAGE_NAME=%1%

set OCTO_EXE=C:\tools\octopus\octo.exe

::todo: secure octopus api key.
set OCTO_EXE_PARAMS=push --package=%PACKAGE_NAME% --replace-existing --server=%OCTOPACK_SERVER% --apiKey=%OCTOPUS_API_KEY%

echo publishing %PACKAGE_NAME%

%OCTO_EXE% %OCTO_EXE_PARAMS%