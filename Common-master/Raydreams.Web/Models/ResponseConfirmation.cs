using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Raydreams.Web.Models
{
    /// <summary>Used as the general model for most of the actions</summary>
    public class BaseViewModel
    {
        public BaseViewModel( HttpRequest req )
        {
            this.Request = req;

            IsSuccess = false;
            Message = String.Empty;
            Code = String.Empty;
        }

        public HttpRequest Request { get; private set; }

        /// <summary>Was it a success</summary>
        public bool IsSuccess { get; set; }

        /// <summary>What message to display</summary>
        public string Message { get; set; }

        /// <summary>The orginal code passed in or new code to display to the user</summary>
        public string Code { get; set; }

        /// <summary>The list of states dictionary to use if necessary</summary>
        public Dictionary<string, string> States { get; set; }

        /// <summary>Echos the IP address of the client itself</summary>
        public string ClientIP
        {
            get
            {
                string ip = Request.HttpContext.Connection.RemoteIpAddress.ToString();

                if ( ip == "::1" )
                    ip = "127.0.0.1";

                return ip;
            }
        }

        /// <summary>The API end point to use as determined by the environment</summary>
        public string API
        {
            get
            {
                return $"{this.Request.Scheme}://{this.Request.Host}/Home/";
            }
        }

        /// <summary>Is this a dev environment</summary>
        public bool IsDev
        {
            get
            {
                return ( this.API.Contains( "localhost" ) || this.API.Contains( "-dev" ) );
            }
        }
    }
}
