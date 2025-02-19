using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Texnokaktus.ProgOlymp.Api.DataAccess.Converters;

internal class EncryptionConverter(IDataProtector dataProtector) : ValueConverter<string?, string?>(data => Encrypt(data, dataProtector),
                                                                                                    data => Decrypt(data, dataProtector))
{
    private static string? Encrypt(string? data, IDataProtector dataProtector) =>
        data is not null
            ? dataProtector.Protect(data)
            : null;

    private static string? Decrypt(string? data, IDataProtector dataProtector)
    {
        if (data is null)
            return null;

        try
        {
            return dataProtector.Unprotect(data);
        }
        catch (CryptographicException)
        {
            return null;
        }
    }
}
