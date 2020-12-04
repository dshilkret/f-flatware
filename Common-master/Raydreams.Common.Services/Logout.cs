using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Raydreams.Common.Extensions;
using Raydreams.Common.Services.Config;
using Raydreams.Common.Services.Interface;
using Raydreams.Common.Services.Security;
using Raydreams.Common.Services.Model;
using Raydreams.Common.Security;

namespace MARS.Services.Admin
{
	/// <summary>The API call to explicitly log out</summary>
	/// <remarks>Since they are loggin out we dont want to refreh the token. Will delete ALL expired sessions.</remarks>
	public static class Logout
	{
		[FunctionName( "Logout" )]
		public static IActionResult Run( [HttpTrigger( AuthorizationLevel.Function, "get", "post", Route = null )] HttpRequest req, ILogger log )
		{
			log.LogInformation( "Logout function triggered." );

			APIResult<bool> results = new APIResult<bool>( APIResultType.NoResults );

			try
			{
				// get the environment config & setup the gateway
				EnvironmentSettings env = EnvironmentSettings.GetSettings( Environment.GetEnvironmentVariable( "env" ) );

				if ( String.IsNullOrWhiteSpace( env.ConnectionString ) )
					throw new System.Exception( "No connection string." );

				//KeyVaultRepository kv = new KeyVaultRepository( this.Config.AppClientID, this.Config.AppClientPW );
				SessionManager sesMgr = new SessionManager( req, new SimpleTokenManager( env.TokenPassword ) );
				TokenPayload tok = sesMgr.DoDecode();

				// load the correct gateway
				ICommonGateway gate = new CommonGateway( env, tok.Domain );

				if ( !sesMgr.IsValidTimestamp( env.TimestampTimeout ) || String.IsNullOrWhiteSpace( sesMgr.CurrentToken ) )
				{
					log.LogInformation( $"Logout invalid timestamp or token {sesMgr.CurrentToken}!" );
					return new UnauthorizedResult();
				}

				// refresh the token if it exists
				results.ResultObject = gate.Logout( tok );
			}
			catch ( System.Exception exp )
			{
				log.LogError( exp.Message );
				return new StatusCodeResult( StatusCodes.Status500InternalServerError );
			}

			results.ResultCode = APIResultType.Success;

			return new OkObjectResult( results );
		}
	}
}
