using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Raydreams.Common.Services.Model;
using Raydreams.Common.Services.Config;
using Raydreams.Common.Services.Interface;
using Raydreams.Common.Services.Security;
using Raydreams.Common.Extensions;
using Raydreams.Common.Security;

namespace Raydreams.Common.Services
{
	/// <summary>When explicitly called, will refresh the session for the specified token. This can be called by the app or dashboard.</summary>
	/// <remarks>Most times another API call will call refresh session, but this will do it explicitly
	/// Token field is not populated since client already has it
	/// </remarks>
	public static class RefreshSession
	{
		[FunctionName( "Refresh" )]
		public static IActionResult Run( [HttpTrigger( AuthorizationLevel.Function, "get", "post", Route = null )] HttpRequest req, ILogger log )
		{
			log.LogInformation( "RefreshSession function triggered." );

			Session session = null;

			try
			{
				// get the environment config
				EnvironmentSettings env = EnvironmentSettings.GetSettings( Environment.GetEnvironmentVariable( "env" ) );

				// test a connection to the backend
				if ( String.IsNullOrWhiteSpace( env.ConnectionString ) )
					throw new System.Exception( "No connection string." );

				// decode the token
				SessionManager ses = new SessionManager( req, new SimpleTokenManager( env.TokenPassword ) );
				TokenPayload tok = ses.DoDecode();

				// load the correct gateway
				ICommonGateway gate = new CommonGateway( env, tok.Domain );

				// validate the request and refresh the session
				if ( !ses.IsValidTimestamp( env.TimestampTimeout ) )
					return new UnauthorizedResult();

				// refresh the token if it exists
				session = gate.RefreshSession( tok );
			}
			catch ( System.Exception exp )
			{
				log.LogError( exp.Message );
				return new StatusCodeResult( StatusCodes.Status500InternalServerError );
			}

			APIResult<bool> results = new APIResult<bool>()
			{
				ResultObject = ( session != null ),
				ResultCode = ( session != null ) ? APIResultType.Success : APIResultType.Unauthorized
			};

			return new OkObjectResult( results );
		}
	}
}
