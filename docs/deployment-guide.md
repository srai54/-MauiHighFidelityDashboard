# Deployment Guide — Windows & Android

Step-by-step instructions for building, running, and shipping **MauiHighFidelityDashboard**, plus 20 interview-level Q&A at the end.

## Project facts (from the `.csproj`)

| Setting | Value | Meaning |
|---|---|---|
| Target frameworks (on Windows) | `net10.0-windows10.0.19041.0`, `net10.0-android` | One project, one binary per platform |
| App ID | `com.companyname.mauihighfidelitydashboard` | Android package name / app identity |
| `ApplicationDisplayVersion` | `1.0` | Human-readable version (`versionName` on Android) |
| `ApplicationVersion` | `1` | Build/store number (`versionCode` on Android) — must increase every store release |
| `WindowsPackageType` | `None` | Windows runs as a plain **unpackaged** `.exe` (no MSIX, no certificate) |
| Min OS | Android 5.0 (API 21) / Windows 10 1809 (17763) | Older devices cannot install/run the app |

> There is **no web target**. .NET MAUI compiles to Windows, Android, iOS, and macOS. A browser version would require a separate Blazor (web) project sharing the same C# models/services.

---

## Part 1 — Windows deployment

### A. Developer run (day-to-day)

1. Open a terminal in the repo root.
2. Run either:
   ```powershell
   .\run.cmd          # choose [1] Windows
   # or directly:
   dotnet build MauiHighFidelityDashboard.csproj -t:Run -f net10.0-windows10.0.19041.0
   ```
3. The app builds and launches maximized. Because `WindowsPackageType` is `None`, this is a plain process — no sideloading or Developer Mode needed.

> **Common failure:** `MSB3027: could not copy apphost.exe — file locked`. A previous instance of the app is still running; close it (or `Stop-Process -Name MauiHighFidelityDashboard`) and rebuild.

### B. Release build for distribution (unpackaged)

1. Publish a Release build:
   ```powershell
   dotnet publish MauiHighFidelityDashboard.csproj -f net10.0-windows10.0.19041.0 -c Release
   ```
2. Collect the output folder:
   ```
   bin\Release\net10.0-windows10.0.19041.0\win-x64\publish\
   ```
3. Zip that folder and hand it to the user. They unzip and run `MauiHighFidelityDashboard.exe` — the .NET runtime is included (self-contained), so nothing needs to be installed first.
4. Smoke-test on a clean machine: launch, check charts render, add/delete an order, print preview.

### C. Optional: Microsoft Store / MSIX path

Only needed for Store distribution or enterprise MSIX deployment:

1. Remove `<WindowsPackageType>None</WindowsPackageType>` (packaged is the default).
2. Associate the app with the Store in Visual Studio (**Project → Publish**), or sign the MSIX with a trusted code-signing certificate.
3. Bump `ApplicationDisplayVersion`/`ApplicationVersion` for every submission.

---

## Part 2 — Android deployment

### A. Developer run on the emulator

1. Quickest path:
   ```powershell
   .\run.cmd          # choose [2] Android
   ```
   The script checks `adb devices`; if nothing is connected it boots the **Pixel 7 (API 36)** AVD with software rendering, waits for `sys.boot_completed`, then builds + installs + launches.
2. Manual equivalent:
   ```powershell
   $sdk = "C:\Program Files (x86)\Android\android-sdk"
   & "$sdk\emulator\emulator.exe" -avd pixel_7_-_api_36_0 -gpu swiftshader_indirect -no-snapshot -no-boot-anim -no-audio
   dotnet build MauiHighFidelityDashboard.csproj -t:Run -f net10.0-android
   ```

> **Emulator patience:** on a cold boot with software rendering, the first launch can sit on the .NET splash for 30–60 s, and Play Store background updates can even trigger an "isn't responding" (ANR) dialog. Choose **Wait** — it recovers once the system settles. A physical device does not have this problem.

### B. Developer run on a physical device

1. On the phone: enable **Developer options → USB debugging**, connect USB, accept the RSA prompt.
2. Verify it's visible: `adb devices` (shows `<serial>  device`).
3. Deploy: `dotnet build MauiHighFidelityDashboard.csproj -t:Run -f net10.0-android`.

### C. Signed release APK (direct install / sideload)

