Layout fixes: nothing is cut off any more.

- `cpx400dp-ld400p-cycling`: the *Last EIN/AUS* and *Ausgang 1 EIN/AUS* buttons, the run counter and the *Export CSV* button were cut off; the group title shows "Messwerte & SOH" again.
- `cpx400dp-ld400p-switch`: the connect buttons read "Verbinden" instead of "Verbind".
- `ngl202-battery-cycling`: the *Nennkapazität* row and the status line were cut off.
- `cpx400-control`: the menu bar no longer covers the top of the window, and the output list shows "Ausgang 1 (Master)" in full.

All six tools as single `.exe` files, built by GitHub Actions.

- `ngl202-battery-cycling.exe` needs the .NET 10 Desktop Runtime.
- The other tools run on the .NET Framework 4.7.2 built into Windows 10 and 11.
- The files are not signed; SmartScreen may warn (*More info › Run anyway*).
