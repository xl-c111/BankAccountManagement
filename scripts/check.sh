#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT_DIR"

dotnet restore BankAccountManagement.slnx
dotnet build BankAccountManagement.slnx
dotnet test BankAccountManagement.Tests

echo "All checks passed."
