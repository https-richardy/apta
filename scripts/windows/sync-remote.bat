@echo off
setlocal enabledelayedexpansion

rem =====================================================================
rem  Apta - sync local repo with the remote
rem
rem  Gets your local repo back in line with the remote:
rem    1. fetch everything and prune stale remote-tracking refs
rem    2. switch to the default branch (main/master) and fast-forward it
rem    3. force-delete local branches whose remote branch is gone
rem       (merged and deleted on the remote, e.g. after a squash/merge PR)
rem
rem  Safe by design: it aborts on a dirty working tree and only fast-forwards
rem  the default branch (never force-resets). Step 3 only ever touches
rem  branches marked "[gone]" (their remote branch was deleted on GitHub),
rem  mirroring the remote; branches never pushed have no upstream, so they
rem  are never "[gone]" and are left untouched.
rem
rem  Double-click to run. The repo root is resolved from this script's
rem  location (scripts\windows -> ..\..), so it works from anywhere.
rem =====================================================================

pushd "%~dp0..\.." >nul
set "ROOT=%CD%"

echo =====================================================================
echo  APTA - SYNC LOCAL REPO WITH REMOTE
echo  ROOT: %ROOT%
echo =====================================================================
echo.

where git >nul 2>&1
if errorlevel 1 (
    echo [!] 'GIT' NOT FOUND ON PATH - ABORTING.
    goto :done
)

git rev-parse --is-inside-work-tree >nul 2>&1
if errorlevel 1 (
    echo [!] NOT INSIDE A GIT REPOSITORY - ABORTING.
    goto :done
)

set "HASORIGIN="
for /f "delims=" %%r in ('git remote') do if /i "%%r"=="origin" set "HASORIGIN=1"
if not defined HASORIGIN (
    echo [!] NO 'ORIGIN' REMOTE CONFIGURED - ABORTING.
    goto :done
)

set "DIRTY="
for /f "delims=" %%x in ('git status --porcelain') do set "DIRTY=1"
if defined DIRTY (
    echo [!] WORKING TREE HAS UNCOMMITTED CHANGES - ABORTING.
    echo     COMMIT OR STASH YOUR CHANGES FIRST, THEN RUN AGAIN.
    goto :done
)

echo [+] (1/3) FETCHING FROM REMOTE (PRUNE)...
git fetch --all --prune
echo.

echo [+] (2/3) UPDATING DEFAULT BRANCH TO MATCH REMOTE...
set "DEFAULT="
for /f "delims=" %%r in ('git symbolic-ref --short refs/remotes/origin/HEAD 2^>nul') do set "DEFAULT=%%r"
if not defined DEFAULT (
    git remote set-head origin -a >nul 2>&1
    for /f "delims=" %%r in ('git symbolic-ref --short refs/remotes/origin/HEAD 2^>nul') do set "DEFAULT=%%r"
)
if defined DEFAULT set "DEFAULT=!DEFAULT:origin/=!"
if not defined DEFAULT (
    git show-ref --verify --quiet refs/remotes/origin/main && set "DEFAULT=main"
)
if not defined DEFAULT (
    git show-ref --verify --quiet refs/remotes/origin/master && set "DEFAULT=master"
)
if not defined DEFAULT set "DEFAULT=main"
echo     [-] DEFAULT BRANCH: !DEFAULT!

git checkout "!DEFAULT!"
git pull --ff-only
if errorlevel 1 (
    echo     [!] COULD NOT FAST-FORWARD !DEFAULT! (LOCAL HAS DIVERGED).
    echo         RESOLVE IT MANUALLY - NOT FORCING A RESET.
) else (
    echo     [-] !DEFAULT! IS UP TO DATE WITH REMOTE.
)
echo.

echo [+] (3/3) PRUNING LOCAL BRANCHES GONE FROM REMOTE...
set "PRUNED=0"
for /f "tokens=1" %%b in ('git for-each-ref --format^="%%(refname:short) %%(upstream:track)" refs/heads ^| findstr /c:"[gone]"') do (
    if /i not "%%b"=="!DEFAULT!" (
        git branch -D "%%b" >nul 2>&1
        if errorlevel 1 (
            echo     [!] %%b ... COULD NOT DELETE
        ) else (
            echo     [-] %%b ... DELETED ^(GONE FROM REMOTE^)
            set /a PRUNED+=1
        )
    )
)
echo     !PRUNED! BRANCH^(ES^) PRUNED.
echo.

echo [+] DONE. LOCAL REPO IS IN LINE WITH THE REMOTE.

:done
echo.
popd >nul
pause
endlocal
