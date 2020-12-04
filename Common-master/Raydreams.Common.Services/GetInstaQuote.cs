using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using Raydreams.Common.Services.Images;
using System.Drawing.Imaging;
using SkiaSharp;
using Raydreams.Common.Extensions;
using System.Net.Http;
using System.Net;

namespace Raydreams.Common.Services
{
    public static class GetInstaQuote
	{
        [FunctionName( "GetInstaQuote" )]
        public static IActionResult Run([HttpTrigger( AuthorizationLevel.Anonymous, "get", Route = null )] HttpRequest req,ILogger log)
        {
			log.LogInformation( "GetInstaQuote function triggered." );

			// message to display
			string width = req.Query["size"];
			string debug = req.Query["debug"];
			string message = req.Query["msg"];
			string bkgColor = req.Query["bkg"];
			string borderColor = req.Query["bc"];
			string starColor = req.Query["sc"];
			string borderWidth = req.Query["bw"];
			string stars = req.Query["stars"];
			string q = req.Query["q"];
			string textColor = req.Query["fc"];
			string textSize = req.Query["fs"];

			byte[] filebytes = null;
			//MemoryStream stream = new MemoryStream();

			// make a file name
			string id = Guid.NewGuid().ToString().Substring( 0, 4 );
			string fileName = $"InstaImage-{id}.png";

			try
			{
				int size = InstaImage.BoundInt( width.GetIntValue(), 100, 1000 );
				int qual = q.GetIntValue();

				qual = (qual <= 0 ) ? 75 : InstaImage.BoundInt(qual, 1, 100 );

				InstaImage img = new InstaImage( size );
				img.BorderColor = SkiaImage.GetColor( borderColor );

				// change to default white
				SkiaImage.DefaultColor = SkiaImage.White;
				img.BackgroundColor = SkiaImage.GetColor( bkgColor );
				img.StarColor = SkiaImage.GetColor( starColor );
				img.TextColor = SkiaImage.GetColor( textColor );
				img.FontSize = textSize.GetFloatValue();
				img.BorderThickness = borderWidth.GetIntValue();
				img.Message = message;
				img.LayoutGrid = debug.GetBooleanValue();
				img.StarDensity = stars.GetIntValue();
				
				img.Draw();

				filebytes = img.GetImage( qual ).ToArray();
				//img.GetImage( 90 ).SaveTo( stream );
			}
			catch (System.Exception exp)
			{
				log.LogError( exp.Message, null );
				return new StatusCodeResult( StatusCodes.Status500InternalServerError );
			}

            return new FileContentResult( filebytes, SkiaImage.ContentTypes[ImageFormat.Png] )
            {
                FileDownloadName = fileName
            };

        }
    }
}
