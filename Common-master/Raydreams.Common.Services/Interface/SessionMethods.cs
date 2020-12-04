using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Raydreams.Common.Logging;
using Raydreams.Common.Logic;
using Raydreams.Common.Security;
using Raydreams.Common.Services.Model;
using Raydreams.Common.Services.Security;
using Raydreams.Common.Extensions;

namespace Raydreams.Common.Services.Interface
{
	public partial class CommonGateway : ICommonGateway
	{
		/// <summary>Key Pair used to decrypt the plain text PW</summary>
		private readonly string privateKey = @"<RSAKeyValue><Modulus>yDOHwWnVQr/eErY298lySN9cffp55ZsUbYoL1gGt38t89OzTNvSiUMe7FT/WBoEbx4FvqExM3nhcojQpS17QLDoPpthKA5BB1gyG5DULPtklvnBMSrCj0db/HwV54n8SBg2dLl+LgpiJ3kY0aayXp/vxz+EW7mLzg1rnpj54/s8=</Modulus><Exponent>AQAB</Exponent><P>6pRJHeeRTPk3KqIme8NECNcjrWaZtHnLfgJdYjWgcmfkqs2tpx9yLxobhb6oQM6MqAJ1r/RYg3e0wup4hsHhdw==</P><Q>2nuZDU6eyYnwTbufkIHk6w0fqpxYXXLBANZ4nwFuZUJDpIpv8S/zGCQsng3a6sDC12AS9dNWQku83WsF2/HjaQ==</Q><DP>fOPt+ansRhr5MA2ch4/yjPJd4FgbxWaC3NfNeBgtEDwYFofiyHOJi2JO/OoBEl/lqx2EIuXoqjX1W0ESYkyfCQ==</DP><DQ>adAKZAN7q6f+lecUdGw0bhJBD+QAd+Gnz7wPVxLeYHQjRoE1jd3nIf4DDdkv+Rm6Q4zoId5knF8J7UJz5/3SYQ==</DQ><InverseQ>1gRCt32o4nkQsiSrhaPKRlthzmFbUnusw5Vv/hIV9HbVV2VIebJMKWvBb1d3Kd+pV/R/Ww518ZFEREL2knXkcg==</InverseQ><D>cx0GGxUydZ4DH/k75AAlwXcFJL7IJlikR1PP+G6gvoSvxdkCnZ/OUOp3TQGWm8jS40UuobO1o1nL0cxyTg8nXLJHepwWQUvIc3NnrRq+w0o9+3x8eANS2AGWifd0QWUJmPvVlI8qpqTe9GW2HljlJpQAgnG84JQOVrNTZK2+WtE=</D></RSAKeyValue>";

		private static readonly int keySize = 1024;

		#region [ Salt Delegates ]

		/// <summary>Fixed value salt</summary>
		public static MakeTokenSalt NoSalter = ( int min, int max ) => {
			return "ABCDEFGH";
		};

		/// <summary>Fixed length salt</summary>
		public static MakeTokenSalt LowSalter = ( int min, int max ) => {
			Randomizer rand = new Randomizer();
			return rand.RandomCode( 8, CharSet.Upper | CharSet.Digits );
		};

		/// <summary>variable length salt</summary>
		public static MakeTokenSalt HeavySalter = ( int min, int max ) => {
			if ( min < 1 ) min = 1;
			if ( max > 128 ) max = 128;

			Randomizer rand = new Randomizer();
			int len = rand.RandomInt( min, max );
			return rand.RandomCode( len, CharSet.Upper | CharSet.Digits );
		};

		#endregion [ Salt Delegates ]

		#region [ Session Methods ]

