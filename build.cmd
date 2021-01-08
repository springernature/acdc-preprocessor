dotnet %SONARSCANNER%  begin /k:"acdc-preprocessor" /d:sonar.host.url=%SONARQUBE_SERVER% /d:sonar.login=%SONARQUBE_TOKEN%
dotnet build .\src\acdc-preprocessor.sln /nodereuse:false
dotnet %SONARSCANNER% end /d:sonar.login=%SONARQUBE_TOKEN%