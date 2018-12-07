:: Set current directory and paths
@echo off
C:
CD %~dp0

:: Update wiki from github
CD "C:\Users\robin\Documents\Visual Studio 2015\Projects\NXtel.wiki"
git pull
CD %~dp0
DEL  /F /Q /S "..\server\NXtelManager\App_Data\GitHubWiki\*.md"
XCOPY /Y "C:\Users\robin\Documents\Visual Studio 2015\Projects\NXtel.wiki\*.md" "..\server\NXtelManager\App_Data\GitHubWiki\*.*"

:: Prepare NXtelServer for publishing
PATH=%PATH%;C:\Program Files (x86)\MSBuild\14.0\bin
CD ..\server\NXtelServer
DEL  /F /Q /S Publish\*.*
RMDIR /S /Q "Publish\app.publish"

:: Publish NXtelServer
msbuild NXtelServer.csproj /p:Configuration="Release" /p:Platform="AnyCPU"

:: Deploy NXtelServer
RMDIR /S /Q "Publish\app.publish"
DEL  /F /Q /S Publish\*.xml
DEL  /F /Q /S Publish\*.pdb
DEL  /F /Q /S Publish\*.config
DEL  /F /Q /S Publish\*.application
XCOPY /Y "Publish\*.*" "%USERPROFILE%\Dropbox\Spectrum\Next\NxTelSync\NXtelServer\"

:: Prepare NXtelManager for publishing
CD ..\NXtelManager
DEL  /F /Q /S Publish\*.*
RMDIR /S /Q "Publish\Views"
RMDIR /S /Q "Publish\SiteHelp"
RMDIR /S /Q "Publish\Scripts"
RMDIR /S /Q "Publish\fonts"
RMDIR /S /Q "Publish\Content"
RMDIR /S /Q "Publish\bin"

:: Publish NXtelManager
msbuild NXtelManager.csproj /p:DeployOnBuild=true /p:PublishProfile=FolderDeploy /p:Configuration="Release" /p:Platform="x86"

:: Deploy NXtelManager
DEL  /F /Q Publish\web.config
RMDIR /S /Q "Publish\bin\roslyn"
XCOPY /Y /E "Publish\*.*" "%USERPROFILE%\Dropbox\Spectrum\Next\NxTelSync\NXtelManager\"
CD %~dp0
DEL  /F /Q /S "%USERPROFILE%\Dropbox\Spectrum\Next\NxTelSync\NXtelManager\App_Data\GitHubWiki\*.md"
XCOPY /Y "..\server\NXtelManager\App_Data\GitHubWiki\*.md" "%USERPROFILE%\Dropbox\Spectrum\Next\NxTelSync\NXtelManager\App_Data\GitHubWiki\*.*"

PAUSE