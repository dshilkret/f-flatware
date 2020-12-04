using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Raydreams.Common.Services.Model
{
	/// <summary>Code that indicates the sucess or failture of an API call.</summary>
	public enum APIResultType
	{
		/// <summary>Normal, active state. Carry on.</summary>
		Success = 0,
		/// <summary>First time the user has ever logged in so they may need to set their UserID and Password.</summary>
		FirstLogin = 1,
		/// <summary>The user has to reset their password before they do anything.</summary>
		ResetPassword = 2,
		/// <summary>The user's account is disabled either by an admin or because of too many failed login attempts</summary>
		Disabled = 3,
		/// <summary>The user ID is already taken</summary>
		UserIDTaken = 4,
		/// <summary>Invalid PW format</summary>
		InvalidPWFormat = 5,
		/// <summary>Invalid User ID format</summary>
		InvalidUserIDFormat = 6,
		/// <summary>The app is offline. No one can login.</summary>
		Offline = 7,
		/// <summary>The credentials are invalid or the account is terminated. Return an unauthorzied.</summary>
		InvalidCredentials = 10,
		/// <summary>Password does not match the one in the DB</summary>
		IncorrectPW = 11,
		/// <summary>There's no matching UserID in the DB</summary>
		UserIDNotFound = 12,
		/// <summary>The user has no role in this domain</summary>
		InvalidDomain = 13,
		/// <summary>The input parameters do not validate</summary>
		InvalidInput = 21,
		/// <summary>No results data was obtained to return</summary>
		NoResults = 22,
		/// <summary>Failed a logic rule, see the logs or additional data</summary>
		FailedRule = 23,
		/// <summary>The user role is not authorized to access this method.</summary>
		Unauthorized = 31,
		/// <summary>Backend server is down</summary>
		Down = 98,
		/// <summary>An exception was thrown, see the logs</summary>
		Exception = 99,
		/// <summary>Error unknown, check the logs</summary>
		Unknown = 100
	}

}
