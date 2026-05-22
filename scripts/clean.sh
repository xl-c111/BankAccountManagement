#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "$0")/.." && pwd)"

find "$ROOT_DIR" -type d \( -name bin -o -name obj -o -name TestResults -o -name coverage-report \) -prune -exec rm -rf {} +

echo "Cleaned build/test artifacts."
