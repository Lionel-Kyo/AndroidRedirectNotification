@echo off
dotnet publish AndroidRedirectNotification\AndroidRedirectNotification.csproj -f net8.0-android -c Release --no-restore
@pause