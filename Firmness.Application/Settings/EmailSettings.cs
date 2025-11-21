namespace Firmness.Application.Settings;

public class EmailSettings
{
    public string From { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Password { get; set; } = string.Empty;
}