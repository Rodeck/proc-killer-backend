using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Services.Abstract
{
    public interface IEncryptor
    {
        string Encrypt(string text);
        string Decrypt(string encryptedText);
    }
}
