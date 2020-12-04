using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Raydreams.Common.Services.Security
{
	/// <summary>Used to manage a API client with its own key to use the API</summary>
    public class APIManager
    {
		#region [ Fields ]

		/// <summary>The default Authorization header value to use</summary>
		public static readonly string DefaultAPIAuthHeader = "x-api-authorization";

		/// <summary>The default timestamp header value to use</summary>
		public static readonly string DefaultTimestampHeader = "x-timestamp";

		/// <summary></summary>
		private string _authHeader = DefaultAPIAuthHeader;

		/// <summary></summary>
		private string _tsHeader = DefaultTimestampHeader;

		/// <summary></summary>
		private DateTimeOffset _clientTS = DateTimeOffset.MinValue;

		#endregion [ Fields ]

		#region [ Constructors ]

		/// <summary>Create with defaults</summary>
		public APIManager( HttpRequest req ) : this( req, DefaultAPIAuthHeader, DefaultTimestampHeader )
		{
		}

		/// <summary></summary>
		public APIManager( HttpRequest req, string authHeader, string tsHeader )
		{
			this.Request = req;
			this.AuthHeader = authHeader;
			this.TimestampHeader = tsHeader;
		}

		#endregion [ Constructors ]

		#region [ Properties ]

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
					return DefaultAPIAuthHeader;

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

		#endregion [ Properties ]

		#region [ Methods  ]

		/// <summary>Client assigned token must match</summary>
        /// <returns></returns>
		public bool IsValidToken()
        {
			// get the token from the header
			StringValues tokens = this.Request.Headers[this.AuthHeader];

			if ( tokens.Count < 1 || String.IsNullOrWhiteSpace(tokens[0]) )
				return false;

			string token = tokens[0].Trim();

			return token.Equals("gx83sfhvf0", StringComparison.InvariantCulture);
		}

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

		#endregion [ Methods ]
	}
}
