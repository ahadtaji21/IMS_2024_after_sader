using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace jrs.Services
{
    public class CryptoService
    {
        private IConfiguration _config;
        public CryptoService(IConfiguration config)
        {
            this._config = config;
        }

        // public string Decrypt(string encpwd) {
        //      string privKey = _config.GetValue<string>("AppSettings:RSAKeys");
        //     RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
        //     RSA.FromXmlString(privKey);
        //     var bytes = Convert.FromBase64String(encpwd);                    
        //     var bytesPlainText = RSA.Decrypt(bytes,true);
        //     return System.Text.Encoding.UTF8.GetString(bytesPlainText);
        // }

        ///<Summary>
        /// Returns the Hash conversion of the given string using sha256 cryptographic hash functions.
        ///</Summary>
        public string GetHash(string str)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(str));

                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                var sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                // Return the hexadecimal string.
                return sBuilder.ToString();
            }
        }

    }
}