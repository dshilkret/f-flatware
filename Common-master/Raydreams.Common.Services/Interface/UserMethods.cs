using System;
using System.Collections.Generic;
using Raydreams.Common.Logging;
using Raydreams.Common.Extensions;
using Raydreams.Common.Services.Model;
using Raydreams.Common.Services.Security;

namespace Raydreams.Common.Services.Interface
{
	public partial class CommonGateway : ICommonGateway
	{
		#region [ User Methods ]

		/// <summary>Gets all the users in the specified domain</summary>
		public List<User> GetAllUsers( string domain )
		{
			// user must be an admin in the domain

			if ( String.IsNullOrWhiteSpace( domain ) )
				return new List<User>();

			return this.UsersRepo.GetAllByDomain( domain );
		}

		/// <summary>Gets the current logged in users info</summary>
		public UserInfo GetCurrentUser()
		{
			User u = this.CurrentUser;

			if ( u == null )
				return null;

			DomainRole r = u.GetRole( this.CurrentSession?.Domain );

			return new UserInfo() { ID = u.UserID, Name = u.DisplayName, Email = u.Email, Role = r.Role };
		}

		/// <summary>Gets a user by their userID</summary>
		/// <returns>A subset of user info</returns>
		/// <param name="userid">Userid.</param>
		public UserInfo GetUserByID( string userID )
		{
			if ( String.IsNullOrWhiteSpace( userID ) )
				return null;

			userID = userID.Trim();

			User u = null;

			try
			{
				// find the user by user ID
				u = this.UsersRepo.GetByUserID( userID );
			}
			catch ( System.Exception exp )
			{
				this.Log( exp );
			}

			return new UserInfo() { ID = u.UserID, Name = u.DisplayName, Email = u.Email };
		}

		/// <summary></summary>
		//public UserPreferences GetUserPreferences()
		//      {
		//	if ( this.CurrentUser == null || this.CurrentUser.ID == Guid.Empty )
		//		return null;

		//	User u = this.UsersRepo.Get( this.CurrentUser.ID );

		//	if (u == null)
		//		return null;

		//	UserPreferences pref = this.UsersRepo.GetPreferences(u.ID);
		//	pref.UserName = u.DisplayName;

		//	return pref;
		//}

		/// <summary>Add a new user</summary>
		/// <returns>The plain text password for the new user</returns>
		/// <remarks>Domain needs to be fixed</remarks>
		public (string UserID, string PW) InsertUser( string userID, string name, string email, string domain, string role )
		{
			// validate all the input
			if ( String.IsNullOrWhiteSpace( userID ) || String.IsNullOrWhiteSpace( name ) )
				return (null, null);

			if ( String.IsNullOrWhiteSpace( email ) || String.IsNullOrWhiteSpace( domain ) )
				return (null, null);

			// check ID DNE in domain to add to

			// clean input
			userID = userID.Trim();

			// parse the role
			RoleLevel lvl = RoleLevel.User;
			if ( !String.IsNullOrWhiteSpace( role ) )
				role.TryParseToEnum<RoleLevel>( out lvl, true );

			// actuall insert the user and get their ID and plain text PW
			(Guid ID, string PW) results = this.UsersRepo.Insert( userID, name.Trim(), email.Trim(),
				new DomainRole() { Domain = domain.Trim(), Role = lvl } );

			// log who did this
			if ( !String.IsNullOrWhiteSpace( results.PW ) )
				this.Log( $"New user {userID} created by {this.CurrentUser.UserID}.", "Audit", LogLevel.Info );
			else
				this.Log( $"{this.CurrentUser.UserID} failed to create a new user.", "Audit", LogLevel.Error );

			// should return more than this
			return (userID, results.PW);
		}

		/// <summary>Disables the user with the specified User ID</summary>
		public bool DisableUser( string userID )
		{
			// user must be an admin in the domain

			User u = this.UsersRepo.GetByUserID( userID );

			if ( u == null )
				return false;

			bool results = this.UsersRepo.Disable( u.ID );

			this.Log( $"User {userID} was disabled by {this.CurrentUser.UserID}.", "Audit", LogLevel.Info );

			return results;
		}

		/// <summary>Completely removes the User</summary>
		public bool DeleteUser( string userID )
		{
			User u = this.UsersRepo.GetByUserID( userID );

			if ( u == null )
				return false;

			bool results = this.UsersRepo.Delete( u.ID );

			this.Log( $"User {userID} was deleted by {this.CurrentUser.UserID}.", "Audit", LogLevel.Info );

			return results;
		}

		/// <summary>For now only updates the display name and email</summary>
		public bool UpdateUser( string userID, string name, string email )
		{
			User u = this.UsersRepo.GetByUserID( userID );

			if ( u == null )
				return false;

			bool results = this.UsersRepo.Update( u.ID, name, email );

			this.Log( $"User {userID} was updated by {this.CurrentUser.UserID}.", "Audit", LogLevel.Info );

			return results;
		}

		#endregion [ User Methods ]
	}
}
