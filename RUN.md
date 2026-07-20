# Running this repo (quick reference)

For the full picture (Android emulator tips, direct dotnet commands, Visual Studio) see the [How to Run](README.md#how-to-run) section of the README. This file is just the fast path.

## Prerequisite

The backend must already be running — this app has no embedded data. See [HighFidelity-Api's RUN.md](../HighFidelity-Api/RUN.md) (or `run be`, below).

## Every time — from a terminal

From this folder:

```powershell
run.cmd
```

or, from the parent folder that contains both this repo and `HighFidelity-Api` as siblings:

```powershell
run fe
```

Both prompt for Windows or Android and keep running attached to the terminal (Ctrl+C to stop).

## From an editor instead

- **Visual Studio**: open `HighFidelity.Ui.slnx`, pick the framework/device from the debug-target dropdown, F5.
- **VS Code**: `.vscode/launch.json` has a "Run Windows" configuration (Run and Debug panel).

## Only run one at a time

Whichever way you launch it, it's one process — starting a second copy (from a different tool) while one's already running just gets you two windows of the same app, or a build/file-lock conflict if it's still compiling. Stop the running instance before starting another.
