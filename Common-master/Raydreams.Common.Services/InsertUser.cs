using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Raydreams.Common.Extensions;
using Raydreams.Common.Services.Config;
using Raydreams.Common.Services.Interface;
using Raydreams.Common.Services.Security;
using Raydreams.Common.Security;
using Raydreams.Common.Services.Model;

namespace Raydreams.Common.Services
{
	/// <summary>Adds a new dashboard user to the instance</summary>
	public static class InsertUser
	{
		[FunctionName( "InsertUser" )]
		public static async Task<IActionResult> Run( [HttpTrigger( AuthorizationLevel.Function, "post", Route = null )] HttpRequest req, ILogger log )
		{
			log.LogInformation( "InsertUser function triggered." );

			string requestBody = await new StreamReader( req.Body ).ReadToEndAsync();
			dynamic data = JsonConvert.DeserializeObject( requestBody );

			// initial data
			string userID = data?.userID; // user ID
			string name = data?.name; // display name
			string email = data?.email; // email to use
			string domain = data?.d; // what domain to add to
			string role = data?.role; // role in domain

			// validate all the input
			if ( String.IsNullOrWhiteSpace( userID ) || String.IsNullOrWhiteSpace( name ) )
				return new BadRequestObjectResult( "Invalid input parameters." );

			if ( String.IsNullOrWhiteSpace( email ) || String.IsNullOrWhiteSpace( domain ) )
				return new BadRequestObjectResult( "Invalid input parameters." );

			APIResult<(string UserID, string PW)> results = new APIResult<(string UserID, string PW)>() { ResultCode = APIResultType.Unknown };

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

				// load the correct gateway using the master domain
				ICommonGateway gate = new CommonGateway( env, CommonGateway.DefaultDomain );

				// setup the session manager callbacks
				gate.RefreshSession( tok );

				// validate the request and refresh the session
				if ( !ses.IsValidTimestamp( env.TimestampTimeout ) || gate.CurrentSession == null )
					return new UnauthorizedResult();

				// insert the user
				results.ResultObject = gate.InsertUser( userID, name, email, domain, role );
			}
			catch ( System.Exception exp )
			{
				log.LogError( exp.Message, null );
				return new StatusCodeResult( StatusCodes.Status500InternalServerError );
			}

			if ( !String.IsNullOrWhiteSpace( results.ResultObject.PW ) )
				results.ResultCode = APIResultType.Success;

			return new OkObjectResult( results );
		}
	}
}
