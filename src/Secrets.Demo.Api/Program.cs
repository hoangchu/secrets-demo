using Microsoft.OpenApi.Models;

using Scalar.AspNetCore;

using Secrets.Demo.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);
var secretsSection = builder.Configuration.GetSection("Secrets");
var secretSettings = secretsSection.Get<SecretsSettings>();
builder.Services.Configure<SecretsSettings>(secretsSection);
builder.Services.AddProblemDetails();
builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Title = secretSettings.ServerName;
        document.Info.Version = "v1";
        document.Info.Description = secretSettings.ServerDescription + $" Configured private symmetric key is: {secretSettings.SymmetricKey}";
        document.Servers = [new OpenApiServer { Url = secretSettings.ServerUrl, Description = secretSettings.ServerName }];
        return Task.CompletedTask;
    });
});

var app = builder.Build();
app.UseExceptionHandler();
app.UseStatusCodePages();
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.Servers = [new ScalarServer(secretSettings.ServerUrl, secretSettings.ServerName)];
    options
        .WithTitle(secretSettings.ServerName)
        .WithTheme(ScalarTheme.Kepler)
        .WithDarkModeToggle()
        .WithDefaultOpenAllTags()
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();