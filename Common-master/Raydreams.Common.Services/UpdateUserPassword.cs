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
using Raydreams.Common.Services.Security;
using Raydreams.Common.Services.Interface;
using Raydreams.Common.Security;

namespace Raydreams.Common.Services
{
	/// <summary>Updates an users password either internally or externally depending on what is passed in the body</summary>
	public static class UpdateUserPassword
	{
		[FunctionName( "UpdatePassword" )]
		public static async Task<IActionResult> Run( [HttpTrigger( AuthorizationLevel.Function, "post", Route = null )] HttpRequest req, ILogger log )
		{
			log.LogInformation( "UpdateUserPassword function triggered." );

			string requestBody = await new StreamReader( req.Body ).ReadToEndAsync();
			dynamic data = JsonConvert.DeserializeObject( requestBody );

			// json body input
			string userID = data?.id; // userID is used with code
			string code = data?.rc; // a reset request confirmation code
			string from = data?.from; // original PW
			string to = data?.to; // new PW

			// must at least be a new password
			if ( String.IsNullOrWhiteSpace( to ) )
				return new BadRequestObjectResult( "Invalid input parameters." );

			// must have either code or from - so if both empty then return error
			if ( String.IsNullOrWhiteSpace( code ) && String.IsNullOrWhiteSpace( from ) )
				return new BadRequestObjectResult( "Invalid input parameters." );

			// get results
			APIResult<bool> results = new APIResult<bool>();

			try
			{

				// get the environment config
				EnvironmentSettings env = EnvironmentSettings.GetSettings( Environment.GetEnvironmentVariable( "env" ) );

				// test a connection to the backend
				if ( String.IsNullOrWhiteSpace( env.ConnectionString ) )
					throw new System.Exception( "No connection string." );

				// create a session manager
				SessionManager ses = new SessionManager( req, new SimpleTokenManager( env.TokenPassword ) );

				// load the default domain since PWs are the same across all domains
				ICommonGateway gate = new CommonGateway( env, CommonGateway.DefaultDomain );

				// external change only checks the timestamp
				if ( !String.IsNullOrWhiteSpace( code ) )
				{
					// validate the request and refresh the session
					if ( !ses.IsValidTimestamp( env.TimestampTimeout ) )
						return new UnauthorizedResult();

					// change by code
					results.ResultObject = gate.UpdatePasswordByCode( code, to );
				}
				else if ( !String.IsNullOrWhiteSpace( from ) ) // internal branch
				{
					// decode the token
					TokenPayload tok = ses.DoDecode();

					// setup the session manager callbacks
					gate.RefreshSession( tok );

					// validate the request and refresh the session
					if ( !ses.IsValidTimestamp( env.TimestampTimeout ) || gate.CurrentSession == null )
						return new UnauthorizedResult();

					results.ResultObject = gate.UpdatePassword( from, to );
				}

			}
			catch ( System.Exception exp )
			{
				log.LogError( exp.Message, null );
				return new StatusCodeResult( StatusCodes.Status500InternalServerError );
			}

			if ( results.ResultObject )
				results.ResultCode = APIResultType.Success;

			return new OkObjectResult( results );
		}

	}
}
