using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

using Secrets.Demo.Api.Extensions;

namespace Secrets.Demo.Api.Services;

public static partial class SecretManager
{
    private const string SecretPrefix = "SECRET:";
    private const int IvLength = 16;
    private const int ValidKeyLength128 = 16;
    private const int ValidKeyLength192 = 24;
    private const int ValidKeyLength256 = 32;

    public static string Encrypt(string secret, string secretKey)
    {
        var key = Encoding.UTF8.GetBytes(secretKey);
        using var aes = Aes.Create();
        var initVector = aes.IV;
        using var encryptor = aes.CreateEncryptor(key, initVector);
        using var memoryStream = new MemoryStream();
        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
        using (var streamWriter = new StreamWriter(cryptoStream))
        {
            streamWriter.Write(secret);
        }

        var encryptedContent = memoryStream.ToArray();
        var result = CombineIvAndCipher(initVector, encryptedContent);
        return Convert.ToBase64String(result);
    }

    public static string DecryptPrefixedSecret(string secret, string secretKey) =>
        IsPrefixedSecret(secret) ? Decrypt(secret, secretKey) : secret;

    public static string Decrypt(string secret, string secretKey)
    {
        secret = RemoveSecretPrefix(secret);
        var fullCipher = Convert.FromBase64String(secret);
        var (initVector, cipher) = SeparateIvAndCipher(fullCipher);
        var key = Encoding.UTF8.GetBytes(secretKey);
        using var aes = Aes.Create();
        using var decryptor = aes.CreateDecryptor(key, initVector);
        using var memoryStream = new MemoryStream(cipher);
        using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
        using var streamReader = new StreamReader(cryptoStream);
        var result = streamReader.ReadToEnd();
        return result;
    }

    public static bool IsValidSecretKey(string key)
    {
        if (key.IsNullOrEmpty()) return false;
        return key.Length is ValidKeyLength128 or ValidKeyLength192 or ValidKeyLength256;
    }

    private static bool IsPrefixedSecret(string secret) =>
        !secret.IsNullOrEmpty() && secret.StartsWith(SecretPrefix, StringComparison.InvariantCultureIgnoreCase);

    private static string RemoveSecretPrefix(string secret) => SecretPrefixRegex().Replace(secret, "");

    private static byte[] CombineIvAndCipher(byte[] initVector, byte[] encryptedContent)
    {
        var result = new byte[initVector.Length + encryptedContent.Length];
        Buffer.BlockCopy(initVector, 0, result, 0, initVector.Length);
        Buffer.BlockCopy(encryptedContent, 0, result, initVector.Length, encryptedContent.Length);
        return result;
    }

    private static (byte[] initVector, byte[] cipher) SeparateIvAndCipher(byte[] fullCipher)
    {
        var initVector = new byte[IvLength];
        var cipher = new byte[fullCipher.Length - initVector.Length];
        Buffer.BlockCopy(fullCipher, 0, initVector, 0, initVector.Length);
        Buffer.BlockCopy(fullCipher, initVector.Length, cipher, 0, fullCipher.Length - initVector.Length);
        return (initVector, cipher);
    }

    [GeneratedRegex("^SECRET:", RegexOptions.IgnoreCase | RegexOptions.Compiled, "nl-NL")]
    private static partial Regex SecretPrefixRegex();
}