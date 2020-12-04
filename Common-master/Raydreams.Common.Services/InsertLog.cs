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
using Raydreams.Common.Services.Model;
using Raydreams.Common.Security;

namespace MARS.Services.Web
{
    public static class InsertLog
    {
        // Request values
        // Method=POST;Path=/api/Log;Query=;Scheme=https;Host=common-dev-v1-api.azurewebsites.net
        // for clients IP = req.HttpContext.Connection.RemoteIpAddress

        /// <summary></summary>
        [FunctionName( "Log" )]
        public static async Task<IActionResult> Run( [HttpTrigger( AuthorizationLevel.Function, "post", Route = null )] HttpRequest req, ILogger log )
        {
            log.LogInformation( "InsertLog function triggered." );

            string requestBody = await new StreamReader( req.Body ).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject( requestBody );

            string msg = data?.msg; // message to log
            string lvl = data?.lvl; // log level
            string domain = data?.d; // domain to log to

            if ( String.IsNullOrWhiteSpace( msg ) )
                return new BadRequestObjectResult( "Invalid input parameters." );

            // always need a domain
            if ( String.IsNullOrWhiteSpace( domain ) )
                domain = CommonGateway.DefaultDomain;

            APIResult<bool> results = new APIResult<bool>( APIResultType.NoResults );

            try
            {
                // get the environment config
                EnvironmentSettings env = EnvironmentSettings.GetSettings( Environment.GetEnvironmentVariable( "env" ) );

                // test a connection to the backend
                if ( String.IsNullOrWhiteSpace( env.ConnectionString ) )
                    throw new System.Exception( "No connection string." );

                // decode the token
                SessionManager ses = new SessionManager( req, null );
                //TokenPayload tok = ses.DoDecode();

                // load the correct gateway
                ICommonGateway gate = new CommonGateway( env, domain );

                // setup the session manager callbacks
                //gate.RefreshSession( tok );

                // validate the request and refresh the session
                if ( !ses.IsValidTimestamp( env.TimestampTimeout ) )
                    return new UnauthorizedResult();

                gate.Log( msg, "Category", lvl );
                results.ResultCode = APIResultType.Success;
                results.ResultObject = true;
            }
            catch ( System.Exception ex )
            {
                log.LogError( ex.Message );
                return new StatusCodeResult( StatusCodes.Status500InternalServerError );
            }

            return new OkObjectResult( results );
        }
    }
}
