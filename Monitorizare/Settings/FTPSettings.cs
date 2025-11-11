namespace Monitorizare.Settings;

public sealed record FTPSettings
{
    public string Host { get; }
    public string User { get; }
    public string Pass { get; }
    public string LocalPath { get; }
    public string ServerPath { get; }

    private FTPSettings(string? host, string? user, string? pass, string? localPath, string? serverPath)
    {
        Host = host ?? throw new ArgumentException("Host field is required.");
        User = user ?? throw new ArgumentException("User field is required.");
        Pass = pass ?? throw new ArgumentException("Password field is required.");
        LocalPath = localPath ?? "logs/";
        ServerPath = serverPath ?? string.Empty;
    }

    public static FTPSettings Create(IAppSettings settings)
    {
        var server = settings.ServerDetails;

        return new FTPSettings
        (
            server.GetValueOrEmpty("Host"),
            server.GetValueOrEmpty("User"),
            server.GetValueOrEmpty("Pass"),
            server.GetValueOrEmpty("LocalPath"),
            server.GetValueOrEmpty("ServerPath")
        );
    }
}