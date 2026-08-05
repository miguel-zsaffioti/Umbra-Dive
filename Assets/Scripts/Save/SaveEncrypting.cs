using System;
using System.Security.Cryptography;
using System.Text;

public static class SaveEncryption
{
    // Key precisa ter exatamente 32 caracteres (AES-256) e IV exatamente 16.
    private static readonly byte[] Key = Encoding.UTF8.GetBytes("Tr0c4Ess4Ch4v3PraAlg0Seu32Chars!");
    private static readonly byte[] IV = Encoding.UTF8.GetBytes("Tr0c4EsseIV16Ch!");

    public static string Encrypt(string plainText)
    {
        using Aes aes = Aes.Create();
        aes.Key = Key;
        aes.IV = IV;

        using ICryptoTransform encryptor = aes.CreateEncryptor();
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] encrypted = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        return Convert.ToBase64String(encrypted);
    }

    public static string Decrypt(string cipherText)
    {
        using Aes aes = Aes.Create();
        aes.Key = Key;
        aes.IV = IV;

        using ICryptoTransform decryptor = aes.CreateDecryptor();
        byte[] cipherBytes = Convert.FromBase64String(cipherText);
        byte[] decrypted = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
        return Encoding.UTF8.GetString(decrypted);
    }
}