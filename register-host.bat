@echo off
setlocal

:: Get the directory of the current script
set "scriptDir=%~dp0"
set "jsonPath=%scriptDir%host.json"

:: Create the registry entries
reg add "HKEY_CURRENT_USER\Software\Mozilla\NativeMessagingHosts\sytd_host" /v "" /t REG_SZ /d "%jsonPath%" /f
echo Registered to Firefox

endlocal
echo Registry keys added successfully.

pause