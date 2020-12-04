using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Raydreams.Common.Services.Model
{
	/// <summary>What is sent back to the client after a successful login</summary>
	public class LoginResponse
    {
		public LoginResponse()
		{
			this.User = new UserInfo();
			this.Results = APIResultType.Unknown;
		}

		/// <summary>The Users info</summary>
		[JsonProperty( "user" )]
		public UserInfo User { get; set; }

		/// <summary>The domain this session is for</summary>
		[JsonProperty( "domain" )]
		public string Domain { get; set; }

		/// <summary>The session security token</summary>
		[JsonProperty( "token" )]
		public string Token { get; set; }

		/// <summary>What happened when trying to log in</summary>
		[JsonProperty( "results" )]
		[JsonConverter( typeof( StringEnumConverter ) )]
		public APIResultType Results { get; set; }

		/// <summary>Only used for debugging</summary>
		//[JsonProperty( "debug" )]
		//public string Debug { get; set; }
	}
}
