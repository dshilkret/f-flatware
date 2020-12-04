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
using Raydreams.Common.Services.Data;
using Raydreams.Common.Services.Config;
using Raydreams.Common.Services.Security;
using System.Collections.Generic;
using Raydreams.Common.Services.Interface;

namespace Raydreams.Common.Services
{
	/// <summary>Gets a specific content page</summary>
	public static class GetContent
	{
		[FunctionName( "GetContent" )]
		public static async Task<IActionResult> Run( [HttpTrigger( AuthorizationLevel.Function, "post", Route = null )] HttpRequest req, ILogger log )
		{
			log.LogInformation( "GetContent function triggered." );

			//if ( req.Method == "get" )
			string requestBody = await new StreamReader( req.Body ).ReadToEndAsync();
			dynamic data = JsonConvert.DeserializeObject( requestBody );
			string site = data?.site; // site domain
			string path = data?.path; // path
			string lang = data?.lang; // language
			string id = data?.id; // DB ID
			string type = data?.type; // content type is optional but only used when returning the full list

			// validate input
			if ( String.IsNullOrWhiteSpace( site ) )
				return new BadRequestObjectResult( "Invalid input parameters." );

			if ( String.IsNullOrWhiteSpace( lang ) )
				lang = "en";

			Guid dbid = Guid.Empty;
			bool byid = Guid.TryParse( id, out dbid );

			try
			{
				// get the environment config & setup the gateway
				EnvironmentSettings env = EnvironmentSettings.GetSettings( Environment.GetEnvironmentVariable( "env" ) );

				// test a connection to the backend
				if ( String.IsNullOrWhiteSpace( env.ConnectionString ) )
					throw new System.Exception( "No connection string." );

				// decode the token
				SessionManager ses = new SessionManager( req, null );

				// load the correct gateway
				ICommonGateway gate = new CommonGateway( env, site );

				// validate the request and refresh the session
				if ( !ses.IsValidTimestamp( env.TimestampTimeout ) )
					return new UnauthorizedResult();

				// if an actual ID was sent - do first
				if ( byid )
				{
					APIResult<ContentPage> results = new APIResult<ContentPage>() { ResultCode = APIResultType.NoResults };
					results.ResultObject = gate.GetContentByID( site, dbid, lang );
					if ( results.ResultObject != null )
						results.ResultCode = APIResultType.Success;

					return new OkObjectResult( results );
				}
				// next by path
				else if ( !String.IsNullOrWhiteSpace( path ) )
				{
					APIResult<ContentPage> results = new APIResult<ContentPage>() { ResultCode = APIResultType.NoResults };
					results.ResultObject = gate.GetContentByPath( site, path, lang );
					if ( results.ResultObject != null )
						results.ResultCode = APIResultType.Success;

					return new OkObjectResult( results );
				}
				else // finally just get the listing if no path or DB ID
				{
					APIResult<List<ContentHeader>> results = new APIResult<List<ContentHeader>>() { ResultCode = APIResultType.NoResults };
					results.ResultObject = gate.GetContentList( site, type );
					if ( results.ResultObject != null )
						results.ResultCode = APIResultType.Success;

					return new OkObjectResult( results );
				}

			}
			catch ( System.Exception exp )
			{
				log.LogError( exp.Message, null );
				return new StatusCodeResult( StatusCodes.Status500InternalServerError );
			}

		}

	}
}
