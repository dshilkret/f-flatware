using System;
using System.Collections.Generic;
using Raydreams.Common.Logging;
using Raydreams.Common.Extensions;
using Raydreams.Common.Services.Model;
using Raydreams.Common.Services.Security;
using Raydreams.Common.Email;
using Raydreams.Common.Logic;
using System.Threading.Tasks;
using System.Linq;

namespace Raydreams.Common.Services.Interface
{
    public partial class CommonGateway : ICommonGateway
    {
		#region [ Password Methods ]

		/// <summary>Update a user's password from old to new. Checks the old PW is a match first, then hashes the new one. The user must be logged in.</summary>
		/// <remarks>This is typically called from a website internally after login by the end user</remarks>
		public bool UpdatePassword( string oldPW, string newPW )
		{
			if ( this.CurrentUser == null )
				return false;

			// find the player by name and PW
			string hashpw = this.UsersRepo.GetHashedPWByUserID( this.CurrentUser.UserID );

			if ( hashpw == null )
				return false;

			// check the PW is valid
			if ( !new BCryptPasswordMaker().Compare( oldPW, hashpw ) )
				return false;

			// get the user
			User user = this.UsersRepo.GetByUserID( this.CurrentUser.UserID );

			// update the PW
			bool results = this.UsersRepo.UpdatePassword( user.ID, newPW );

			// log it
			this.Log( $"User {this.CurrentUser.UserID} changed their password.", "Audit", LogLevel.Info );

			return results;
		}

		/// <summary>Changes the user's password to a random one and returns the new password</summary>
		/// <returns>Thew new random plain text PW</returns>
		/// <remarks>This is typically called by an Admin to reset a user's PW for them</remarks>
		public string ResetPassword( Guid id )
		{
			// calling user must be an admin on the default domain

			// get the user
			User user = this.UsersRepo.Get( id );

			if ( user == null )
				return null;

			// update the PW
			string pw = this.UsersRepo.ResetPassword( user.ID );

			this.Log( $"Password for user {user.UserID} reset by {this.CurrentUser.UserID}.", "Audit", LogLevel.Info );

			return pw;
		}

		/// <summary>Comfirms the password reset exists before the reset form is presented</summary>
		public bool ConfirmPasswordReset( string code )
		{
			// basic param testing, log a bad request
			if ( String.IsNullOrWhiteSpace( code ) )
				return false;

			code = code.Trim();

			// get the reset code
			User req = this.UsersRepo.GetByResetRequest( code );

			return ( req != null && req.ResetRequests.Where( r => r.Code == code ).FirstOrDefault() != null );
		}

		/// <summary>Sends out a link to reset the user's password at the email or TXT number in the user's account. Adds a reset object to the users account info.</summary>
		/// <param name="userID">User ID of the user to reset for</param>
		/// <param name="magic">The magic word that the clinet must know</param>
		/// <returns></returns>
		public bool SendResetPassword( string userID, string magic )
		{
			if ( String.IsNullOrWhiteSpace( userID ) || String.IsNullOrWhiteSpace( magic ) )
				return false;

			// resetting the PW requires knowing the token hint
			//if (!this.Settings.TokenHint.Equals( magic, StringComparison.InvariantCulture ))
			//	return false;

			try
			{
				// get the account
				User user = this.UsersRepo.GetByUserID( userID );

				if ( user == null )
					return false;

				// create a random code
				Randomizer rand = new Randomizer();
				string resetCode = rand.RandomCode( 32, CharSet.Upper | CharSet.Lower | CharSet.Digits );
				//string link = "https://{0}/reset/{1}".Formatter( this.Settings.PublicWeb?.ShortURL, resetCode );

				//// write a password reset request to the DB
				//bool results = this.ResetRepo.Insert( resetCode, user.UserID, SessionDomain.App );

				//if (!results)
				//{
				//	this.Logger.Log( String.Format( "No PW Reset entry created for user {0}.", user.UserID ), LogLevel.Warn );
				//	return false;
				//}

				//// get the template
				//EmailTemplate et = this.TempsRepo.GetByName( "resetpw" );
				//Dictionary<string, string> values = new Dictionary<string, string>();
				//values.Add( "NAME", user.DisplayName );
				//values.Add( "LINK", link );

				//// send out email or TXT
				//IMailer mailer = this.Settings.Mail.Enabled ? (IMailer)new SendGridMailer( this.Settings.Mail.Key ) { IsHTML = true } : (IMailer)new LogMailer( this.Logger );
				//mailer.To = ( String.IsNullOrWhiteSpace( this.Settings.Mail.DebugTo ) ) ? new string[] { user.Email } : new string[] { this.Settings.Mail.DebugTo };

				//// send out an email with link and reset code
				//results = mailer.Send( this.Settings.Mail.From, et.Subject, Templator.Prepare( et.Template, values ) );
			}
			catch ( System.Exception exp )
			{
				this.Log( exp );
			}

			return true;
		}

		/// <summary>Changes the user's PW through the website by passing the confirmation code as well as the new password</summary>
		public bool UpdatePasswordByCode( string code, string newPW )
		{
			// basic param testing, log a bad request
			if ( String.IsNullOrWhiteSpace( code ) || String.IsNullOrWhiteSpace( newPW ) )
				return false;

			code = code.Trim();

			// check for valid PW format
			newPW = newPW.Trim();
			//if (!newPW.IsValidUserPassword())
			//return false;

			// check for valid reset code
			User req = this.UsersRepo.GetByResetRequest( code );

			if ( req == null || req.ResetRequests.Where( r => r.Code == code ).FirstOrDefault() != null )
				return false;

			bool updated = false;

			try
			{
				User user = this.UsersRepo.GetByUserID( req.UserID );

				if ( user == null )
					return false;

				// update to the new PW
				updated = this.UsersRepo.UpdatePassword( user.ID, newPW );

				// now remove the request
				if ( updated )
				{
					Task.Run( () => {

						// send out an email to the user
						//EmailTemplate et = this.TempsRepo.GetByName( "pwupdated", true );
						//Dictionary<string, string> values = new Dictionary<string, string>();
						//values.Add( "NUMBER", this.Settings.SupportNumber );
						//this.MailIt( et, values, new string[] { user.Email } );

						// remove any sessions
						//this.SessionRepo.DeleteAllByUser( user.UserID, SessionDomain.App );

						// remove the EVERY reset request related to this user
						this.ResetRepo.ClearResetRequests( user.ID );

					} );
				}
			}
			catch ( System.Exception exp )
			{
				this.Log( exp );
			}

			return updated;
		}

		#endregion [ Password Methods ]
	}
}
