<h1 align="center">bench-tools</h1>

<p align="center">
  <strong>Small Windows tools for lab power supplies and electronic loads: remote control, voltage ramps and battery cycling with SOH.</strong><br>
  Aim-TTi CPX400DP · Aim-TTi LD400P · Rohde &amp; Schwarz NGL202
</p>

<p align="center">
  <img alt="Status: stable" src="https://img.shields.io/badge/status-stable-brightgreen">
  <a href="https://github.com/TechnikWeber/bench-tools/actions/workflows/release.yml"><img alt="Build" src="https://github.com/TechnikWeber/bench-tools/actions/workflows/release.yml/badge.svg"></a>
  <img alt="Platform: Windows" src="https://img.shields.io/badge/platform-Windows-blue">
  <img alt="UI language: German" src="https://img.shields.io/badge/UI-Deutsch-informational">
  <a href="LICENSE"><img alt="License: MIT" src="https://img.shields.io/badge/license-MIT-green"></a>
</p>

<p align="center">
  ⬇️ <a href="https://github.com/TechnikWeber/bench-tools/releases/latest"><strong>Download the .exe files</strong></a>
</p>

---

Each tool is a single `.exe`, no installer. They talk to the instruments over a
serial port (RS-232/USB-UART) or, for the NGL202, over the network (SCPI on
port 5025). The user interface is in German.

> [!CAUTION]
> These tools switch real power. Set current limits and OVP/OCP on the
> instrument first, and never leave a battery cycling unattended without
> protection. Use at your own risk.

## The tools

| Tool | Instruments | What it does |
|---|---|---|
| [`cpx400-control`](cpx400-control) | CPX400DP | Remote control of both outputs: voltage, current, OVP/OCP, output on/off, step up/down with optional verify. |
| [`cpx400-control-advanced`](cpx400-control-advanced) | CPX400DP | Set and read back both outputs, tracking mode (output 2 follows output 1 by a ratio) and timed **voltage ramps**. |
| [`cpx400dp-ld400p-switch`](cpx400dp-ld400p-switch) | CPX400DP + LD400P | Switch the supply and the electronic load from one window, with a command log. |
| [`cpx400dp-ld400p-cycling`](cpx400dp-ld400p-cycling) | CPX400DP + LD400P | **Battery cycling**: charge with the supply, pause, discharge with the load, pause — in a loop. Counts mAh per pass, calculates the state of health (SOH) against the nominal capacity, exports CSV. |
| [`ld400p-mah-correction`](ld400p-mah-correction) | LD400P | The LD400P's clock often runs slow, so its mAh reading is off. Enter the measured mAh and the drift in seconds per hour, get the corrected value. |
| [`ngl202-battery-cycling`](ngl202-battery-cycling) | R&S NGL202 | Battery cycling over the network: charge/discharge voltage and current, rest time, number of cycles; capacity, retention and SOH per cycle, CSV export. |

## Download and run

1. Get the `.exe` you need from the [latest release](https://github.com/TechnikWeber/bench-tools/releases/latest).
2. Windows SmartScreen may warn because the files are not signed: *More info › Run anyway*.
3. `ngl202-battery-cycling.exe` needs the
   [.NET 10 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/10.0).
   All other tools run on the .NET Framework 4.7.2 that ships with Windows 10 and 11.

## Build from source

Open the `.slnx` file of a tool in Visual Studio 2022 (17.13 or newer) and build.
The release builds are made by GitHub Actions on a Windows machine
([`.github/workflows/release.yml`](.github/workflows/release.yml)) whenever a
version tag is pushed.

## History

Until October 2026 every tool lived in its own repository (`Control-CPX400`,
`ControlAdvanced-CPX400`, `BatteryCyclingTool-NGL202`, `LD400P-correction`,
`CPX400DP-LD400D`, `CPX400DP-Cycle-LD400D`). They were merged here with their
git history.

## License

[MIT](LICENSE).
