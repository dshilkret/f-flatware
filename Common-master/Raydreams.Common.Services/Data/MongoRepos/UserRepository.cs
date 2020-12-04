using System;
using MongoDB.Driver;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Raydreams.Common.Data;
using Raydreams.Common.Services.Model;
using Raydreams.Common.Services.Security;

namespace Raydreams.Common.Services.Data
{
	/// <summary>For CRUD operations on the User physical data source.</summary>
	/// <summary>For CRUD operations on the User physical data source.</summary>
	public class UsersRepository : MongoDataManager<User, Guid>, IUsersRepository
	{
		#region [Fields]

		private string _table = "Users";

		/// <summary>The Password Field physical name</summary>
		public static readonly string PWFieldName = "pw";

		#endregion [Fields]

		#region [Constructors]

		/// <summary>Constrcutor with a password maker</summary>
		public UsersRepository( IPasswordMaker pw, string connStr, string db, string table ) : base( connStr, db )
		{
			this.Passwords = pw;
			this.Table = table;
		}

		#endregion [Constructors]

		#region [Properties]

		/// <summary>Interface class used to generate passwords</summary>
		public IPasswordMaker Passwords { get; set; }

		/// <summary>The physical Collection name</summary>
		public string Table
		{
			get { return this._table; }
			protected set { if ( !String.IsNullOrWhiteSpace( value ) ) this._table = value.Trim(); }
		}

		#endregion [Properties]

		#region [Methods]

		/// <summary>Gets by the DB ID</summary>
		/// <returns></returns>
		public User Get( Guid id )
		{
			return base.Get( id, this.Table );
		}

		/// <summary>Gets by the common user ID</summary>
		/// <returns></returns>
		public User GetByUserID( string id )
		{
			if ( String.IsNullOrWhiteSpace( id ) )
				return null;

			id = id.Trim().ToLowerInvariant();

			IMongoCollection<User> collection = this.Database.GetCollection<User>( this.Table );
			List<User> results = collection.Find<User>( u => u.UserID.ToLowerInvariant() == id ).ToList();

			return ( results != null && results.Count > 0 ) ? results[0] : null;
		}

		/// <summary>Gets every user account in the specified domain</summary>
		/// <returns></returns>
		public List<User> GetAllByDomain( string domain, bool enabledOnly = true )
		{
			IMongoCollection<User> collection = this.Database.GetCollection<User>( this.Table );

			List<User> results = collection.Find( Builders<User>.Filter.Empty ).ToList();

			return ( results != null && results.Count > 0 ) ? results : new List<User>();
		}

		/// <summary>Returns just a pw hash for the specified user</summary>
		/// <remarks>Bug user ID has to match case</remarks>
		public string GetHashedPWByUserID( string userID )
		{
			if ( String.IsNullOrWhiteSpace( userID ) )
				return null;

			//userID = userID.Trim().ToLowerInvariant();

			IMongoCollection<BsonDocument> collection = this.Database.GetCollection<BsonDocument>( this.Table );
			FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq( "userID", userID );
			List<BsonDocument> results = collection.Find<BsonDocument>( filter ).ToList();

			return ( results != null && results.Count > 0 ) ? results[0].GetValue( PWFieldName ).ToString() : null;
		}

		/// <summary>Gets a user by a userID and hashed PW match</summary>
		/// <returns></returns>
		/// <remarks>Not used</remarks>
		public User GetByLogin( string userid, string pwhash )
		{
			if ( String.IsNullOrWhiteSpace( userid ) || String.IsNullOrWhiteSpace( pwhash ) )
				return null;

			IMongoCollection<BsonDocument> collection = this.Database.GetCollection<BsonDocument>( this.Table );

			var filterBuilder = Builders<BsonDocument>.Filter;
			var filter = filterBuilder.Eq( "userID", userid.Trim() ) & filterBuilder.Eq( PWFieldName, pwhash.Trim() );

			List<BsonDocument> results = collection.Find<BsonDocument>( filter ).ToList();

			if ( results == null || results.Count < 0 )
				return null;

			string result = results[0].GetValue( PWFieldName, String.Empty ).AsString;

			// check for an exact match on the pw
			if ( !result.Equals( pwhash, StringComparison.InvariantCulture ) )
				return null;

			return BsonSerializer.Deserialize<User>( results[0] );
		}

		/// <summary>Get a request by its confirmation code</summary>
		public User GetByResetRequest( string code )
		{
			if ( String.IsNullOrWhiteSpace( code ) )
				return null;

			IMongoCollection<User> coll = this.Database.GetCollection<User>( this.Table );
			var filter = Builders<User>.Filter.AnyEq( "resets.code", code.Trim() );
			List<User> results = coll.Find<User>( filter ).ToList();

			return ( results != null && results.Count > 0 ) ? results[0] : null;
		}

