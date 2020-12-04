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
using Raydreams.Common.Extensions;
using System.Reflection;
using System.Dynamic;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Raydreams.Common.Security;
using System.Linq;
using Raydreams.Common.Services.Security;

namespace Raydreams.Common.Services
{
    public static class EncryptForm
    {
        // password will be sent by the user or set in their account
        private static string TestPassword = "monkey";

        /// <summary>init vector to use</summary>
        //private static byte[] _iv = new byte[] { 0x86, 0x6a, 0x8f, 0xe4, 0xc4, 0xae, 0x46, 0x7f, 0x7b, 0x6f, 0x94, 0x34, 0xd4, 0xf8, 0xac, 0x66 };

        /// <summary>salt is generated as part of the users account</summary>
        private static byte[] _salt = new byte[] { 0x9f, 0x57, 0x8b, 0xc5, 0xe9, 0xfc, 0x13, 0x20, 0xc2, 0x42, 0x79, 0x44, 0x74, 0xff, 0xc5, 0x7e };

        [FunctionName( "EncryptForm" )]
        public static async Task<IActionResult> Run([HttpTrigger( AuthorizationLevel.Anonymous, "post", Route = null )] HttpRequest req, ILogger log )
        {
            log.LogInformation( "EncryptForm function triggered." );

            // read the input
            string requestBody = await new StreamReader( req.Body ).ReadToEndAsync();
            PlainForm form = JsonConvert.DeserializeObject<PlainForm>( requestBody );

            // validate input
            if ( form == null || String.IsNullOrWhiteSpace( form.Name ) || form.FormData == null )
                return new BadRequestObjectResult( "Invalid input parameters." );

            // validate
            if ( String.IsNullOrWhiteSpace( form.Format ) )
                form.Format = "base64";

            if ( form.Fields == null )
                form.Fields = new string[0];


            // create an output object
            //dynamic outForm = new ExpandoObject();
            //Dictionary<string, string> dict = new Dictionary<string, string>();
            APIResult<EncryptedForm> results = new APIResult<EncryptedForm>();
            EncryptedForm dict = new EncryptedForm() { Name = form.Name, Format = form.Format, Fields = form.Fields };

            try
            {
                // make a new 32 bit encryption key
                // user could have their password for encryption
                byte[] key = new FormFieldKeyMaker().MakeKey( TestPassword, _salt, 1000 );
                SymmetricEncryptor enc = new SymmetricEncryptor( SymmetricAlgoType.AES );
                dict.IV = enc.CreateIV();

                // iterate over the form properties
                foreach ( JProperty pi in form.FormData.Properties() )
                {
                    string plain = pi.Value.ToString();

                    if ( form.Fields.Contains( pi.Name ) )
                    {
                        byte[] encField = enc.Encrypt( plain, key, dict.IV );

                        if ( form.Format.Equals( "base64", StringComparison.InvariantCultureIgnoreCase ) )
                        {
                            string encrypted = Convert.ToBase64String( encField, Base64FormattingOptions.None );
                            dict.FormData.Add( pi.Name, encrypted );
                        }
                        else if ( form.Format.Equals( "hex", StringComparison.InvariantCultureIgnoreCase ) )
                        {
                            string encrypted = encField.ToHexString();
                            dict.FormData.Add( pi.Name, encrypted );
                        }
                        else
                        {
                            string encrypted = String.Join( ",", encField.Select( b => b.ToString() ).ToArray() );
                            dict.FormData.Add( pi.Name, encrypted );
                        }
                    }
                    else
                    {
                        dict.FormData.Add( pi.Name, plain );
                    }
                }
            }
            catch ( System.Exception ex )
            {
                log.LogError( ex.ToLogMsg( true ), null );
                //return new StatusCodeResult( StatusCodes.Status500InternalServerError );
                return new OkObjectResult( new { IsSuccess = false, Results = form, Message = ex.ToLogMsg( true ) } );
            }

            results.ResultObject = dict;
            results.ResultCode = APIResultType.Success;

            return new OkObjectResult( results );
        }
    }
}
