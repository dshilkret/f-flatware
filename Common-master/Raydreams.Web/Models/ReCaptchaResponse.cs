using System;
using Newtonsoft.Json;

namespace Raydreams.Web.Models
{
	/// <summary></summary>
	public class ReCaptchaResponse
	{
		public ReCaptchaResponse()
		{
			this.Hostname = String.Empty;
			this.ErrorCodes = new string[0];
		}

		[JsonProperty( PropertyName = "success" )]
		public bool Success { get; set; }

		[JsonProperty( PropertyName = "challenge_ts" )]
		public DateTime ChallengeTS { get; set; }

		[JsonProperty( PropertyName = "hostname" )]
		public string Hostname { get; set; }

		[JsonProperty( PropertyName = "error-codes" )]
		public string[] ErrorCodes { get; set; }
	}
}
