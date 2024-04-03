using Library.Domain.Entity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.Encryption
{
    public class EncryptionService : IEncryptionService
    {
        private readonly EncryptionSettings _encryptionSettings;

        public EncryptionService(IOptions<EncryptionSettings> encryptionSettings)
        {
            _encryptionSettings = encryptionSettings.Value;
        }

        public (byte[] CipherText, byte[] Iv, byte[] Hmac) EncryptAndHmac(byte[] plaintext)
        {
            using (var aes = Aes.Create())
            {
                byte[] aesKeyBytes = Encoding.UTF8.GetBytes(_encryptionSettings.AesKey);
                aes.Mode = CipherMode.CBC;

                byte[] iv = Convert.FromBase64String(_encryptionSettings.Iv);

                using (var encryptor = aes.CreateEncryptor(aesKeyBytes, iv))
                using (var resultStream = new MemoryStream())
                {
                    using (var aesStream = new MemoryStream(plaintext))
                    using (var cryptoStream = new CryptoStream(resultStream, encryptor, CryptoStreamMode.Write))
                    {
                        aesStream.CopyTo(cryptoStream);
                    }

                    var cipherText = resultStream.ToArray();

                    byte[] hmacKeyBytes = Encoding.UTF8.GetBytes(_encryptionSettings.HmacKey);
                    using (var hmac = new HMACSHA256(hmacKeyBytes))
                    {
                        var hmacBytes = hmac.ComputeHash(cipherText);
                        return (cipherText, iv, hmacBytes);
                    }
                }
            }
        }

        public string EncryptData(string data)
        {
            if (string.IsNullOrEmpty(data))
                return null;

            var dataBytes = Encoding.UTF8.GetBytes(data);
            var encryptedData = EncryptAndHmac(dataBytes);
            return Convert.ToBase64String(encryptedData.CipherText);
        }

        public byte[] Decrypt(byte[] cipherText, byte[] iv, byte[] hmac)
        {
            using (var hmacSha = new HMACSHA256(Encoding.UTF8.GetBytes(_encryptionSettings.HmacKey)))
            {
                using (var aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(_encryptionSettings.AesKey);
                    aes.IV = iv;
                    aes.Mode = CipherMode.CBC;

                    using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    using (var resultStream = new MemoryStream())
                    {
                        using (var aesStream = new MemoryStream(cipherText))
                        using (var cryptoStream = new CryptoStream(aesStream, decryptor, CryptoStreamMode.Read))
                        {
                            cryptoStream.CopyTo(resultStream);
                        }

                        return resultStream.ToArray();
                    }
                }
            }
        }

        public string DecryptData(string data)
        {
            if (string.IsNullOrEmpty(data))
                return null;

            try
            {
                byte[] encryptedBytes = Convert.FromBase64String(data);
                byte[] hmacKeyBytes = Encoding.UTF8.GetBytes(_encryptionSettings.HmacKey);
                byte[] iv = Convert.FromBase64String(_encryptionSettings.Iv);

                byte[] decryptedBytes = Decrypt(encryptedBytes, iv, hmacKeyBytes);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
            catch (FormatException ex)
            {
                throw new FormatException("Invalid base64 string format.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to decrypt data.", ex);
            }
        }
    }
}