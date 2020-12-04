using System;
using System.Collections.Generic;
using System.Drawing;
using Raydreams.Common.Security;
using Raydreams.Common.Services.Model;

namespace Raydreams.Common.Services.Interface
{
	/// <summary>Common Gateway Interface</summary>
	public interface ICommonGateway
	{
		/// <summary>The current user session</summary>
		Session CurrentSession { get; }

		/// <summary>Returns a signature on the instance of the app.</summary>
		/// <returns></returns>
		string Signature();

		/// <summary>Get the settings for the specified domain</summary>
		/// <param name="domain"></param>
		/// <returns></returns>
		Settings GetSettings();

		/// <summary></summary>
		/// <param name="message"></param>
		/// <param name="category"></param>
		/// <param name="level"></param>
		void Log(string message, string category, string level = null);

		/// <summary></summary>
		string CreateCharPIN( string fname, string lname, string mname = null );

		#region [ Session Methods ]

		/// <summary>Called to login a new user</summary>
		/// <returns>Returns a session object</returns>
		LoginResponse Login(string userID, string hashpw, string domain, string ip, bool isEncrypted = false);

		/// <summary>Validate the sessions is still alive. Expired sessions are removed and valid ones are updated.</summary>
		/// <returns>True if still active, otherwise false</returns>
		/// <remarks>Also called 'KeepAlive'</remarks>
		Session RefreshSession( TokenPayload token );

		/// <summary>Explicitly removes session associated with the user</summary>
		/// <param name="token">The session token itself</param>
		/// <returns></returns>
		bool Logout( TokenPayload token );

		/// <summary>Deletes all expired sessions</summary>
		long DeleteExpiredSessions(string domain);

		#endregion [ Session Methods ]

		#region [ User Methods ]

		/// <summary>Gets the current logged in users info</summary>
		UserInfo GetCurrentUser();

		/// <summary>Gets all the users in the specified domain</summary>
		/// <returns></returns>
		List<User> GetAllUsers( string domain );

		/// <summary></summary>
		/// <returns></returns>
		(string UserID, string PW) InsertUser( string userID, string name, string email, string domain, string role );

		/// <summary></summary>
		/// <returns></returns>
		bool DeleteUser( string userID );

		/// <summary></summary>
		/// <returns></returns>
		bool UpdateUser( string userID, string name, string email );

		/// <summary></summary>
		bool DisableUser( string userID );

		/// <summary>Gets prefs & settings specific to the user</summary>
		//UserPreferences GetUserPreferences();

		#endregion [ User Methods ]

		#region [ Password Methods ]

		/// <summary>Updates an existing password for the specified user</summary>
		bool UpdatePassword( string oldPW, string newPW );

		/// <summary>Changes the user's password to a random one and returns the new password</summary>
		string ResetPassword( Guid userID );

		/// <summary>Checks the password reset request exists</summary>
		bool ConfirmPasswordReset( string code );

		/// <summary>Sends a password reset request</summary>
		bool SendResetPassword( string userID, string magic );

		/// <summary>Updates an existing password to the new password using a password reset code</summary>
		bool UpdatePasswordByCode( string code, string newPW );

		#endregion [ Password Methods ]

		#region [ CMS Methods ]

		/// <summary>Gets a specifiec page</summary>
		ContentPage GetContentByID( string domain, Guid id, string lang = "en" );

		/// <summary>Gets a specifiec page</summary>
		ContentPage GetContentByPath( string domain, string path, string lang );

		/// <summary>Gets all content pages that fall within a speified domain as a list of headers</summary>
		List<ContentHeader> GetContentList( string domain, string type = null );

		/// <summary>Inserts a new page</summary>
		bool InsertContentPage( ContentPage page );

		/// <summary>Gets all the Page Content Tags</summary>
		List<ContentTag> GetContentTags();

		#endregion [ CMS Methods ]

		#region [ Color Methods ]

		/// <summary></summary>
		List<ProductLine> GetProductLines();

		/// <summary></summary>
		List<ProductItem> GetProductItems();

		/// <summary></summary>
		ProductLine MatchColor( string lineKey, Color color, int top = 10 );

		#endregion [ Color Methods ]

		#region [ Stock Methods ]

		/// <summary></summary>
		List<LookupPair> GetPortfolioList();

		/// <summary></summary>
		Portfolio GetPortfolio(string name);

		/// <summary></summary>
		int InsertWatchedStock(string name, string symbol, double price);

		/// <summary></summary>
		bool DeleteWatchedStock(string name, string symbol);

		/// <summary></summary>
		bool InsertPortfolio(string name );

		#endregion [ Stocks Methods ]
	}
}
