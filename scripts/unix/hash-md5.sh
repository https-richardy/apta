#!/usr/bin/env bash
# =====================================================================
#  Apta - generate a catalog error code (#NAME-ERROR-XXXXX)
#
#  Builds the stable, unique error code used by the error catalog:
#    1. ask for the RESOURCE (e.g. Administrator) and the ERROR NAME
#       (e.g. AlreadyExists)
#    2. XXXXX = first 5 chars, UPPERCASE, of the MD5 of RESOURCE+ERROR NAME
#    3. print  #<RESOURCE-UPPERCASE>-ERROR-<XXXXX>  ready to copy
#
#  The NAME segment is the RESOURCE exactly as typed, uppercased (no
#  pluralisation). The MD5 input keeps the typed casing, so typing
#  'Administrator' + 'AlreadyExists' reproduces the catalog's 5F4EC.
#
#  The code is FIXED once created - it becomes a public identifier of
#  the error. Generate it once when adding the error and do not
#  recompute it later.
#
#  Run with:  ./generate-error-code.sh
#  For double-click, make sure it is executable:
#     chmod +x generate-error-code.sh
# =====================================================================
set -euo pipefail

pause_exit() {
    if [ -t 0 ]; then
        read -n 1 -r -s -p "PRESS ANY KEY TO EXIT..."
        echo
    fi
}

md5_hex() {
    if command -v md5sum >/dev/null 2>&1; then
        printf '%s' "$1" | md5sum | cut -d' ' -f1
    elif command -v md5 >/dev/null 2>&1; then
        printf '%s' "$1" | md5 -q
    elif command -v openssl >/dev/null 2>&1; then
        printf '%s' "$1" | openssl md5 -r | cut -d' ' -f1
    else
        return 1
    fi
}

echo "====================================================================="
echo " APTA - GENERATE CATALOG ERROR CODE (#NAME-ERROR-XXXXX)"
echo "====================================================================="
echo

if ! md5_hex "probe" >/dev/null 2>&1; then
    echo "[!] NO MD5 TOOL FOUND (md5sum / md5 / openssl) - ABORTING."
    pause_exit
    exit 1
fi

read -r -p "RESOURCE (e.g. Administrator): " resource
read -r -p "ERROR NAME (e.g. AlreadyExists): " error_name

if [ -z "${resource// /}" ] || [ -z "${error_name// /}" ]; then
    echo "[!] RESOURCE AND ERROR NAME ARE BOTH REQUIRED - ABORTING."
    pause_exit
    exit 1
fi

name="$(printf '%s' "$resource" | tr '[:lower:]' '[:upper:]')"
suffix="$(md5_hex "${resource}${error_name}" | cut -c1-5 | tr '[:lower:]' '[:upper:]')"
code="#${name}-ERROR-${suffix}"

echo
echo "    [-] MD5 INPUT : ${resource}${error_name}"
echo "    [-] ERROR CODE:"
echo
echo "    ${code}"
echo
pause_exit
