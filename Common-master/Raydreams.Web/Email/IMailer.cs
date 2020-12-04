using System;

namespace Raydreams.Web.Email
{
	public interface IMailer
	{
		/// <summary>HTML format or not</summary>
		bool IsHTML { get; set; }

		/// <summary>List of valid email addresses</summary>
		string[] To { get; set; }

		/// <summary></summary>
		bool Send( string from, string subject, string body );
	}
}
