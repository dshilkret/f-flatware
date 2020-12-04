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
using Raydreams.Common.Services.Data;
using Raydreams.Common.Services.Security;
using Raydreams.Common.Security;
using Raydreams.Common.Services.Interface;

namespace Raydreams.Common.Services
{
	/// <summary></summary>
	/// <remarks>Not done yet</remarks>
	public static class InsertContentPage
	{
		[FunctionName( "InsertContentPage" )]
		public static async Task<IActionResult> Run( [HttpTrigger( AuthorizationLevel.Function, "post", Route = null )] HttpRequest req, ILogger log )
		{
			log.LogInformation( "InsertContentPage function triggered." );

			string requestBody = await new StreamReader( req.Body ).ReadToEndAsync();
			ContentPage data = JsonConvert.DeserializeObject<ContentPage>( requestBody );

			// validate input
			if ( data == null )
				return new BadRequestObjectResult( "Invalid input parameters." );

			APIResult<bool> results = new APIResult<bool>() { ResultCode = APIResultType.Unknown };

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

				PagesRepository repo = new PagesRepository( env.ConnectionString, "texcorp", "Pages" );

				results.ResultObject = repo.Insert( data );
			}
			catch ( System.Exception exp )
			{
				log.LogError( exp.Message, null );
				return new StatusCodeResult( StatusCodes.Status500InternalServerError );
			}

			return new OkObjectResult( results );
		}
	}
}
