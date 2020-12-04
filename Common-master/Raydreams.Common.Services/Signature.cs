using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Raydreams.Common.Extensions;
using Raydreams.Common.Services.Config;
using Raydreams.Common.Services.Interface;
using Raydreams.Common.Services.Security;
using Raydreams.Common.Security;
using Raydreams.Common.Services.Model;

namespace Raydreams.Common.Services
{
	/// <summary>Echos an ACK signature back to the caller</summary>
	public static class Signature
	{
		[FunctionName( "Signature" )]
		public static IActionResult Run([HttpTrigger( AuthorizationLevel.Function, "get", Route = null )] HttpRequest req, ILogger log )
		{
			log.LogInformation( "Signature function triggered." );

			string echo = req.Query["echo"];

			echo = (String.IsNullOrWhiteSpace(echo)) ? String.Empty : echo.Truncate(128, true);

			string sig = String.Empty;

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

				// setup the session manager callbacks
				gate.RefreshSession( tok );

				// validate the request and refresh the session
				if ( !ses.IsValidTimestamp( env.TimestampTimeout ) || gate.CurrentSession == null )
					return new UnauthorizedResult();

				sig = String.Format( "{0};UserID : {1}; Token : {2}", gate.Signature(), ses.CurrentSession.UserID, ses.CurrentToken );
			}
			catch ( System.Exception ex )
			{
				log.LogError( ex.ToLogMsg( true ), null );
				return new StatusCodeResult( StatusCodes.Status500InternalServerError );
			}

			return new OkObjectResult( sig );
		}
	}
}
