using System;
using Raydreams.Common.Logic;

namespace Raydreams.Common.Services.Security
{
	/// <summary>Logic for creating new temp PWs and verifying existing ones.</summary>
	/// <remarks>Need to be able to set the randomizer on construction. Needs to be an interface class</remarks>
	public class BCryptPasswordMaker : IPasswordMaker
	{
		/// <summary>Checks a plain text string is valid against the hash</summary>
		/// <returns>The compare.</returns>
		/// <param name="pw">Plain text PW</param>
		/// <param name="hash">stroed hash of the PW</param>
		public bool Compare( string pw, string hash )
		{
			if ( pw == null || hash == null )
				return false;

			return BCrypt.Net.BCrypt.Verify( pw, hash );
		}

		/// <summary>Hashes a plain text PW using the BCrypt algo</summary>
		/// <param name="pw">PLain text PW to hash</param>
		/// <returns></returns>
		public string HashPassword( string pw )
		{
			if ( String.IsNullOrWhiteSpace( pw ) )
				return null;

			string hash = BCrypt.Net.BCrypt.HashPassword( pw );

			return hash;
		}

		/// <summary>Generate a random pw and return it along with its hash</summary>
		/// <returns>Returns a tuple with the plain text PW first followed by the hashed version</returns>
		/// <remarks>Used both for the dashboard and app</remarks>
		public (string Password, string Hash) MakeRandom()
		{
			Randomizer rand = new Randomizer();

			// generate a base of characters
			char[] plainPW = rand.RandomCode( 6, CharSet.Upper | CharSet.Lower | CharSet.NoSimilar ).ToCharArray();
			Array.Resize( ref plainPW, 8 );

			// add in a couple special chars
			plainPW[6] = rand.RandomChar( CharSet.Digits | CharSet.NoSimilar );
			plainPW[7] = rand.RandomChar( CharSet.LimitedSpecial | CharSet.NoSimilar );

			// shuffle the characters again
			for ( int idx = 0; idx < plainPW.Length - 1; ++idx )
			{
				// pick a location
				int pick = rand.RandomInt( idx + 1, plainPW.Length - 1 );

				// now swap the current and the random location
				char temp = plainPW[idx];
				plainPW[idx] = plainPW[pick];
				plainPW[pick] = temp;
			}

			// hash it
			string pw = new String( plainPW ).Trim();
			string hash = BCrypt.Net.BCrypt.HashPassword( pw );

			return (pw, hash);
		}

	}
}
