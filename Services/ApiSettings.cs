namespace HighFidelity.Ui.Services;

/// <summary>
/// Frontend-side configuration for reaching the HighFidelity.Api backend,
/// which lives in its own repo: https://github.com/srai54/HighFidelity-Api
/// Kept as its own file (rather than inline in MauiProgram) so the FE's
/// "where's the backend" concern is visually separate from DI wiring.
/// </summary>
public static class ApiSettings
{
    // The Android emulator can't see the host's "localhost"; 10.0.2.2 is its
    // loopback alias to the host machine. Physical devices need the host's LAN IP.
#if ANDROID
    public const string BaseAddress = "http://10.0.2.2:5199/";
#else
    public const string BaseAddress = "http://localhost:5199/";
#endif

    public static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(10);

    // Matches the seeded row in HighFidelity.Api's dbo.Users table
    // (database/seed.sql) — see HighFidelity-Api/docs/ARCHITECTURE.md.
    public const string DemoUsername = "admin";
    public const string DemoPassword = "ChangeMe123!";
}
