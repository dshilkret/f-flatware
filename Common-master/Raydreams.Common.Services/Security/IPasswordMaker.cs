using System;
using System.Text;
using Raydreams.Common.Logic;
using Raydreams.Common.Extensions;

namespace Raydreams.Common.Services.Security
{
    /// <summary>Defines a Password Maker</summary>
    public interface IPasswordMaker
    {
        /// <summary>Test a plain text password equals the hash</summary>
        bool Compare( string pw, string hash );

        /// <summary>Turn plain text into a password hash</summary>
        /// <param name="pw"></param>
        /// <returns></returns>
        string HashPassword( string pw );

        /// <summary>Generate a completely random password</summary>
        /// <returns>A tuple of the plain text password and the hash</returns>
        (string Password, string Hash) MakeRandom();
    }

    /// <summary>Null password maker always returns true on a Compare</summary>
    public class NullPassworMaker : IPasswordMaker
    {
        private static readonly string _pw = "password";

        public bool Compare( string pw, string hash )
        {
            return true;
        }

        public string HashPassword( string pw )
        {
            return Convert.ToBase64String( Encoding.UTF8.GetBytes( _pw ), Base64FormattingOptions.None );
        }

        public (string Password, string Hash) MakeRandom()
        {
            return (_pw, this.HashPassword( _pw ));
        }
    }


    /// <summary>Password maker based on MD5 Hashing</summary>
    public class MD5PassworMaker : IPasswordMaker
    {
        public bool Compare( string pw, string hash )
        {
            if ( pw == null || hash == null )
                return false;

            string hash1 = Convert.ToBase64String( pw.HashToMD5(), Base64FormattingOptions.None );

            return hash1 == hash;
        }

        public string HashPassword( string pw )
        {
            if ( String.IsNullOrWhiteSpace( pw ) )
                return null;

            return Convert.ToBase64String( pw.HashToMD5(), Base64FormattingOptions.None );
        }

        public (string Password, string Hash) MakeRandom()
        {
            Randomizer rand = new Randomizer();

            // generate a base of characters
            char[] plainPW = rand.RandomCode( 8, CharSet.NoSimilar ).ToCharArray();

            string pw = new String( plainPW ).Trim();
            string hash = Convert.ToBase64String( pw.HashToMD5(), Base64FormattingOptions.None );

            return (pw, this.HashPassword( pw ));
        }

    }
}
