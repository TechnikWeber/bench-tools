A native build for Windows on ARM and checksums. The tools themselves are unchanged since v1.0.1.

- **New: `ngl202-battery-cycling-arm64.exe`** — a native program for Windows on ARM (Snapdragon laptops). It needs the Arm64 .NET 10 Desktop Runtime.
- **New: `SHA256SUMS`** lists the checksum of every file.

All six tools as single `.exe` files, built by GitHub Actions.

- `ngl202-battery-cycling.exe` needs the [.NET 10 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/10.0).
- The other tools run on the .NET Framework 4.7.2 built into Windows 10 and 11, on Windows on ARM too.
- The files are not signed; SmartScreen may warn (*More info › Run anyway*).
