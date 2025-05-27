namespace Monitorizare.Settings;

internal class FTPServerSettings
{
    public readonly string Host;
    public readonly string User;
    public readonly string Pass;
    public readonly string LocalPath;
    public readonly string ServerPath;

    public FTPServerSettings(IAppSettings settings)
    {
        var server = settings.GetServerDetails();
        Host = server["Host"] ?? throw new ArgumentException("Host field is required.");
        User = server["User"] ?? throw new ArgumentException("User field is required.");
        Pass = server["Pass"] ?? throw new ArgumentException("Password field is required.");
        LocalPath = server["LocalPath"] ?? "logs/";
        ServerPath = server["ServerPath"] ?? string.Empty;
    }
}