		/// <summary>Returns the user by an active session ID</summary>
		public User GetBySessionID( Guid id )
		{
			if ( id == Guid.Empty )
				return null;

			IMongoCollection<User> coll = this.Database.GetCollection<User>( this.Table );
			var filter = Builders<User>.Filter.AnyEq( "sessions.id", id );
			List<User> results = coll.Find<User>( filter ).ToList();

			if ( results == null || results.Count < 1 )
				return null;

			return results[0];
		}

		/// <summary>Returns just the preferences child object</summary>
		/// <param name="oid"></param>
		/// <returns></returns>
		/// <remarks>Really just get the user</remarks>
		//public UserPreferences GetPreferences(Guid oid)
		//      {
		//	User u = this.Get(oid);

		//	if (u == null || u.Preferences == null)
		//		return null;

		//	return u.Preferences;
		//      }

		/// <summary>Add a new account</summary>
		/// <returns>Returns the new temp plaintext PW if all is a success, otherwise null.</returns>
		public (Guid ID, string PlainPW) Insert( string userID, string name, string email, DomainRole role )
		{
			if ( String.IsNullOrWhiteSpace( userID ) || String.IsNullOrWhiteSpace( name ) )
				return (Guid.Empty, String.Empty);

			userID = userID.Trim();

			// validate the userID address
			if ( userID.Length < 3 )
				return (Guid.Empty, String.Empty);

			// generate PW from the Password maker
			(string PW, string Hash) pair = this.Passwords.MakeRandom();

			User acct = new User()
			{
				UserID = userID,
				DisplayName = name.Trim(),
				ResetPassword = true,
				Created = DateTime.UtcNow,
				Updated = DateTime.UtcNow,
				Email = email.Trim(),
				Enabled = true
			};

			if ( role == null || String.IsNullOrWhiteSpace( role.Domain ) )
				acct.Roles = new List<DomainRole>();
			else
				acct.Roles = new List<DomainRole>() { role };

			// try to insert user
			if ( !base.Insert( acct, this.Table ) )
				return (Guid.Empty, String.Empty);

			// update PW separately now we know the ID
			IMongoCollection<User> collection = this.Database.GetCollection<User>( this.Table );
			FilterDefinition<User> filter = Builders<User>.Filter.Where( u => u.ID == acct.ID );
			UpdateDefinition<User> update = Builders<User>.Update.Set( PWFieldName, pair.Hash );
			UpdateResult result = collection.UpdateOne( filter, update );

			// return the plain text temp pw
			return ( result.ModifiedCount > 0 ) ? (acct.ID, pair.PW) : (acct.ID, String.Empty);
		}

		/// <summary>Updates a user's most basic info</summary>
		public bool Update( Guid id, string name, string email )
		{
			if ( id == Guid.Empty )
				return false;

			// have at least one parameter to update
			if ( String.IsNullOrWhiteSpace( name ) && String.IsNullOrWhiteSpace( email ) )
				return false;

			IMongoCollection<User> collection = this.Database.GetCollection<User>( this.Table );
			FilterDefinition<User> filter = Builders<User>.Filter.Eq( "_id", id );

			var updates = new List<UpdateDefinition<User>>();

			if ( !String.IsNullOrWhiteSpace( name ) )
				updates.Add( Builders<User>.Update.Set( "name", name.Trim() ) );

			if ( !String.IsNullOrWhiteSpace( email ) )
				updates.Add( Builders<User>.Update.Set( "email", email.Trim() ) );

			updates.Add( Builders<User>.Update.Set( "updated", DateTime.UtcNow ) );

			UpdateResult result = collection.UpdateOne( filter, Builders<User>.Update.Combine( updates ) );

			return ( result.ModifiedCount > 0 );
		}

		/// <summary>Updates the plan text PW for the user object.</summary>
		/// <param name="id">The actual object ID of the user</param>
		/// <param name="newPW">New plain text PW that will be hashed using Password factory class</param>
		/// <returns></returns>
		public bool UpdatePassword( Guid id, string newPW )
		{
			if ( id == Guid.Empty || String.IsNullOrWhiteSpace( newPW ) )
				return false;

			string hashPW = Passwords.HashPassword( newPW.Trim() );

			IMongoCollection<User> collection = this.Database.GetCollection<User>( this.Table );

			// update PW separately now we know the ID
			FilterDefinition<User> filter = Builders<User>.Filter.Eq( "_id", id );
			UpdateDefinition<User> update = Builders<User>.Update.Set( PWFieldName, hashPW ).Set( "resetPW", false ).Set( "updated", DateTime.UtcNow );
			UpdateResult result = collection.UpdateOne( filter, update );

			return ( result.ModifiedCount > 0 );
		}

