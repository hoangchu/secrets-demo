FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

USER $APP_UID
WORKDIR /app
EXPOSE 5080
EXPOSE 5443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["src/Secrets.Demo.Api/Secrets.Demo.Api.csproj", "src/Secrets.Demo.Api/"]
RUN dotnet restore "./src/Secrets.Demo.Api/Secrets.Demo.Api.csproj"
COPY . .
WORKDIR "/src/src/Secrets.Demo.Api"
RUN dotnet build "./Secrets.Demo.Api.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "./Secrets.Demo.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Secrets.Demo.Api.dll"]