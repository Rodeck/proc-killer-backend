using Newtonsoft.Json;
using ProcastinationKiller.Services;
using ProcastinationKiller.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using Xunit;

namespace TodosTests
{
    public class EncryptorTests
    {
        [Theory]
        [InlineData("SomeTextToEncrypt")]
        [InlineData("")]
        [MemberData(nameof(encodingData))]
        public void CanEncrypt(string toEncode)
        {
            IEncryptor encryptor = new Encryptor(GetSecureString());

            var encrypted = encryptor.Encrypt(toEncode);

            Assert.NotEqual(toEncode, encrypted);
        }

        [Theory]
        [InlineData("SomeTextToEncrypt")]
        [InlineData("")]
        [MemberData(nameof(encodingData))]
        public void CanEncryptAndDecrypt(string toEncode)
        {
            IEncryptor encryptor = new Encryptor(GetSecureString());

            var encrypted = encryptor.Encrypt(toEncode);

            Assert.NotEqual(toEncode, encrypted);

            var decrypted = encryptor.Decrypt(encrypted);

            Assert.Equal(toEncode, decrypted);
        }

        public static IEnumerable<object[]> encodingData()
        {
            yield return new object[]
            {
                JsonConvert.SerializeObject(new {
                    UserName = ""
                })
            }
            
            ;yield return new object[]
            {
                JsonConvert.SerializeObject(new {
                    UserName = "asdasdasd"
                })
            }
            
            ;yield return new object[]
            {
                JsonConvert.SerializeObject(new {
                    UserName = "111111111"
                })
            };
        }

        #region helpers

        private SecureString GetSecureString()
        {
            var secureString = new SecureString();
            secureString.AppendChar('s');
            secureString.AppendChar('1');
            secureString.AppendChar('^');
            secureString.AppendChar('2');
            secureString.AppendChar('5');
            secureString.AppendChar('9');
            secureString.AppendChar('0');
            secureString.AppendChar(')');
            secureString.AppendChar(')');

            return secureString;
        }

        #endregion
    }
}