		/// <summary>Generates a random PW, hashes and stores in the DB, and returns the plain text</summary>
		/// <param name="oid"></param>
		/// <returns>PLan text version of the PW.</returns>
		public string ResetPassword( Guid id )
		{
			if ( id == Guid.Empty )
				return null;

			// generate PW from bcrypt and returned new temp hashed PW
			(string PW, string Hash) pair = Passwords.MakeRandom();

			IMongoCollection<User> collection = this.Database.GetCollection<User>( this.Table );

			// update PW separately now we know the ID
			FilterDefinition<User> filter = Builders<User>.Filter.Eq( "_id", id );
			UpdateDefinition<User> update = Builders<User>.Update.Set( PWFieldName, pair.Hash ).Set( "resetPW", true ).Set( "updated", DateTime.UtcNow );
			UpdateResult result = collection.UpdateOne( filter, update );

			// return the plain text temp pw
			return ( result.ModifiedCount > 0 ) ? pair.PW : null;
		}

		/// <summary>Disable a user by their oid</summary>
		/// <returns></returns>
		public bool Disable( Guid id )
		{
			if ( id == Guid.Empty )
				return false;

			IMongoCollection<User> collection = this.Database.GetCollection<User>( this.Table );
			FilterDefinition<User> filter = Builders<User>.Filter.Eq( "_id", id );
			UpdateDefinition<User> update = Builders<User>.Update.Set( "enabled", false ).Set( "updated", DateTime.UtcNow );
			UpdateResult result = collection.UpdateOne( filter, update );

			return ( result.ModifiedCount > 0 );
		}

		/// <summary>Inserts a new domain role for the specified user</summary>
		/// <returns></returns>
		// NOT DONE
		public bool InsertRole( Guid id, DomainRole role )
		{
			if ( id == Guid.Empty )
				return false;

			if ( role == null || String.IsNullOrWhiteSpace( role.Domain ) )
				return false;

			//IMongoCollection<User> collection = this.Database.GetCollection<User>( this.Table );
			//FilterDefinition<User> filter = Builders<User>.Filter.Eq( "_id", id );
			//UpdateDefinition<User> update = Builders<User>.Update.Set( "role", role ).Set( "updated", DateTime.UtcNow );
			//UpdateResult result = collection.UpdateOne( filter, update );

			//return (result.ModifiedCount > 0);

			return false;
		}

		/// <summary>Updates the lastlogin to now of the specified user</summary>
		public bool UpdateLastLogin( Guid id )
		{
			if ( id == Guid.Empty )
				return false;

			IMongoCollection<User> collection = this.Database.GetCollection<User>( this.Table );
			var filter = Builders<User>.Filter.Eq( "_id", id );
			UpdateDefinition<User> update = Builders<User>.Update.Set( "lastLogin", DateTime.UtcNow );

			UpdateResult result = collection.UpdateOne( filter, update );

			return ( result.ModifiedCount > 0 );
		}

		/// <summary>Inserts the current timestamp into the Failed login attempts of the specified user</summary>
		/// <param name="id">The User DB ID who failed to login correctly</param>
		/// <returns></returns>
		public bool InsertFailedLogin( Guid id )
		{
			if ( id == Guid.Empty )
				return false;

			IMongoCollection<User> collection = this.Database.GetCollection<User>( this.Table );
			FilterDefinition<User> filter = Builders<User>.Filter.Eq( "_id", id );
			UpdateDefinition<User> update = Builders<User>.Update.AddToSet( "fails", DateTime.UtcNow ).Set( "updated", DateTime.UtcNow );

			UpdateResult result = collection.UpdateOne( filter, update );

			return ( result.ModifiedCount > 0 );
		}

		/// <summary>Resets the failed login attempts back to zero</summary>
		/// <param name="id">The User's DB ID who loogged</param>
		/// <returns></returns>
		public bool ClearFailedLogins( Guid id )
		{
			if ( id == Guid.Empty )
				return false;

			IMongoCollection<User> collection = this.Database.GetCollection<User>( this.Table );
			FilterDefinition<User> filter = Builders<User>.Filter.Eq( "_id", id );
			UpdateDefinition<User> update = Builders<User>.Update.Set( "fails", new List<DateTime>() ).Set( "updated", DateTime.UtcNow );

			UpdateResult result = collection.UpdateOne( filter, update );

			return ( result.ModifiedCount > 0 );
		}

		/// <summary>Completely deletes a user</summary>
		/// <param name="id">The users DB ID</param>
		/// <returns>True if the user was successfully deleted otherwise false</returns>
		public bool Delete( Guid id )
		{
			if ( id == Guid.Empty )
				return false;

			return base.Delete( id, this.Table );
		}

		#endregion [Methods]
	}
}
