namespace Monitorizare.Settings;

public record FTPSettings(string Host, string User, string Pass, string LocalPath, string ServerPath)
{
    public static FTPSettings Create(IAppSettings settings)
    {
        var server = settings.ServerDetails;

        return new FTPSettings(
            server["Host"] ?? throw new ArgumentException("Host field is required."),
            server["User"] ?? throw new ArgumentException("User field is required."),
            server["Pass"] ?? throw new ArgumentException("Password field is required."),
            server["LocalPath"] ?? "logs/",
            server["ServerPath"] ?? string.Empty
        );
    }
}