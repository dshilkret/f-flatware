using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using Raydreams.Common.Services.Model;

namespace Raydreams.Web.Data
{
    /// <summary>Services written for the Form Encrypt API</summary>
    public class FormEncryptRepo : RESTServiceBase
    {
        public static readonly string AppDomain = @"raydreams.com";

        //public static readonly string CommonCoreAPIKey = @"EfxEuOUGIbXr9FZKSOST/r3zS2u4zP0J8OSnzYdnVMTLDJULykAF6Q==";
        //public static readonly string CommonCoreURL = @"https://formencrypt-dev-v1-api.azurewebsites.net/api";

        public static readonly string CommonCoreAPIKey = @"oVvDNLxVeVTptaX8LIzipOnXh1TTtZbLzkaw0n7nAJzfO/tOpgj7Ag==";
        public static readonly string CommonCoreURL = @"https://raydreams-common-dev-api.azurewebsites.net/api/";

        public FormEncryptRepo() : base( CommonCoreURL )
        {
            base.APIKey = CommonCoreAPIKey;
            base.AuthTokenHeader = "x-api-authorization";
        }

        /// <summary></summary>
        /// <returns></returns>
        public ProductLine MatchColor( string line, string hex )
        {
            if ( String.IsNullOrWhiteSpace( line ) )
                return null;

            if ( String.IsNullOrWhiteSpace( hex ) )
                hex = "000000";

            APIResult<ProductLine> results = new APIResult<ProductLine>();

            var param = new Dictionary<string, string>() {
                { "line", line },
                { "hex", hex }
            };

            HttpRequestMessage message = this.GetRequest( "MatchColor", param, true, true);
            HttpResponseMessage httpResponse = this.Client.SendAsync( message ).GetAwaiter().GetResult();
            string response = httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult(); ;
            results = JsonConvert.DeserializeObject<APIResult<ProductLine>>( response );

            return results.ResultObject;
        }

        /// <summary></summary>
        /// <returns></returns>
        public List<ProductLine> GetProductLines()
        {
            APIResult<List<ProductLine>> results = new APIResult<List<ProductLine>>();

            //string postBody = JsonConvert.SerializeObject( new { site = AppDomain } );

            HttpRequestMessage message = this.GetRequest( "ProductLines", true, false );

            try
            {
                HttpResponseMessage httpResponse = this.Client.SendAsync( message ).GetAwaiter().GetResult();
                string response = httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult(); ;

                // deserialize the response
                results = JsonConvert.DeserializeObject<APIResult<List<ProductLine>>>( response );
            }
            catch ( System.Exception )
            {
                // log it
                return new List<ProductLine>();
            }

            return results.ResultObject;
        }

        /// <summary>Call to the EncryptForm API func</summary>
        /// <param name="name"></param>
        /// <param name="fields"></param>
        /// <param name="data"></param>
        public ServiceResponse<EncryptedForm> EncryptForm(string name, string[] fields, dynamic data)
        {
            ServiceResponse<EncryptedForm> results = new ServiceResponse<EncryptedForm>();

            string postBody = JsonConvert.SerializeObject( new { name = name, format="base64", fields = fields, formData = data } );

            HttpRequestMessage message = this.PostRequest( "EncryptForm", postBody, false );

            try
            {
                HttpResponseMessage httpResponse = this.Client.SendAsync( message ).GetAwaiter().GetResult();
                string response = httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult(); ;

                // deserialize the response
                results.Data = JsonConvert.DeserializeObject<APIResult<EncryptedForm>>( response );
                results.StatusCode = httpResponse.StatusCode;
            }
            catch ( System.Exception )
            {
                return null;
            }

            return results;
        }

    }
}
