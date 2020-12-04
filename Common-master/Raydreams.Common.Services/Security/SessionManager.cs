using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Raydreams.Common.Security;
using Raydreams.Common.Services.Model;

namespace Raydreams.Common.Services.Security
{
	/// <summary>Handles low level session validation and authetication in all the API calls.</summary>
	public class SessionManager
	{
		#region [ Fields ]

		/// <summary>The default Authorization header value to use</summary>
		//public static readonly string DefaultAuthHeader = "Authorization";
		public static readonly string DefaultAuthHeader = "x-token";

		/// <summary>The default timestamp header value to use</summary>
		public static readonly string DefaultTimestampHeader = "x-timestamp";

		/// <summary>Delegate to use to refresh a token for the user</summary>
		public delegate Session RefreshSession( string token );

		/// <summary></summary>
		private string _authHeader = DefaultAuthHeader;

		/// <summary></summary>
		private string _tsHeader = DefaultTimestampHeader;

		/// <summary></summary>
		private DateTimeOffset _clientTS = DateTimeOffset.MinValue;

		/// <summary>Retrieved session</summary>
		private ITokenManager TokenManager = null;

		/// <summary>Retrieved session</summary>
		private Session _ses = null;

		#endregion [ Fields ]

		#region [ Constructors ]

		/// <summary>Create with defaults</summary>
		public SessionManager( HttpRequest req, ITokenManager mgr ) : this( req, mgr, DefaultAuthHeader, DefaultTimestampHeader )
		{
		}

		/// <summary></summary>
		public SessionManager( HttpRequest req, ITokenManager mgr, string authHeader, string tsHeader )
		{
			this.Request = req;
			this.TokenManager = mgr;

			this.AuthHeader = authHeader;
			this.TimestampHeader = tsHeader;
		}

		#endregion [ Constructors ]

		#region [ Properties ]

		/// <summary>The handler used to refresh the token</summary>
		//public RefreshSession OnRefresh { get; set; }

		/// <summary>Decodes the token with some input method</summary>
		public TokenPayload DoDecode()
		{
			if ( this.TokenManager == null )
				return null;

			TokenPayload payload = this.TokenManager.Decode( this.CurrentToken );

			return payload;
		}

		/// <summary>The current HTTP Request stored for later retrieveal</summary>
		public HttpRequest Request { get; protected set; }

		/// <summary>Gets the IP Address of the incoming address</summary>
		public string RequestIP
		{
			get
			{
				if ( this.Request != null )
					return this.Request.HttpContext.Connection.RemoteIpAddress.ToString();

				return "0.0.0.0";
			}
		}

		/// <summary>The current session set after Refresh</summary>
		public Session CurrentSession
		{
			get
			{
				if ( this._ses == null )
					return new Session() { Created = DateTime.UtcNow, UserID = Guid.Empty };

				return this._ses;
			}
			protected set
			{
				this._ses = value;
			}
		}

		/// <summary>The timestamp sent from the client</summary>
		public DateTimeOffset ClientTimestamp
		{
			get
			{
				return this._clientTS;
			}
			protected set
			{
				this._clientTS = value;
			}
		}

		/// <summary>What header field to look for the token in</summary>
		public string AuthHeader
		{
			get
			{
				if ( String.IsNullOrWhiteSpace( this._authHeader ) )
					return DefaultAuthHeader;

				return this._authHeader;
			}
			set
			{
				if ( !String.IsNullOrWhiteSpace( value ) )
					this._authHeader = value.Trim();
			}
		}

		/// <summary>What header field to look for the token in</summary>
		public string TimestampHeader
		{
			get
			{
				if ( String.IsNullOrWhiteSpace( this._tsHeader ) )
					return DefaultTimestampHeader;

				return this._tsHeader;
			}
			set
			{
				if ( !String.IsNullOrWhiteSpace( value ) )
					this._tsHeader = value.Trim();
			}
		}

		/// <summary>Just extract the current auth token from the header</summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public string CurrentToken
		{
			get
			{
				StringValues tokens = this.Request.Headers[this.AuthHeader];
				return ( tokens.Count > 0 ) ? tokens[0] : String.Empty;
			}
		}

		#endregion [ Properties ]

		#region [ Methods  ]

		/// <summary>Checks the timestamp header is in range. Any input less than 1 will make it always return true.</summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public bool IsValidTimestamp( int min = 1 )
		{
			if ( min < 1 )
				return true;

			// get the token from the header
			StringValues tokens = this.Request.Headers[this.TimestampHeader];

			if ( tokens.Count < 1 )
				return false;

			// parse the ts
			DateTimeOffset ts;
			if ( !DateTimeOffset.TryParse( tokens[0], out ts ) )
				return false;

			// set this as a property to hold on to
			this.ClientTimestamp = ts;

			// if the timestamp is too far in the future
			if ( ts > DateTimeOffset.UtcNow.AddMinutes( 1 ) )
				return false;

			// if the current time is within N minutes of the request
			if ( DateTimeOffset.UtcNow < ts.AddMinutes( min ) )
				return true;

			return false;
		}

		/// <summary>Is it postman calling?</summary>
		public static bool IsPostman( HttpRequest req )
		{
			StringValues tokens = req.Headers["User-Agent"];

			return ( tokens.Count > 0 && tokens.Contains( "PostmanRuntime" ) );
		}

		#endregion [ Methods ]
	}

}