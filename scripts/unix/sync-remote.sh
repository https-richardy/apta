#!/usr/bin/env bash
# =====================================================================
#  Apta - sync local repo with the remote
#
#  Gets your local repo back in line with the remote:
#    1. fetch everything and prune stale remote-tracking refs
#    2. switch to the default branch (main/master) and fast-forward it
#    3. delete local branches whose remote branch is gone
#
#  Safe by design: it aborts on a dirty working tree, only fast-forwards
#  (never force-resets), and only deletes branches that are fully merged
#  (unmerged ones are skipped with a warning), so it never throws away
#  local work.
#
#  The repo root is resolved from this script's location
#  (scripts/unix -> ../..), so it works from anywhere.
#
#  Run with:  ./sync-with-remote.sh
#  For double-click, make sure it is executable:  chmod +x sync-with-remote.sh
# =====================================================================
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
cd "$ROOT"

pause_exit() {
    if [ -t 0 ]; then
        read -n 1 -r -s -p "PRESS ANY KEY TO EXIT..."
        echo
    fi
}

echo "====================================================================="
echo " APTA - SYNC LOCAL REPO WITH REMOTE"
echo " ROOT: $ROOT"
echo "====================================================================="
echo

if ! command -v git >/dev/null 2>&1; then
    echo "[!] 'GIT' NOT FOUND ON PATH - ABORTING."
    pause_exit
    exit 1
fi

if ! git rev-parse --is-inside-work-tree >/dev/null 2>&1; then
    echo "[!] NOT INSIDE A GIT REPOSITORY - ABORTING."
    pause_exit
    exit 1
fi

if ! git remote | grep -qx "origin"; then
    echo "[!] NO 'ORIGIN' REMOTE CONFIGURED - ABORTING."
    pause_exit
    exit 1
fi

if [ -n "$(git status --porcelain)" ]; then
    echo "[!] WORKING TREE HAS UNCOMMITTED CHANGES - ABORTING."
    echo "    COMMIT OR STASH YOUR CHANGES FIRST, THEN RUN AGAIN."
    pause_exit
    exit 1
fi

echo "[+] (1/3) FETCHING FROM REMOTE (PRUNE)..."
git fetch --all --prune
echo

echo "[+] (2/3) UPDATING DEFAULT BRANCH TO MATCH REMOTE..."
default_branch=""
if git symbolic-ref --quiet refs/remotes/origin/HEAD >/dev/null 2>&1; then
    default_branch="$(git symbolic-ref --short refs/remotes/origin/HEAD)"
else
    git remote set-head origin -a >/dev/null 2>&1 || true
    if git symbolic-ref --quiet refs/remotes/origin/HEAD >/dev/null 2>&1; then
        default_branch="$(git symbolic-ref --short refs/remotes/origin/HEAD)"
    fi
fi
default_branch="${default_branch#origin/}"
if [ -z "$default_branch" ]; then
    if git show-ref --verify --quiet refs/remotes/origin/main; then
        default_branch="main"
    elif git show-ref --verify --quiet refs/remotes/origin/master; then
        default_branch="master"
    else
        default_branch="main"
    fi
fi
echo "    [-] DEFAULT BRANCH: $default_branch"

git checkout "$default_branch"
if git pull --ff-only; then
    echo "    [-] $default_branch IS UP TO DATE WITH REMOTE."
else
    echo "    [!] COULD NOT FAST-FORWARD $default_branch (LOCAL HAS DIVERGED)."
    echo "        RESOLVE IT MANUALLY - NOT FORCING A RESET."
fi
echo

echo "[+] (3/3) PRUNING LOCAL BRANCHES GONE FROM REMOTE..."
pruned=0
while IFS= read -r br; do
    [ -z "$br" ] && continue
    [ "$br" = "$default_branch" ] && continue
    if git branch -d "$br" >/dev/null 2>&1; then
        echo "    [-] $br ... DELETED (MERGED)"
        pruned=$((pruned + 1))
    else
        echo "    [!] $br ... SKIPPED (UNMERGED COMMITS)"
    fi
done < <(git for-each-ref --format='%(refname:short) %(upstream:track)' refs/heads | awk '$2=="[gone]"{print $1}')
echo "    $pruned BRANCH(ES) PRUNED."
echo

echo "[+] DONE. LOCAL REPO IS IN LINE WITH THE REMOTE."
pause_exit
