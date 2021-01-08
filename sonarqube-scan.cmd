dotnet sonarscanner begin /k:acdc-preprocessor /d:sonar.host.url=http://acdcsonar.springernature.com /d:sonar.login=8b2b6bf36f0e969f2f2993fe5ee410f260e9a1bb /d:sonar.cs.opencover.reportsPaths="./src/ImageExtraction.Core.Tests/coverage.opencover.xml"
dotnet build ./src/
dotnet sonarscanner end /d:sonar.login=8b2b6bf36f0e969f2f2993fe5ee410f260e9a1bb	