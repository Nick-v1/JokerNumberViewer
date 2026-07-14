@echo off
title Joker Viewer
echo Starting Joker Viewer...
echo.

start "" "publish\Backend.exe"

timeout /t 2 /nobreak >nul

start "" "http://localhost:5289"

echo.
echo The app is running. You can close this window when you're done using it,
echo but closing it will also stop the app.
pause