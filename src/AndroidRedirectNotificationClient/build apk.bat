@echo off
dotnet publish AndroidRedirectNotification\AndroidRedirectNotification.csproj -f net9.0-android -c Release --no-restore
@pause