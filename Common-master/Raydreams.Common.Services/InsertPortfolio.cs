using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Raydreams.Common.Services.Config;
using Raydreams.Common.Services.Interface;
using Raydreams.Common.Services.Security;
using Raydreams.Common.Services.Model;
using Raydreams.Common.Extensions;
using Raydreams.Common.Security;

namespace Raydreams.Common.Services
{
  //  public static class InsertPortfolio
  //  {
  //      [FunctionName("InsertPortfolio")]
  //      public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log )
  //      {
		//	log.LogInformation("InsertPortfolio function triggered.");

		//	string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
		//	dynamic data = JsonConvert.DeserializeObject(requestBody);

		//	string name = data?.name;

		//	if ( String.IsNullOrWhiteSpace(name) )
		//		return new BadRequestObjectResult("Invalid input parameters.");

		//	APIResult<bool> results = new APIResult<bool>(APIResultType.NoResults);

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

		//		// insert
		//		results.ResultObject = gate.InsertPortfolio(name);
		//	}
		//	catch (System.Exception ex)
		//	{
		//		log.LogError(ex.ToLogMsg(true), null);
		//		return new StatusCodeResult(StatusCodes.Status500InternalServerError);
		//	}

		//	results.ResultCode = APIResultType.Success;

		//	return new OkObjectResult(results);
		//}
  //  }
}
