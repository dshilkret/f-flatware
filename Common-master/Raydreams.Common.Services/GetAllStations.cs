using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Raydreams.Common.Services.Model;
using System.Collections.Generic;

namespace Raydreams.Common.Services
{
    public static class GetAllStations
    {
        [FunctionName( "GetAllStations" )]
        public static async Task<IActionResult> Run( [HttpTrigger( AuthorizationLevel.Anonymous, "get", "post", Route = null )] HttpRequest req, ILogger log )
        {
            log.LogInformation( "GetAllStations function triggered." );

            string connStr = @"DefaultEndpointsProtocol=https;AccountName=raydreamsdevsa002;AccountKey=VHRzONoual1tKzfOUEqhDHmPzBb/GUZ9uyLDv9kj698JcQAkIELDx6K/LtDXOIrUpmeJIrWECj493eyOpSCldQ==;EndpointSuffix=core.windows.net";

            CloudStorageAccount account = CloudStorageAccount.Parse( connStr );
            CloudTableClient serviceClient = account.CreateCloudTableClient();
            CloudTable tbl = serviceClient.GetTableReference( "GasStations" );

            TableQuerySegment<GasStation> results = null;
            TableContinuationToken tok = new TableContinuationToken();

            APIResult<List<GasStation>> r = new APIResult<List<GasStation>>();

            try
            {
                //TableQuery query = new TableQuery();
                results = await tbl.ExecuteQuerySegmentedAsync<GasStation>( new TableQuery<GasStation>(), tok );

                if ( results != null )
                {
                    r.ResultCode = APIResultType.Success;
                    r.ResultObject = results.Results;
                }
            }
            catch ( System.Exception exp )
            {
                log.LogInformation( exp.Message );
                return new StatusCodeResult( StatusCodes.Status500InternalServerError );
            }

            return new OkObjectResult( r );
        }
    }
}
