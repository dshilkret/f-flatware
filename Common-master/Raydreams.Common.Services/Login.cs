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

namespace Raydreams.Common.Services
{
    /// <summary>Login function</summary>
    public static class Login
    {
        [FunctionName( "Login" )]
        public static async Task<IActionResult> Run( [HttpTrigger( AuthorizationLevel.Function, "post", Route = null )] HttpRequest req, ILogger log )
        {
            log.LogInformation( "Login function triggered." );

            string requestBody = await new StreamReader( req.Body ).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject( requestBody );

            string name = data?.id;
            string pw = data?.pw;
            string domain = data?.d;

            // validate
            if ( String.IsNullOrWhiteSpace( name ) || String.IsNullOrWhiteSpace( pw ) )
                return new BadRequestObjectResult( "Invalid input parameters." );

            if ( String.IsNullOrWhiteSpace( domain ) )
                domain = EnvironmentSettings.DefaultDomain;

            // setup the response
            LoginResponse results = null;

            try
            {
                // no token to decode on login but is it a mock account?
                SessionManager sesMgr = new SessionManager( req, null );

                // if a mock situation - change to mock env

                // get the environment config & setup the gateway
                EnvironmentSettings env = EnvironmentSettings.GetSettings( Environment.GetEnvironmentVariable( "env" ) );

                // test for a connection to the backend
                if ( String.IsNullOrWhiteSpace( env.ConnectionString ) )
                    throw new System.Exception( "No connection string." );

                // load the correct gateway
                ICommonGateway gate = new CommonGateway( env, domain );

                // check the timestamp on the request to stop replays to login
                if ( !sesMgr.IsValidTimestamp( env.TimestampTimeout ) )
                {
                    log.LogInformation( "Expired timestamp!" );
                    return new UnauthorizedResult();
                }

                // set last param to true/false if the PW is encrypted with an asym key
                results = gate.Login( name, pw, domain.Trim(), sesMgr.RequestIP, false );
            }
            catch ( System.Exception exp )
            {
                log.LogError( exp.Message );
                return new StatusCodeResult( StatusCodes.Status500InternalServerError );
            }

            if ( results == null )
                return new UnauthorizedResult();

            // some results may need to be modified

            return new OkObjectResult( results );
        }
    }
}
