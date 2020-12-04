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
using Raydreams.Common.Services.Model;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Raydreams.Common.Services
{
	/// <summary>Creates a user ID CHAR PIN based on the users name with a default length of 5</summary>
    public static class GetCharPin
    {
        [FunctionName( "GetCharPin" )]
        public static async Task<IActionResult> Run([HttpTrigger( AuthorizationLevel.Function, "post", Route = null )] HttpRequest req,ILogger log)
        {
			log.LogInformation( "GetCharPin function triggered." );

			string requestBody = await new StreamReader( req.Body ).ReadToEndAsync();
			dynamic data = JsonConvert.DeserializeObject( requestBody );
			string fname = data?.fname;
			string lname = data?.lname;
			string mname = data?.mname;

			APIResult<string> results = new APIResult<string>( APIResultType.NoResults );

			try
			{
				// get the environment config & setup the gateway
				EnvironmentSettings env = EnvironmentSettings.GetSettings( Environment.GetEnvironmentVariable( "env" ) );
				ICommonGateway gate = new CommonGateway( env, null );

				if (String.IsNullOrWhiteSpace( env.ConnectionString ))
					throw new System.Exception( "No connection string." );

				// check for the auth token
				//SessionManager ses = new SessionManager( req ) { OnRefresh = gate.RefreshSession };
				//if (!ses.IsValidTimestamp( env.TimestampTimeout ) || !ses.AuthRefresh())
					//return new UnauthorizedResult();

				// extract the user ID

				results.ResultObject = gate.CreateCharPIN( fname, lname, mname );
			}
			catch (System.Exception ex)
			{
				log.LogError( ex.ToLogMsg( true ), null );
				return new StatusCodeResult( StatusCodes.Status500InternalServerError );
			}

			if (results.ResultObject != null)
				results.ResultCode = APIResultType.Success;
			else
				results.ResultCode = APIResultType.Exception;

			return new OkObjectResult( results );
		}
    }
}
