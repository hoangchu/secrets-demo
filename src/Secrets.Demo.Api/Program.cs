using Scalar.AspNetCore;

using Secrets.Demo.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<SecretsSettings>(builder.Configuration.GetSection("Secrets"));
builder.Services.AddProblemDetails();
builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Title = "Secrets Demo API";
        document.Info.Version = "v1";
        document.Info.Description =
            "API for encrypting and decrypting secrets using symmetric key encryption.";

        return Task.CompletedTask;
    });
});

var app = builder.Build();
app.UseExceptionHandler();
app.UseStatusCodePages();
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options
        .WithTitle("Secrets Demo API")
        .WithTheme(ScalarTheme.Kepler)
        .WithDarkModeToggle()
        .WithDefaultOpenAllTags()
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();