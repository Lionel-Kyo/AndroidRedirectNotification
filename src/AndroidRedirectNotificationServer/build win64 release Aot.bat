@echo off
dotnet publish --runtime win-x64 --configuration Release /p:PublishAot=true /p:EnableTrimAnalyzer=false /p:SUPPRESS_NETSDK1175=true
@pause