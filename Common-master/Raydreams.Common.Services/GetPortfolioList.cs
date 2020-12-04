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
using System.Collections.Generic;
using Raydreams.Common.Security;

namespace Raydreams.Common.Services
{
  //  public static class GetPortfolioList
  //  {
  //      [FunctionName("GetPortfolioList")]
  //      public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,ILogger log )
  //      {
		//	log.LogInformation("GetPortfolioList function triggered.");

		//	string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
		//	dynamic data = JsonConvert.DeserializeObject(requestBody);

		//	APIResult<List<LookupPair>> results = new APIResult<List<LookupPair>>(APIResultType.NoResults);

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


		//		results.ResultObject = gate.GetPortfolioList();
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
