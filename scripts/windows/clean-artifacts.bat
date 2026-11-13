@echo off
setlocal enabledelayedexpansion

rem =====================================================================
rem  Apta - clean build artifacts (bin / obj / .vs) + restore
rem
rem  Handy when the dotnet cache gets in the way: stale build output,
rem  a restore that won't pick up changes, or leftover .vs state after
rem  switching branches. Wipes the artifacts and does a clean restore.
rem
rem  Double-click to run. The repo root is resolved from this script's
rem  location (scripts\windows -> ..\..), so it works from anywhere.
rem =====================================================================

pushd "%~dp0..\.." >nul
set "ROOT=%CD%"

echo =====================================================================
echo  APTA - CLEAN BUILD ARTIFACTS
echo  ROOT: %ROOT%
echo =====================================================================
echo.

echo [+] (1/2) REMOVING BIN, OBJ AND .VS FOLDERS (SKIPPING NODE_MODULES AND .GIT)...
set "REMOVED=0"
for /d /r "%ROOT%" %%d in (bin obj .vs) do (
    if exist "%%d" (
        set "SKIP="
        echo "%%d" | find /i "\node_modules\" >nul && set "SKIP=1"
        echo "%%d" | find /i "\.git\" >nul && set "SKIP=1"
        if not defined SKIP (
            echo     [-] %%d
            rd /s /q "%%d"
            set /a REMOVED+=1
        )
    )
)
echo [+] !REMOVED! FOLDER(S) REMOVED.
echo.

echo [+] (2/2) RESTORING SOLUTIONS (DOTNET RESTORE --NO-CACHE)...
where dotnet >nul 2>&1
if errorlevel 1 (
    echo [!] 'DOTNET' NOT FOUND ON PATH - SKIPPING RESTORE.
    goto :done
)

set "FOUND=0"
for /r "%ROOT%" %%s in (*.sln) do (
    set "SKIP="
    echo "%%s" | find /i "\node_modules\" >nul && set "SKIP=1"
    if not defined SKIP (
        echo     [-] %%s
        dotnet restore "%%s" --no-cache
        set /a FOUND+=1
    )
)
if "!FOUND!"=="0" echo [!] NO .SLN SOLUTION FOUND.

:done
echo.
echo [+] DONE.
popd >nul
pause
endlocal
