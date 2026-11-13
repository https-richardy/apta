@echo off
setlocal enabledelayedexpansion

rem =====================================================================
rem  Apta - generate a catalog error code (#NAME-ERROR-XXXXX)
rem
rem  Builds the stable, unique error code used by the error catalog:
rem    1. ask for the RESOURCE (e.g. Administrator) and the ERROR NAME
rem       (e.g. AlreadyExists)
rem    2. XXXXX = first 5 chars, UPPERCASE, of the MD5 of RESOURCE+ERROR NAME
rem    3. print  #<RESOURCE-UPPERCASE>-ERROR-<XXXXX>  ready to copy
rem
rem  The NAME segment is the RESOURCE exactly as typed, uppercased (no
rem  pluralisation). The MD5 input keeps the typed casing, so typing
rem  'Administrator' + 'AlreadyExists' reproduces the catalog's 5F4EC.
rem
rem  The code is FIXED once created - it becomes a public identifier of
rem  the error. Generate it once when adding the error and do not
rem  recompute it later.
rem
rem  Double-click to run.
rem =====================================================================

echo =====================================================================
echo  APTA - GENERATE CATALOG ERROR CODE (#NAME-ERROR-XXXXX)
echo =====================================================================
echo.

set "resource="
set /p "resource=RESOURCE (e.g. Administrator): "
set "errorName="
set /p "errorName=ERROR NAME (e.g. AlreadyExists): "

if not defined resource goto :missing
if not defined errorName goto :missing

set "input=%resource%%errorName%"

rem -- MD5 via PowerShell; take first 5 hex chars, uppercase --
rem    pass the input through an env var to avoid quoting/escaping issues
set "APTA_ERR_INPUT=%input%"
set "suffix="
for /f "delims=" %%h in ('powershell -NoProfile -Command "$m=[System.Security.Cryptography.MD5]::Create(); $b=$m.ComputeHash([Text.Encoding]::UTF8.GetBytes($env:APTA_ERR_INPUT)); ([BitConverter]::ToString($b) -replace '-','').Substring(0,5).ToUpper()"') do set "suffix=%%h"
set "APTA_ERR_INPUT="

if not defined suffix (
    echo [!] FAILED TO COMPUTE MD5 ^(POWERSHELL NOT AVAILABLE?^) - ABORTING.
    goto :done
)

rem -- uppercase the resource for the NAME segment --
set "name=%resource%"
call :toupper name

set "code=#!name!-ERROR-!suffix!"

echo.
echo     [-] MD5 INPUT : %input%
echo     [-] ERROR CODE:
echo.
echo     !code!
echo.
goto :done

:missing
echo [!] RESOURCE AND ERROR NAME ARE BOTH REQUIRED - ABORTING.

:done
echo.
pause
endlocal
goto :eof

:toupper
rem Uppercase the variable whose name is passed as %1
call set "_v=%%%1%%"
for %%A in ("a=A" "b=B" "c=C" "d=D" "e=E" "f=F" "g=G" "h=H" "i=I" "j=J" "k=K" "l=L" "m=M" "n=N" "o=O" "p=P" "q=Q" "r=R" "s=S" "t=T" "u=U" "v=V" "w=W" "x=X" "y=Y" "z=Z") do (
    for /f "tokens=1,2 delims==" %%B in (%%A) do call set "_v=%%_v:%%B=%%C%%"
)
set "%1=%_v%"
goto :eof
