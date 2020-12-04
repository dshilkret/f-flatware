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
using Raydreams.Common.Security;

namespace Raydreams.Common.Services
{
  //  public static class GetUserSettings
  //  {
  //      [FunctionName("GetUserSettings")]
  //      public static IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
  //      {
		//	log.LogInformation("GetSettings function triggered.");

		//	APIResult<UserPreferences> results = new APIResult<UserPreferences>(APIResultType.NoResults);

		//	try
		//	{
		//		// get the environment config
		//		EnvironmentSettings env = Environments.Get( Environment.GetEnvironmentVariable( "env" ) );

		//		// test a connection to the backend
		//		if ( String.IsNullOrWhiteSpace( env.ConnectionString ) )
		//			throw new System.Exception( "No connection string." );

		//		// decode the token
		//		ITokenManager tmgr = new SimpleTokenManager( "password" );
		//		SessionManager ses = new SessionManager( req );
		//		TokenPayload tok = ses.DoDecode( tmgr.Decode );

		//		// load the correct gateway
		//		ICommonGateway gate = new CommonGateway( env.Key );
		//		if ( tok.HasFlag( TokenParam.LoadMocks ) )
		//			gate.LoadMocks();

		//		// setup the session manager callbacks
		//		Session session = gate.RefreshSession( tok );

		//		// validate the request and refresh the session
		//		if ( !ses.IsValidTimestamp( env.TimestampTimeout ) || session == null )
		//			return new UnauthorizedResult();

		//		results.ResultObject = gate.GetUserPreferences();
		//	}
		//	catch (System.Exception ex)
		//	{
		//		log.LogError(ex.ToLogMsg(true), null);
		//		return new StatusCodeResult(StatusCodes.Status500InternalServerError);
		//	}

		//	if (results.ResultObject != null)
		//		results.ResultCode = APIResultType.Success;

		//	return new OkObjectResult(results);
		//}

  //  }
}
