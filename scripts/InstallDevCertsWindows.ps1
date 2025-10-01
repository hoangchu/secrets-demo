dotnet dev-certs https -ep $env:APPDATA\ASP.NET\Https\Secrets.Demo.Api.pfx -p demopassword
dotnet dev-certs https --trust
