"""Apply the repository's C# formatting rules using dotnet format."""

from __future__ import annotations

import argparse
import shutil
import subprocess
import sys
from pathlib import Path


ROOT = Path(__file__).resolve().parent
SOLUTION = ROOT / "test.sln"


def main() -> int:
    parser = argparse.ArgumentParser(
        description="Format C# code with Allman braces and 8-space indentation."
    )
    parser.add_argument(
        "paths",
        nargs="*",
        help="Optional C# files or directories (defaults to all C# files in the solution).",
    )
    args = parser.parse_args()

    dotnet = shutil.which("dotnet")
    if dotnet is None:
        print("Error: dotnet SDK is required but was not found on PATH.", file=sys.stderr)
        return 1

    command = [dotnet, "format", "whitespace", str(SOLUTION), "--no-restore", "--verbosity", "quiet"]
    if args.paths:
        included: list[str] = []
        for raw_path in args.paths:
            path = Path(raw_path)
            if not path.is_absolute():
                path = ROOT / path
            try:
                included.append(str(path.resolve().relative_to(ROOT)))
            except ValueError:
                print(f"Error: path must be inside the repository: {raw_path}", file=sys.stderr)
                return 2
        command.extend(["--include", *included])

    result = subprocess.run(command, cwd=ROOT, check=False)
    return result.returncode


if __name__ == "__main__":
    raise SystemExit(main())
