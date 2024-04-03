using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.Encryption
{
    public interface IEncryptionService
    {
        (byte[] CipherText, byte[] Iv, byte[] Hmac) EncryptAndHmac(byte[] plaintext);
        byte[] Decrypt(byte[] cipherText, byte[] iv, byte[] hmac);
        string EncryptData(string data);
        string DecryptData(string data);
    }
}
