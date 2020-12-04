using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Linq;
using Raydreams.Common.Services.Config;
using Raydreams.Common.Services.Interface;
using Newtonsoft.Json;
using Raydreams.Common.Extensions;
using Raydreams.Common.Services.Model;
using System.Text.RegularExpressions;

namespace Raydreams.Common.Services
{
	/// <summary>Parses OCR TXT into structured data</summary>
	public static class InsertReceipt
	{
		[FunctionName( "InsertReceipt" )]
		public static async Task<IActionResult> Run([HttpTrigger( AuthorizationLevel.Anonymous, "post", Route = null )] HttpRequest req, ILogger log )
		{
			log.LogInformation( "InsertReceipt function triggered." );

			// get the data
			string requestBody = await new StreamReader( req.Body ).ReadToEndAsync();
			string body = requestBody.Trim();

			GasTransaction trans = null;

			try
			{
				// get the environment config & setup the gateway
				EnvironmentSettings env = EnvironmentSettings.GetSettings( Environment.GetEnvironmentVariable( "env" ) );
				ICommonGateway gate = new CommonGateway( env, null );

				if ( String.IsNullOrWhiteSpace( env.ConnectionString ) )
					throw new System.Exception( "No connection string." );

				// make sure we have data
				if ( body.Length < 1 )
				{
					string msg = "Triggered file has no data.";
					//gate.Log( msg, "Import", "Error" );
					return new BadRequestObjectResult( new { Error = msg } );
				}

				trans = ParseTransaction( body, ObjectId.Empty, gate );

			}
			catch ( System.Exception ex )
			{
				log.LogError( ex.ToLogMsg( true ), null );
				return new StatusCodeResult( StatusCodes.Status500InternalServerError );
			}

			APIResult<dynamic> results = new APIResult<dynamic>( APIResultType.Success )
			{
				ResultObject = trans,
			};

			return new OkObjectResult( results );
		}

		/// <summary>Calculates a SHA256 signature for ALL the lines of code</summary>
		/// <returns></returns>
		private static GasTransaction ParseTransaction(string body, ObjectId transID, ICommonGateway gate)
        {
			GasTransaction trans = new GasTransaction();

			// split the body on new lines - do we need \r\n as well
			string[] lines = body.Split( new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries );

			// log we have some data
			//gate.Log( $"File read with {lines.Length} records.", "Import", "Info" );

			// matches
			Match pricegal = new Regex( @"PRICE/GAL \$(?<pricegal>\d+\.\d{2,3})", RegexOptions.IgnoreCase | RegexOptions.Singleline ).Match( body );
			Match gals = new Regex( @"Supreme-\+ (?<gals>\d+\.\d{2,3})G", RegexOptions.IgnoreCase | RegexOptions.Singleline ).Match( body );

			trans.Price = Double.Parse( pricegal.Groups["pricegal"].Value );
			trans.Gallons = Double.Parse( gals.Groups["gals"].Value );
			trans.Location = $"{lines[5]} {lines[6]} {lines[7]}";

			return trans;
		}

	}
}

//WELCOME TO TIMEWISE
//STORE #173
//A TIMEWISE EXXON
//TIME WISE #173
//FG48127870001
//1002 MONTROSE
//HOUSTON, TX
//77019
//08/02/2020 846614424
//11:48:54 AM
//XXXXXXXXXXXX3372
//MOBILE
//INVOICE 005075
//AUTH 02358D
//MOBILE
//AUTH 02358D
//SEQUENCE 217487
//PUMP# 7
//Supreme-+ 14.990G
//PRICE/GAL $2.469
//FUEL TOTAL $ 37.01
//CREDIT $ 37.01
