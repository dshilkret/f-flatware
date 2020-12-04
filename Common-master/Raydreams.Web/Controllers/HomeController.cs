using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Raydreams.Web.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using Newtonsoft.Json.Linq;
using Raydreams.Web.Email;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using Raydreams.Web.Data;
using Raydreams.Common.Services.Model;
using Microsoft.Extensions.Logging;
using Raydreams.Common.Services.Config;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Raydreams.Web.Controllers
{
    public class HomeController : Controller
    {
        #region [ Constructor ]

        public HomeController( IWebHostEnvironment env )
        {
            //_logger = logger;
            //env.EnvironmentName

            // get the environment key and setup the repo to the API
            // you can alter the environment locally in the project settings
            //this.EnvSettings = EnvironmentSettings.GetSettings( Environment.GetEnvironmentVariable( "env" ) );
        }

        #endregion [ Constructor ]

        #region [ Properties ]

        #endregion [ Properties ]

        /// <summary></summary>
        public IActionResult Index()
        {
            ViewData["Title"] = "Home";
            ViewData["SelectedTab"] = "index";
            return View();
        }

        /// <summary></summary>
        public IActionResult MarkerMatch()
        {
            ViewData["Title"] = "Marker Match";
            ViewData["SelectedTab"] = "mm";
            return View();
        }

        /// <summary></summary>
        public IActionResult BoardBuilder()
        {
            ViewData["Title"] = "Board Builder";
            ViewData["SelectedTab"] = "bb";
            return View();
        }

        /// <summary></summary>
        public IActionResult Apparel()
        {
            ViewData["Title"] = "Apparel";
            ViewData["SelectedTab"] = "apparel";
            return View();
        }

        /// <summary></summary>
        public IActionResult Contact()
        {
            ViewData["Title"] = "Contact";
            ViewData["SelectedTab"] = "contact";
            ViewData["API"] = $"{this.Request.Scheme}://{this.Request.Host}/";
            ViewData["ClientIP"] = this.Request.HttpContext.Connection.RemoteIpAddress.ToString();
            return View();
        }

        /// <summary></summary>
        public IActionResult Color()
        {
            FormEncryptRepo repo = new FormEncryptRepo();
            List<ProductLine> lines = repo.GetProductLines();

            ViewData["Title"] = "Marker Match Database";
            ViewData["SelectedTab"] = "color";
            ViewData["API"] = $"{this.Request.Scheme}://{this.Request.Host}/";

            ColorMatchModel model = new ColorMatchModel() { Lines = lines };

            return View( model );
        }

        /// <summary></summary>
        public IActionResult Encrypt()
        {
            ViewData["Title"] = "Form Encrypt Demo";
            ViewData["SelectedTab"] = "demo";
            ViewData["API"] = $"{this.Request.Scheme}://{this.Request.Host}/";

            return View();
        }

        /// <summary></summary>
        public IActionResult Disclaimer()
        {
            ViewData["Title"] = "Disclaimer";
            return View();
        }

        /// <summary></summary>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary></summary>
        [HttpPost]
        public async Task<IActionResult> Signature()
        {
            string requestBody = await new StreamReader(this.Request.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string msg = data?.msg;

            return new OkObjectResult(new { Message = $"Hello {msg}" });
        }

        /// <summary></summary>
        [ResponseCache( Duration = 0, Location = ResponseCacheLocation.None, NoStore = true )]
        public IActionResult Error()
        {
            return View( new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier } );
        }

        #region [ API Methods ]

        /// <summary>Handles the calculator earnings projection calls</summary>
        /// <param name="size">Galaxy level 1 size</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult MatchColor( string line, string hex )
        {
            // validate the input is an int
            if ( String.IsNullOrWhiteSpace( line ) )
                return new OkObjectResult( new { IsSuccess = false } );

            FormEncryptRepo repo = new FormEncryptRepo() { SessionID = "gx83sfhvf0" };
            ProductLine results = repo.MatchColor(line, hex);

            return new OkObjectResult( new { IsSuccess = true, Results = results } );
        }

        /// <summary>Handles the calculator earnings projection calls</summary>
        /// <param name="size">Galaxy level 1 size</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> EncryptForm()
        {
            // get all the data
            string requestBody = await new StreamReader( this.Request.Body ).ReadToEndAsync();
            PlainForm data = JsonConvert.DeserializeObject<PlainForm>( requestBody );
            //dynamic data = JsonConvert.DeserializeObject( requestBody );

            if ( data == null )
                return new BadRequestObjectResult( "No Input" );

            //string name = data?.name;
            //string[] fields = data?.fields;
            //dynamic form = data?.form;

            if ( String.IsNullOrWhiteSpace( data.Name ) || data.FormData == null )
                return new OkObjectResult( new { IsSuccess = false } );

            FormEncryptRepo repo = new FormEncryptRepo();
            ServiceResponse<EncryptedForm> results = repo.EncryptForm( data.Name, data.Fields, data.FormData);

            // outer most object returned to the callback
            return new OkObjectResult( results );
        }

        /// <summary></summary>
        [HttpPost]
        public async Task<IActionResult> Recaptcha()
        {
			// get all the data
            string requestBody = await new StreamReader(this.Request.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string token = data?.token;
            string clientIP = data?.ip;

			// send in genric post format
            string postBody = String.Format( "secret=6LdgA9kUAAAAADjnWpRMglCIow-AfalXNGAqKSOI&response={0}", token );

            if ( !String.IsNullOrWhiteSpace(clientIP) )
				postBody += String.Format( "&remoteip={0}", clientIP );

            // call recaptcha API with a post request
            HttpClient httpClient = new HttpClient();
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, "https://www.google.com/recaptcha/api/siteverify");
            message.Headers.Clear();
            message.Content = new StringContent(postBody, Encoding.UTF8, "application/x-www-form-urlencoded");
            message.Content.Headers.Add("Content-Length", postBody.Length.ToString() );

            ReCaptchaResponse result = null;

            try
            {
                HttpResponseMessage httpResponse = await httpClient.SendAsync(message);
                string postResponse = await httpResponse.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<ReCaptchaResponse>( postResponse );
			}
            catch (System.Exception)
            {
                return new OkObjectResult(new { Verified = false });
            }

            return new OkObjectResult( new { Verified = result.Success } );
        }

        /// <summary></summary>
        [HttpPost]
        public async Task<IActionResult> SendFeedback()
        {
            string requestBody = await new StreamReader(this.Request.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            if (data == null)
                return new BadRequestObjectResult("No Input");

            string msg = data?.msg;
            string from = data?.email;
            string name = data?.name;
            string clientIP = data?.ip;
            //string captcha = data?.captcha;

            if ( String.IsNullOrWhiteSpace(msg) || String.IsNullOrWhiteSpace( from ) )
                return new BadRequestObjectResult("Invalid Input");

            //if (!captcha.Equals("one", StringComparison.InvariantCultureIgnoreCase) && captcha[0] != '1')
            //    return new BadRequestObjectResult("Bot");

            msg += $"{msg}\n\nIP Address:{clientIP}";

            string sgapikey = @"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";
            SendGridMailer mailer = new SendGridMailer(sgapikey);
            mailer.IsHTML = true;
            mailer.To = new string[] { @"tguillory@gmail.com" };

            bool sent = mailer.Send( from, $"Feedback from {name} to TAG Digital Studios", msg );

            return new OkObjectResult(true);
        }

        #endregion [ API Methods ]

    }
}
