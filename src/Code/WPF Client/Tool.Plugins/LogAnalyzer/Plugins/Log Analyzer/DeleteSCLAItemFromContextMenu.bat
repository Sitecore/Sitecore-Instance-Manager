@ECHO off

REG DELETE "hkcu\Software\Classes\txtfile\shell\Open with scla" /f
REG DELETE "hkcu\Software\Classes\folder\shell\Open with scla" /f

ECHO.
ECHO Context menu handlers were deleted successfully.
PAUSE