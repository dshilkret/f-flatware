using System;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace Raydreams.Web.Extensions
{
	public static class HttpRequestHelpers
	{
		public static string ResolveAppRoot( this HttpRequest request )
		{
			return "var x = 5";

		}

		//public static string ResolveAppRoot( this HttpRequest request )
		//{
		//	StringBuilder sb = new StringBuilder();
		//	sb.AppendFormat( "{0}://{1}", request.Url.Scheme, request.Headers["host"] );

		//	if ( !String.IsNullOrWhiteSpace( request.ApplicationPath ) )
		//		sb.Append( request.ApplicationPath );

		//	if ( !sb[sb.Length - 1].Equals( '/' ) )
		//		sb.Append( "/" );

		//	return sb.ToString();
		//}

		///// <summary></summary>
		///// <param name="request"></param>
		///// <returns></returns>
		//public static Uri UrlOriginal( this HttpRequestBase request )
		//{
		//	StringBuilder sb = new StringBuilder();
		//	sb.AppendFormat("{0}://{1}", request.Url.Scheme, request.Headers["host"]);

		//	if ( !String.IsNullOrWhiteSpace(request.ApplicationPath)  )
		//		sb.Append(request.ApplicationPath);

		//	if ( !sb[sb.Length - 1].Equals( '/' ) )
		//		sb.Append( "/" );

		//	return new Uri( sb.ToString() );
		//	//return new Uri( string.Format( "{0}://{1}{2}", request.Url.Scheme, hostHeader, request.RawUrl ) );
		//	//return new Uri( string.Format( "{0}://{1}", request.Url.Scheme, hostHeader ) );
		//}
	}
}
