using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Secrets.Demo.Api.Configuration;
using Secrets.Demo.Api.Extensions;
using Secrets.Demo.Api.Services;

namespace Secrets.Demo.Api.Controllers;

[ApiController]
public class SecretsController(IOptions<SecretsSettings> secretsSettings) : ControllerBase
{
    private readonly SecretsSettings secrets = secretsSettings.Value;

    [HttpGet("/encrypt")]
    [Tags("Secrets")]
    [EndpointSummary("Encrypt a secret with own symmetric key")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public IActionResult Encrypt(
        [FromQuery] [Required] [Description("Secret to encrypt")] string secret,
        [FromQuery] [Required] [Description("Symmetric encryption key (must be 16, 24, or 32 characters long)")] string symetricKey)
    {
        if (secret.IsNullOrEmpty() || symetricKey.IsNullOrEmpty())
        {
            return this.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid parameters",
                detail: "Secret and symmetric key must not be empty");
        }

        if (!SecretManager.IsValidSecretKey(symetricKey))
        {
            return this.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid key length",
                detail: "Key length must be 16, 24, or 32 characters");
        }

        return this.Ok(SecretManager.Encrypt(secret, symetricKey));
    }

    [HttpGet("/decrypt")]
    [Tags("Secrets")]
    [EndpointSummary("Decrypt a secret with own symmetric key")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public IActionResult Decrypt(
        [FromQuery] [Required] [Description("Encrypted secret to decrypt")] string encryptedSecret,
        [FromQuery] [Required] [Description("Symmetric decryption key (must be 16, 24, or 32 characters long)")] string symetricKey)
    {
        if (encryptedSecret.IsNullOrEmpty() || symetricKey.IsNullOrEmpty())
        {
            return this.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid parameters",
                detail: "Encrypted secret and symmetric key must not be empty");
        }

        if (!SecretManager.IsValidSecretKey(symetricKey))
        {
            return this.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid key length",
                detail: "Key length must be 16, 24, or 32 characters");
        }

        return this.Ok(SecretManager.Decrypt(encryptedSecret, symetricKey));
    }

    [HttpGet("/private/encrypt")]
    [Tags("Secrets with private symetric key")]
    [EndpointSummary("Encrypt a secret with private symmetric key")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public IActionResult PrivateEncrypt([FromQuery] [Required] [Description("Secret to encrypt")] string secret)
    {
        if (secret.IsNullOrEmpty())
        {
            return this.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid parameters",
                detail: "Secret must not be empty");
        }

        if (!SecretManager.IsValidSecretKey(this.secrets.SymmetricKey))
        {
            return this.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid private key",
                detail: "Private symmetric key is not configured properly");
        }

        return this.Ok(SecretManager.Encrypt(secret, this.secrets.SymmetricKey));
    }

    [HttpGet("/private/decrypt")]
    [Tags("Secrets with private symetric key")]
    [EndpointSummary("Decrypt a secret with private symmetric key")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public IActionResult Decrypt([FromQuery] [Required] [Description("Encrypted secret to decrypt")] string encryptedSecret)
    {
        if (encryptedSecret.IsNullOrEmpty())
        {
            return this.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid parameters",
                detail: "Encrypted secret must not be empty");
        }

        if (!SecretManager.IsValidSecretKey(this.secrets.SymmetricKey))
        {
            return this.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid private key",
                detail: "Private symmetric key is not configured properly");
        }

        return this.Ok(SecretManager.Decrypt(encryptedSecret, this.secrets.SymmetricKey));
    }
}