1. Create a keystore **once** and keep it safe — losing it means you can never update the installed app:
   ```powershell
   keytool -genkeypair -v -keystore dashboard.keystore -alias dashboard `
           -keyalg RSA -keysize 2048 -validity 10000
   ```
2. Publish a signed Release APK:
   ```powershell
   dotnet publish MauiHighFidelityDashboard.csproj -f net10.0-android -c Release `
     -p:AndroidKeyStore=true `
     -p:AndroidSigningKeyStore=dashboard.keystore `
     -p:AndroidSigningKeyAlias=dashboard `
     -p:AndroidSigningStorePass=<store-password> `
     -p:AndroidSigningKeyPass=<key-password>
   ```
3. Grab the `-Signed.apk` from `bin\Release\net10.0-android\publish\`.
4. Install it: `adb install -r <name>-Signed.apk`, or share the file directly (users must allow "install unknown apps").

### D. Release AAB (Google Play)

1. Same command as above plus `-p:AndroidPackageFormats=aab` (AAB is also the default packaging for Release publish).
2. Increment `<ApplicationVersion>` — Play rejects a `versionCode` it has already seen.
3. Upload the `.aab` in Play Console → create release → roll out. With **Play App Signing**, your keystore becomes the *upload key* and Google holds the final *app signing key*.

### E. Quick troubleshooting

| Symptom | Cause / fix |
|---|---|
| `adb` not recognized | SDK isn't on PATH — use the full path `C:\Program Files (x86)\Android\android-sdk\platform-tools\adb.exe` |
| Emulator vanished from `adb devices` | Emulator process crashed (flaky under software rendering) — restart it, `adb kill-server; adb start-server` if needed |
| App stuck on .NET splash | Cold emulator + JIT first start — wait; check the process is alive with `adb shell pidof com.companyname.mauihighfidelitydashboard` |
| "isn't responding" dialog | Emulator CPU starved (e.g. Play Store update storm after boot) — tap **Wait**, let the system settle, relaunch |
| Release app crashes where Debug worked | Linker/trimming removed reflection-used code — keep the type or add a `[DynamicDependency]`/linker config |

---

## Part 3 — 20 interview-level questions & answers

**1. Why does the `.csproj` declare `TargetFrameworks` conditionally on the OS?**
Because each platform head needs its host OS tooling: Windows targets (WinUI) only build on Windows, while iOS/Mac Catalyst need macOS. The condition gives Windows machines `net10.0-windows...;net10.0-android` and non-Windows machines the mobile/mac set, so `dotnet build` never attempts an impossible target.

**2. What's the difference between `dotnet build -t:Run` and `dotnet publish`?**
`-t:Run` builds the Debug binary and immediately launches it on the selected target (desktop process or connected Android device) — a developer loop. `dotnet publish` produces the distributable output: self-contained folder for Windows, signed APK/AAB for Android, with Release optimizations (linking/trimming) applied.

**3. What does `WindowsPackageType=None` mean and what are the trade-offs?**
The Windows app runs as a plain unpackaged `.exe` — instant F5, no certificate, no installer. The trade-off: no MSIX identity, so no Store distribution, no clean uninstall database entry, and no package-based auto-update; those require switching back to MSIX packaging and signing.

**4. Why can't plain `dotnet run` be used in a multi-targeted MAUI project?**
`dotnet run` resolves a single target framework; with several TFMs it's ambiguous and errors. You must pass `-f <tfm>` — which is exactly what `run.cmd` automates by asking "Windows or Android?" first.

**5. What is a Runtime Identifier (RID) and where does it appear here?**
A RID (e.g. `win-x64`, `android-arm64`) tells .NET which OS/CPU to produce native assets for. The Windows publish output lands in `...\win-x64\publish\`; Android bundles multiple ABIs inside the APK/AAB and the device picks its own at install time.

**6. APK vs AAB — when do you use each?**
APK is the installable artifact — right for sideloading, QA handoffs, and CI smoke tests. AAB is a publishing format for Google Play: the store generates per-device split APKs from it (smaller downloads) and it's mandatory for new Play apps.

**7. How does Android release signing work in .NET MAUI?**
You generate a keystore with `keytool`, then pass `AndroidKeyStore=true` plus keystore path, alias, and passwords as MSBuild properties (or csproj entries) to `dotnet publish`. The output is a `-Signed.apk`/`.aab`; Android verifies the signature on install and rejects updates signed with a different key.

**8. What happens if you lose the keystore?**
Installed users can never receive an update — Android treats a differently-signed build as a different app, forcing uninstall/reinstall (losing local data). With Play App Signing, Google escrows the app signing key, so a lost *upload* key can be reset — a key reason to enroll.

**9. Difference between `ApplicationVersion` and `ApplicationDisplayVersion`?**
`ApplicationDisplayVersion` (1.0) is the human-facing `versionName`. `ApplicationVersion` (1) is the monotonically increasing integer (`versionCode` on Android, build number elsewhere) that stores compare — every Play upload must raise it, even for the same display version.

**10. What changes between Debug and Release builds on Android?**
Debug uses Fast Deployment (assemblies pushed beside the app, no full repackage), no linking, and JIT — fast iteration, big and slow. Release repackages everything, runs the trimmer/linker to strip unused IL, can enable AOT profiling, and must be signed — smaller, faster, but stricter.

**11. What is the linker/trimmer and what's the classic bug it causes?**
It removes code the static analyzer thinks is unreachable to shrink the app. Anything reached only via reflection — XAML bindings to otherwise-unused members, DI-activated types, JSON serialization by name — can be stripped, causing Release-only crashes; fixes include `DynamicDependency`, linker descriptors, or source-generated alternatives.

**12. What is Fast Deployment on Android?**
A Debug-only trick where .NET assemblies are copied to the device's app directory instead of being embedded in the APK, so most code changes redeploy in seconds without reinstalling the package. It's automatically off for Release.

**13. What must be true before `dotnet build -t:Run -f net10.0-android` reaches a device?**
`adb` must list the target as `device` (emulator booted with `sys.boot_completed=1`, or a phone with USB debugging authorized), and the device API level must be ≥ the project's minimum (21 here). The build then installs via adb and fires the launch intent.

**14. What role does `ApplicationId` play on each platform?**
On Android it's the package name — the permanent identity; changing it publishes a *different* app. On packaged Windows it feeds the MSIX identity; unpackaged (this project) it's informational. Reverse-DNS naming (`com.company.app`) is the convention.

**15. How would you CI/CD this project with GitHub Actions?**
Use a `windows-latest` runner (only Windows can build both targets here): `dotnet workload install maui`, restore, `dotnet publish` each TFM, then upload artifacts — zip for Windows, signed AAB/APK for Android. Version bump, signing, and store upload (e.g. Play publisher API) run in the release job.

**16. How do you keep the keystore and its passwords out of the repo?**
Never commit them: store the keystore base64-encoded in a CI secret (decoded to a temp file at build time) and pass passwords as secret-backed MSBuild properties or environment variables. Locally, keep them in user-level config outside the working tree.

**17. What do `MauiIcon`/`MauiSplashScreen` do at build time?**
MAUI's single-project resizetizer takes the one SVG source and generates every platform-specific asset — Android mipmap densities, Windows scaled PNGs, splash screens — so no per-platform icon folders are maintained by hand. (The purple `.NET` splash you see on Android comes from `Resources\Splash\splash.svg`.)

**18. What do the `SupportedOSPlatformVersion` values enforce?**
They set the platform floor: Android `minSdkVersion` 21 (older devices can't install) and Windows 10 build 17763 (the runtime refuses to start on older builds). APIs newer than the floor must be guarded with runtime OS checks.

**19. Why did the app publish as *self-contained* on Windows, and what's the alternative?**
MAUI Windows publishes self-contained by default so the target machine needs no .NET install — simplest for handing an unpackaged folder to a reviewer. Framework-dependent output is smaller but requires the exact .NET runtime preinstalled — a support burden for non-technical users.

**20. A Release Android build works on the emulator but ANRs/crashes on a low-end device — first three things you check?**
(1) Main-thread work: long synchronous operations in startup/`OnAppearing` that a slow CPU exposes (ANR = main thread blocked >5 s). (2) Trimmer/AOT differences vs Debug — check a Release build with linking temporarily relaxed. (3) `adb logcat` around the crash/ANR for the real stack trace or "Reason:" block — never guess without it.

---

*Verified commands in this repo: Windows dev-run and publish, Android emulator deploy via `run.cmd` paths (`C:\Program Files (x86)\Android\android-sdk`, AVD `pixel_7_-_api_36_0`).*
