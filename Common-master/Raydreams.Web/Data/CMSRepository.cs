using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Raydreams.Common.Services.Model;

namespace Raydreams.Web.Data
{
    /// <summary></summary>
	public class CMSRepository : RESTServiceBase
    {
        public static readonly string AppDomain = @"raydreams.com";

        public static readonly string CommonCoreAPIKey = @"oVvDNLxVeVTptaX8LIzipOnXh1TTtZbLzkaw0n7nAJzfO/tOpgj7Ag==";
        public static readonly string CommonCoreURL = @"https://raydreams-common-dev-api.azurewebsites.net/api/";

        public CMSRepository() : base( CommonCoreURL )
        {
            base.APIKey = CommonCoreAPIKey;
        }

        /// <summary></summary>
        /// <returns></returns>
        public List<ContentHeader> GetContentList()
        {
            APIResult<List<ContentHeader>> results = new APIResult<List<ContentHeader>>();

            string postBody = JsonConvert.SerializeObject( new { site = AppDomain } );

            HttpRequestMessage message = this.PostRequest( "GetContent", postBody );

            try
            {
                HttpResponseMessage httpResponse = this.Client.SendAsync( message ).GetAwaiter().GetResult();
                string response = httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult(); ;

                // deserialize the response
                results = JsonConvert.DeserializeObject<APIResult<List<ContentHeader>>>( response );
            }
            catch ( System.Exception )
            {
                return null;
            }

            return results.ResultObject;
        }

        /// <summary>Gets a specific page</summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ContentPage GetContent( string path, string lang = "en" )
        {
            if ( String.IsNullOrWhiteSpace( path ) )
                return null;

            path = path.Trim();

            APIResult<ContentPage> results = new APIResult<ContentPage>();

            string postBody = JsonConvert.SerializeObject( new { site = AppDomain, path = path, lang = lang } );

            HttpRequestMessage message = this.PostRequest( "GetContent", postBody );

            try
            {
                HttpResponseMessage httpResponse = this.Client.SendAsync( message ).GetAwaiter().GetResult();
                string response = httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult(); ;

                // deserialize the response
                results = JsonConvert.DeserializeObject<APIResult<ContentPage>>( response );

            }
            catch ( System.Exception )
            {
                return null;
            }

            return results.ResultObject;
        }


        //      public ContentPage GetContent(string path)
        //{
        //          string postBody = JsonConvert.SerializeObject( new { site = "raydreams.com", path = path } );

        //          HttpRequestMessage message = new HttpRequestMessage( HttpMethod.Post, "https://raydreams-common-dev-api.azurewebsites.net/api/GetContent" );
        //          message.Headers.Clear();
        //          message.Content = new StringContent( postBody, Encoding.UTF8, "application/json" );
        //          message.Content.Headers.Add( "x-functions-key", "oVvDNLxVeVTptaX8LIzipOnXh1TTtZbLzkaw0n7nAJzfO/tOpgj7Ag==" );
        //          message.Content.Headers.Add( "Content-Length", postBody.Length.ToString() );
        //          //message.Content.Headers.Add( "User-Agent", "raydreams_com" );

        //         ContentPage result = null;

        //          try
        //          {
        //              HttpResponseMessage httpResponse = this.Client.SendAsync( message ).GetAwaiter().GetResult();
        //              string postResponse = httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult(); ;
        //              result = JsonConvert.DeserializeObject<ContentPage>( postResponse );
        //          }
        //          catch ( System.Exception )
        //          {
        //              return null;
        //          }

        //          return result;
        //      }
    }
}
