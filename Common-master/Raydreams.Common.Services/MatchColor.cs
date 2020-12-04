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
using System.Collections.Generic;
using Raydreams.Common.Logic;
using System.Drawing;
using Raydreams.Common.Services.Security;

namespace Raydreams.Common.Services
{
	/// <summary></summary>
    public static class MatchColor
    {
        [FunctionName( "MatchColor" )]
        public static async Task<IActionResult> Run([HttpTrigger( AuthorizationLevel.Anonymous, "get", "post", Route = null )] HttpRequest req, ILogger log )
        {
            log.LogInformation( "C# HTTP trigger function processed a request." );

            string line = req.Query["line"];
			string rgb = req.Query["rgb"];
			string hex = req.Query["hex"];

			string requestBody = await new StreamReader( req.Body ).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject( requestBody );
            line = line ?? data?.line;
			rgb = rgb ?? data?.rgb;
			hex = hex ?? data?.hex;

			// validate
			if ( String.IsNullOrWhiteSpace( line ) )
				return new BadRequestObjectResult( "Invalid input parameters." );

			// determine color space to use - RGB has priority
			Color c = Color.Black;

			if ( !String.IsNullOrWhiteSpace( rgb ) )
				c = ColorConverters.RGBStringToColor( rgb.Trim() );
			else if ( !String.IsNullOrWhiteSpace( hex ) )
				c = ColorConverters.HexToColor( hex.Trim() );

			APIResult<ProductLine> results = new APIResult<ProductLine>( APIResultType.Unknown );

			try
			{
				// get the environment config & setup the gateway
				EnvironmentSettings env = EnvironmentSettings.GetSettings( Environment.GetEnvironmentVariable( "env" ) );

				// test a connection to the backend
				if ( String.IsNullOrWhiteSpace( env.ConnectionString ) )
					throw new System.Exception( "No connection string." );

				// check the API token
				APIManager ses = new APIManager( req );

				// load the correct gateway
				ICommonGateway gate = new CommonGateway( env, CommonGateway.DefaultDomain );

				// validate the request and refresh the session
				if ( !ses.IsValidToken() )
					return new UnauthorizedResult();

				results.ResultObject = gate.MatchColor( line.Trim(), c );
			}
			catch ( System.Exception exp )
			{
				log.LogError( exp.Message );
				return new StatusCodeResult( StatusCodes.Status500InternalServerError );
			}

			if ( results.ResultObject != null )
				results.ResultCode = APIResultType.Success;

			return new OkObjectResult( results );
		}
    }
}
