using System;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Raydreams.Common.Extensions;
using Raydreams.Common.Security;

namespace Raydreams.Common.Services.Security
{
    /// <summary>Encrypts and decrypts Session Tokens using a very weak static password</summary>
    /// <remarks>Purely for testing. Do not use in production.</remarks>
    public class SimpleTokenManager : ITokenManager
    {
        /// <summary>The secret used to make a symmetric key. Store in Key Vault or a file or other DB.</summary>
        private string _key = "password";

        /// <summary>Random salting of the key to protect against dictionary attacks. At least 8 bytes.</summary>
        private byte[] _salt = new byte[] { 0x22, 0xb1, 0xdf, 0xd9, 0x8c, 0x19, 0x97, 0x0c, 0x2b, 0xd0, 0x71, 0x86, 0x4e, 0x36, 0x7f, 0x55 };

        /// <summary>How many interations (1000 is default)</summary>
        private int _iterations = 1000;

        #region [ Constructor ]

        /// <summary></summary>
        /// <param name="pw">The plain text password to use to crypt the token. This is NOT the AES key.</param>
        public SimpleTokenManager( string pw, bool bigKey = false )
        {
            this._key = pw;
            this.BigKey = bigKey;
        }

        #endregion [ Constructor ]

        /// <summary>If true will use a 256 bit key. Default is 128.</summary>
        public bool BigKey { get; set; }

        /// <summary>Generates a key from a string password and returns as a byte array</summary>
        /// <remarks>Generates a 128 bit key</remarks>
        protected byte[] TokenPassword
        {
            get
            {
                byte[] key = Encoding.UTF8.GetBytes( this._key );
                Rfc2898DeriveBytes pwdGen = new Rfc2898DeriveBytes( key, this._salt, _iterations );
                return pwdGen.GetBytes( ( this.BigKey ) ? 32 : 16 );
            }
        }

        #region [ Methods ]

        /// <summary>Decodes a token back to the Payload object</summary>
        public TokenPayload Decode( string token )
        {
            // split the string token on the deliminator
            string[] parts = token.Split( '$', StringSplitOptions.RemoveEmptyEntries );
            byte[] msg = parts[1].BASE64UrlDecode();

            // decrypt it
            SymmetricEncryptor enc = new SymmetricEncryptor( SymmetricAlgoType.AES );
            string json = enc.Decrypt( msg, this.TokenPassword, parts[0].BASE64UrlDecode() );
            enc.Clear();

            // deserialize it
            TokenPayload result = JsonConvert.DeserializeObject<TokenPayload>( json );

            return result;
        }

        /// <summary>Creates a new token from input values</summary>
        public string Encode( TokenPayload payload )
        {
            // validate the token
            if ( payload == null || !payload.IsValid() )
                return null;

            // turn to a string
            string token = JsonConvert.SerializeObject( payload );

            // encrypt it
            //KeyVaultRepository kv = new KeyVaultRepository( this.Config.AppClientID, this.Config.AppClientPW );
            SymmetricEncryptor enc = new SymmetricEncryptor( SymmetricAlgoType.AES );
            CipherMessage tok = enc.Encrypt( token, this.TokenPassword );
            enc.Clear();

            // URL encode the token with the IV on the front
            return $"{StringExtensions.BASE64UrlEncode( tok.IV )}${StringExtensions.BASE64UrlEncode( tok.CipherBytes )}";
        }

        #endregion [ Methods ]
    }
}