		/// <summary>Creates a new session if the login attempt is successful and the player is not logged in. Replaces any existing session with a new one.</summary>
		/// <param name="userID">user ID which is the user's chosen ID. Later my add email reconciliation.</param>
		/// <param name="pw">plaintext PW</param
		/// <param name="domain">What domain the user is trying to log into</param>
		/// <param name="ip">IP address of the client</param>
		/// <param name="decryptPW">(optional) use this private key to decrypt the incoming PW first</param>
		/// <returns>A login response object with results code and the security token if any</returns>
		/// <remarks>Handle Reset PW scenario</remarks>
		public LoginResponse Login( string userID, string pw, string domain, string ip, bool decryptPW = false )
		{
			// start with invalid input
			LoginResponse results = new LoginResponse() { Results = APIResultType.Unknown };

			// the app may be offline completely

			// validate all input
			if ( String.IsNullOrWhiteSpace( userID ) || String.IsNullOrWhiteSpace( pw ) || String.IsNullOrWhiteSpace( domain ) )
			{
				results.Results = APIResultType.InvalidInput;
				return results;
			}

			// update the input
			results.User.ID = userID.Trim();
			results.Domain = domain.Trim().ToLowerInvariant();

			try
			{
				// first get the hashed PW for this user
				string hashpw = this.UsersRepo.GetHashedPWByUserID( results.User.ID );

				// if using asym enc then have to decrypt the password first
				if ( decryptPW )
				{
					pw = AsymmetricEncryptor.Decrypt( pw, keySize, privateKey );
					this.Log( $"A login password was decrypted.", "Security", LogLevel.Info );
				}

				// check the PW is valid and matches whats in the DB
				if ( hashpw == null )
				{
					this.Log( $"No password for {results.User.ID}!", "Security", LogLevel.Error );
					results.Results = APIResultType.IncorrectPW;
					return results;
				}

				// now get the full user
				User user = this.UsersRepo.GetByUserID( results.User.ID );

				// no user
				if ( user == null )
				{
					results.Results = APIResultType.UserIDNotFound;
					return results;
				}

				// update the User info now
				// TODO - revert back to JUST returning the User ID
				results.User.ID = user.UserID;
				results.User.Name = user.DisplayName;

				// check the PW is valid
				if ( !new BCryptPasswordMaker().Compare( pw, hashpw ) )
				{
					this.Log( $"Incorrect password for {results.User.ID}!", "Security", LogLevel.Info );

					// set as failed attempt and return
					Task.Run( () =>
					{
						this.UsersRepo.InsertFailedLogin( user.ID );
					} );

					results.Results = APIResultType.IncorrectPW;
					return results;
				}

				// user is disabled or has too many failed logins
				if ( !user.Enabled )
				{
					results.Results = APIResultType.Disabled;
					return results;
				}

				// TODO - check for too many failed logins

				// user has no role in this domain
				if ( user.Roles.FindIndex( r => r.Domain.Equals( results.Domain, StringComparison.InvariantCultureIgnoreCase ) ) < 0 )
				{
					results.Results = APIResultType.InvalidDomain;
					return results;
				}

				// look for a session for this user
				List<Session> current = this.SessionRepo.GetByUser( user.ID, results.Domain );

				// do some side work
				Task.Run( () =>
				{
					bool updated = this.UsersRepo.UpdateLastLogin( user.ID );

					if ( current != null )
					{
						foreach ( Session s in current )
							this.SessionRepo.Delete( s.ID );
					}

					if ( user.FailedLogins?.Count > 0 )
						this.UsersRepo.ClearFailedLogins( user.ID );
				} );

				// insert new session only after the current ones are found
				Session ses = this.SessionRepo.Insert( user.ID, LowSalter(), results.Domain, ip );

				if ( ses == null )
				{
					this.Log( $"Failed to insert a login session for {user.UserID}.", "Security", LogLevel.Error );
					results.Results = APIResultType.Exception;
					return results;
				}

				// update the domain
				results.Domain = ses.Domain;

				// create a token - set the Mock flag iff the mock param is set
				ITokenManager tmgr = new SimpleTokenManager( this.Config.TokenPassword );
				results.Token = tmgr.Encode( new TokenPayload()
				{
					ID = ses.ID.ToString(),
					Salt = ses.Salt,
					//Hint = this.TokenHint,
					Domain = ses.Domain,
					Parameters = ( this._isMock ) ? (int)TokenParam.LoadMocks : 0
				} );

				// token creation error
				if ( results.Token == null )
				{
					this.Log( $"Failed to create token for {user.UserID}.", "Security", LogLevel.Error );
					results.Results = APIResultType.Exception;
					return results;
				}

				results.Results = APIResultType.Success;
				this.Log( $"{user.UserID} logged in to {results.Domain}.", "Security", LogLevel.Info );
			}
			catch ( System.Exception exp )
			{
				this.Log( exp );
				results.Results = APIResultType.Exception;
			}

			return results;
		}

		/// <summary>Determines if a session with the sepcified ID exists</summary>
		/// <param name="token">the token to check on</param>
		/// <returns>The refreshed session if it exists are validates otherwise null is returned.</returns>
		public Session RefreshSession( TokenPayload payload )
		{
			if ( payload == null || !payload.IsValid() )
				return null;

			Guid sid = Guid.Empty;
			if ( !Guid.TryParse( payload.ID, out sid ) )
				return null;

			// then find an active session with that user - user IDs must match
			User user = this.UsersRepo.GetBySessionID( sid );

			// no user with such session ID found
			if ( user == null )
				return null;

			// get the actual session from the DB
			Session ses = user.GetSession( sid );

			// no session, the user has been logged out
			if ( ses == null )
				return null;

			// delete any expired sessions within this domain
			Task.Run( () => {
				long deleted = this.DeleteExpiredSessions( ses.Domain );
			} );

			// compare the hint
			// TODO - Consider removing the hint property if domain and salt are in the token
			//if ( !payload.Hint.Equals( this.TokenHint, StringComparison.InvariantCulture ) )
			//return null;

			// compare the salt & domain
			if ( !payload.IsEqual( ses.Salt, ses.Domain ) )
				return null;

			// check to see if this session has already expired
			if ( DateTime.UtcNow >= ( ses.LastModified + new TimeSpan( 0, 0, this.Timeout ) ) )
			{
				Task.Run( () => this.SessionRepo.Delete( ses.ID ) );
				return null;
			}

			// set the current user in this gateway for later use by other methods
			this.CurrentUser = user;
			this.CurrentSession = ses;

			// update the session
			return this.SessionRepo.Refresh( ses.ID );
		}

		/// <summary>Explicitly remove the users session token</summary>
		public bool Logout( TokenPayload payload )
		{
			if ( payload == null || !payload.IsValid() )
				return false;

			// then find an active session with that user
			Session ses = this.SessionRepo.Get( payload.ID );

			// no session, the user has been logged out
			if ( ses == null )
				return false;

			// delete any expired sessions within this domain
			Task.Run( () => {
				long deleted = this.DeleteExpiredSessions( ses.Domain );
			} );

			// compare the salt & domain
			if ( !payload.IsEqual( ses.Salt, ses.Domain ) )
				return false;

			// now delete it
			return this.SessionRepo.Delete( ses.ID );
		}

		/// <summary>Removed any expired sessions in this domain</summary>
		public long DeleteExpiredSessions( string domain )
		{
			TimeSpan ts = new TimeSpan( 0, 0, this.Timeout );
			return this.SessionRepo.DeleteExpired( ts, domain );
		}

		#endregion [ Session Methods ]
	}
}
