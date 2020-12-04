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
using System.Collections.Generic;

namespace Raydreams.Common.Services
{
    public static class GetProductLines
    {
        [FunctionName( "ProductLines" )]
        public static IActionResult Run([HttpTrigger( AuthorizationLevel.Anonymous, "get", Route = null )] HttpRequest req,ILogger log )
		{
			log.LogInformation( "GetProductLines function triggered." );

			APIResult<List<ProductLine>> results = new APIResult<List<ProductLine>>( APIResultType.NoResults );

			try
			{
				// get the environment config & setup the gateway
				EnvironmentSettings env = EnvironmentSettings.GetSettings( Environment.GetEnvironmentVariable( "env" ) );

				// test a connection to the backend
				if ( String.IsNullOrWhiteSpace( env.ConnectionString ) )
					throw new System.Exception( "No connection string." );

				// decode the token
				//SessionManager ses = new SessionManager( req, null );

				// load the correct gateway
				ICommonGateway gate = new CommonGateway( env, CommonGateway.DefaultDomain );

				// validate the request and refresh the session
				//if ( !ses.IsValidTimestamp( env.TimestampTimeout ) )
					//return new UnauthorizedResult();

				results.ResultObject = gate.GetProductLines();
			}
			catch ( System.Exception ex )
			{
				log.LogError( ex.Message, null );
				return new StatusCodeResult( StatusCodes.Status500InternalServerError );
			}

			if ( results.ResultObject != null )
				results.ResultCode = APIResultType.Success;

			return new OkObjectResult( results );
		}
	}
}
