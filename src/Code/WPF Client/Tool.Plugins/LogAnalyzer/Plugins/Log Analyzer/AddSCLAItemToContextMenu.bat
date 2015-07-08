@ECHO off

IF NOT EXIST "%CD%\SitecoreLogAnalyzer.exe" ECHO File "%CD%\SitecoreLogAnalyzer.exe" should exist. Cannot complete operation.
IF NOT EXIST "%CD%\SitecoreLogAnalyzer.exe" PAUSE
IF NOT EXIST "%CD%\SitecoreLogAnalyzer.exe" exit

REG ADD "hkcu\Software\Classes\txtfile\shell\Open with scla" /ve /t REG_SZ /d "Open with Sitecore Log Analyzer" /f
REG ADD "hkcu\Software\Classes\txtfile\shell\Open with scla" /v Icon /t REG_SZ /d "%CD%\SitecoreLogAnalyzer.exe" /f
REG ADD "hkcu\Software\Classes\txtfile\shell\Open with scla" /v MultiSelectModel /t REG_SZ /d Single /f
REG ADD "hkcu\Software\Classes\txtfile\shell\Open with scla\command" /ve /t REG_SZ /d "%CD%\SitecoreLogAnalyzer.exe %%1" /f

REG ADD "hkcu\Software\Classes\folder\shell\Open with scla" /ve /t REG_SZ /d "Open with Sitecore Log Analyzer" /f
REG ADD "hkcu\Software\Classes\folder\shell\Open with scla" /v Icon /t REG_SZ /d "%CD%\SitecoreLogAnalyzer.exe" /f
REG ADD "hkcu\Software\Classes\folder\shell\Open with scla" /v MultiSelectModel /t REG_SZ /d Single /f
REG ADD "hkcu\Software\Classes\folder\shell\Open with scla\command" /ve /t REG_SZ /d "%CD%\SitecoreLogAnalyzer.exe %%1" /f

ECHO.
ECHO Context menu handlers were added successfully.
PAUSE