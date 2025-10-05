namespace Secrets.Demo.Api.Configuration;

public class SecretsSettings
{
    public required string ServerName { get; set; }
    public required string ServerDescription { get; set; }
    public required string ServerUrl { get; set; }
    public required string SymmetricKey { get; set; }
}