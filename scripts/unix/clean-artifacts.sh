#!/usr/bin/env bash
# =====================================================================
#  Apta - clean build artifacts (bin / obj / .vs) + restore
#
#  Handy when the dotnet cache gets in the way: stale build output,
#  a restore that won't pick up changes, or leftover .vs state after
#  switching branches. Wipes the artifacts and does a clean restore.
#
#  The repo root is resolved from this script's location
#  (scripts/unix -> ../..), so it works from anywhere.
#
#  Run with:  ./clean-artifacts.sh
#  For double-click, make sure it is executable:  chmod +x clean-artifacts.sh
# =====================================================================
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"

echo "====================================================================="
echo " APTA - CLEAN BUILD ARTIFACTS"
echo " ROOT: $ROOT"
echo "====================================================================="
echo

echo "[+] (1/2) REMOVING BIN, OBJ AND .VS FOLDERS (SKIPPING NODE_MODULES AND .GIT)..."
removed=0
while IFS= read -r -d '' dir; do
    echo "    [-] $dir"
    rm -rf "$dir"
    removed=$((removed + 1))
done < <(
    find "$ROOT" \
        -type d \( -name node_modules -o -name .git \) -prune -o \
        -type d \( -name bin -o -name obj -o -name .vs \) -print0
)
echo "[+] $removed FOLDER(S) REMOVED."
echo

echo "[+] (2/2) RESTORING SOLUTIONS (DOTNET RESTORE --NO-CACHE)..."
if ! command -v dotnet >/dev/null 2>&1; then
    echo "[!] 'DOTNET' NOT FOUND ON PATH - SKIPPING RESTORE."
else
    found=0
    while IFS= read -r -d '' sln; do
        echo "    [-] $sln"
        dotnet restore "$sln" --no-cache
        found=$((found + 1))
    done < <(
        find "$ROOT" \
            -type d -name node_modules -prune -o \
            -type f -name '*.sln' -print0
    )
    if [ "$found" -eq 0 ]; then
        echo "[!] NO .SLN SOLUTION FOUND."
    fi
fi

echo
echo "[+] DONE."

# Pause only when running interactively (double-click in a terminal).
if [ -t 0 ]; then
    read -n 1 -r -s -p "PRESS ANY KEY TO EXIT..."
    echo
fi
