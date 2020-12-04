using System;
using System.Security.Cryptography;
using System.Text;

namespace Raydreams.Common.Services.Security
{
    public interface IKeyMaker
    {
        byte[] MakeKey( string pw, byte[] salt, int iterations = 1 );
    }

	/// <summary></summary>
    public class FormFieldKeyMaker : IKeyMaker
    {
		/// <summary>Generates a symmetric key to use from a string password</summary>
		/// <remarks>Remove from this class - app specific</remarks>
		public byte[] MakeKey( string pw, byte[] salt, int iterations = 10 )
		{
			// valiadate
			if ( pw == null )
				pw = String.Empty;

			if ( iterations < 1 )
				iterations = 1;

			byte[] key = Encoding.UTF8.GetBytes( pw );
			Rfc2898DeriveBytes pwdGen = new Rfc2898DeriveBytes( key, salt, iterations );

			// size of the key has to match the algorithm
			return pwdGen.GetBytes( 32 );
		}
	}
}
