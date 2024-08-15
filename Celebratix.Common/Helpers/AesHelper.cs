﻿using System.Security.Cryptography;
using System.Text;
using F = CSharpFunctionalExtensions;

namespace Celebratix.Common.Helpers;

public static class AesHelper
{
    public static string EncryptString(string key, string plainText)
    {
        var iv = new byte[16];

        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key);
        aes.IV = iv;

        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        using var memoryStream = new MemoryStream();
        using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
        // Needs to be enclosed in a using block or be flushed manually)
        using (var streamWriter = new StreamWriter(cryptoStream))
        {
            streamWriter.Write(plainText);
        }

        var array = memoryStream.ToArray();

        return Convert.ToBase64String(array);
    }

    public static F.Maybe<string> DecryptString(string key, string cipherText)
    {
        try
        {
            var iv = new byte[16];
            var buffer = Convert.FromBase64String(cipherText);

            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = iv;
            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using var memoryStream = new MemoryStream(buffer);
            using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            using var streamReader = new StreamReader(cryptoStream);

            return streamReader.ReadToEnd();
        }
        catch (Exception)
        {
            return F.Maybe.None;
        }
    }
}
