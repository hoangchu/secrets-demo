# Secrets Demo
A demo ASP.NET Core API showcasing secrets encryption and decryption with a symmetric key, GitOps-based deployment practices, and debugging a Dockerized web app with HTTPS support in both Visual Studio and JetBrains Rider.

## Getting started
Clone and open the solution in your favorite IDE: Visual Studio or JetBrains Rider.

To make the application fully functional, set a user secret `Secrets:SymmetricKey` with a 256-bit string value by running the following command from the project folder:

```powershell
dotnet user-secrets init
dotnet user-secret set "Secrets:SymmetricKey" "5PsLWDPDxchr1x3HVtJ2RhIKhGbi4iNG"
```
Or, you can set the `Secrets__SymmetricKey` environment variable to the same value. This 256-bit string is used for symmetric encryption by the demo app and can be any randomly generated 256-bit value.

Using an environment variable is also a common approach when deploying the application to Kubernetes.

## JetBrains Rider - HTTPS Debugging in Docker
If you want to debug the Docker container with HTTPS support in Rider, you need to install the development certificates first.
Run the following PowerShell script with administrator privileges:
```powershell
.\scripts\InstallDevCertsWindows.ps1
```
That will install the development certificates in the current user's certificate store and make an export available in the location for Rider to pick it up and mount it into the container.
Once that's in place you can debug the application with the launch profile `Rider:Docker`.

## Visual Studio - HTTPS Debugging in Docker
Visual Studio integrates well with Docker, so you can debug the application using the `VisualStudio:Docker` launch profile.

If you use both Visual Studio and Rider (for example, for testing), run `InstallDevCertsWindows.ps1` first. Then, start debugging using the `RiderVisualStudio:Docker` launch profile in Visual Studio, and the `Rider:Docker` launch profile in Rider.
