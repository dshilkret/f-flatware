using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using Raydreams.Common.Data;
using Raydreams.Common.Services.Model;

namespace Raydreams.Common.Services.Data
{
	/// <summary></summary>
    public class MongoSessionsRepo : MongoDataManager<User, Guid>, ISessionsRepository
    {
		#region [Fields]

		private string _table = "Users";

		#endregion [Fields]

		#region [Constructors]

		/// <summary></summary>
		public MongoSessionsRepo( string connStr, string db, string table ) : base( connStr, db )
		{
			this.Table = table;
		}

		#endregion [Constructors]

		#region [Properties]

		/// <summary>The table to use for this data source</summary>
		public string Table
		{
			get { return this._table; }
			protected set { if ( !String.IsNullOrWhiteSpace( value ) ) this._table = value.Trim(); }
		}

		#endregion [Properties]

		/// <summary>Delete a session with the specified ID</summary>
		public bool Delete( Guid id )
        {
			if ( id == Guid.Empty )
				return false;

			Session current = this.Get( id );

			if ( current == null )
				return false;

			IMongoCollection<User> col = this.Database.GetCollection<User>( this.Table );
			var filter = Builders<User>.Filter.Where( u => u.ID == current.UserID );
			UpdateDefinition<User> update = Builders<User>.Update.PullFilter( "sessions", Builders<Session>.Filter.Where( s => s.ID == current.ID ) );
			UpdateResult result = col.UpdateOne( filter, update );

			return ( result.ModifiedCount > 0 );
		}

		/// <summary>Deletes all sessions for the specified user and domain</summary>
		public long DeleteByUser( Guid userID, string domain )
        {
			if ( userID == Guid.Empty || String.IsNullOrWhiteSpace( domain ) )
				return 0;

			domain = domain.Trim().ToLowerInvariant();

			IMongoCollection<User> coll = this.Database.GetCollection<User>( this.Table );
			var filter = Builders<User>.Filter.Where( u => u.ID == userID );
			UpdateDefinition<User> update = Builders<User>.Update.PullFilter( "sessions", Builders<Session>.Filter.Where( s => s.Domain.ToLowerInvariant() == domain ) );
			UpdateResult result = coll.UpdateOne( filter, update );

			return result.ModifiedCount;
		}

		/// <summary></summary>
		public long DeleteExpired( TimeSpan ts, string domain )
		{
			if ( String.IsNullOrWhiteSpace( domain ) )
				return 0;

			domain = domain.Trim().ToLowerInvariant();

			if ( ts.Ticks < 0 )
				ts = new TimeSpan( 0 );

			DateTime now = DateTime.UtcNow.Subtract( ts );

			IMongoCollection<User> coll = this.Database.GetCollection<User>( this.Table );
			FilterDefinition<User> filter = Builders<User>.Filter.Empty;
			UpdateDefinition<User> update = Builders<User>.Update.PullFilter( "sessions", Builders<Session>.Filter.Where( s => s.LastModified <= now && s.Domain.ToLowerInvariant() == domain ) );
			UpdateResult result = coll.UpdateMany( filter, update );

			return result.ModifiedCount;
		}

		/// <summary></summary>
		public Session Get( string id )
        {
			return this.Get( Guid.Parse( id ) );
        }

		/// <summary></summary>
		public Session Get( Guid id )
        {
			if ( id == Guid.Empty )
				return null;

			IMongoCollection<User> coll = this.Database.GetCollection<User>( this.Table );
			var filter = Builders<User>.Filter.AnyEq( "sessions.id", id );
			List<User> results = coll.Find<User>( filter ).ToList();

			if ( results == null || results.Count < 1 )
				return null;

			Session s = results[0].Sessions.Where( s => s.ID == id ).FirstOrDefault();

			if ( s == null )
				return null;

			s.UserID = results[0].ID;
			return s;
		}

		/// <summary></summary>
        public List<Session> GetByUser( Guid id, string domain )
        {
			if ( id == Guid.Empty )
				return null;

			domain = domain.Trim().ToLowerInvariant();

			IMongoCollection<User> coll = this.Database.GetCollection<User>( this.Table );
			List<User> results = coll.Find<User>( u => u.ID == id ).ToList();

			if ( results == null || results.Count < 1 )
				return new List<Session>();

			// add user ID
			List<Session> ses = results[0].Sessions.Where( s => s.Domain.ToLowerInvariant() == domain ).ToList();
			ses.ForEach( s => s.UserID = results[0].ID );

			return ses;
		}

		/// <summary>Add a new session to the user</summary>
		public Session Insert( Guid userid, string salt, string domain, string ip = null )
        {
			if ( String.IsNullOrWhiteSpace( salt ) || userid == Guid.Empty || String.IsNullOrWhiteSpace( domain ) )
				return null;

			if ( String.IsNullOrWhiteSpace( ip ) )
				ip = null;

			domain = domain.Trim().ToLowerInvariant();

			Session ses = new Session { Salt = salt, Domain = domain, IPAddress = ip?.Trim() };

			IMongoCollection<User> coll = this.Database.GetCollection<User>( this.Table );
			FilterDefinition<User> filter = Builders<User>.Filter.Where( u => u.ID == userid );
			UpdateDefinition<User> update = Builders<User>.Update.AddToSet( "sessions", ses ).Set( "updated", DateTime.UtcNow );
			UpdateResult result = coll.UpdateOne( filter, update );

			return ( result.ModifiedCount > 0 ) ? ses : null;
		}

		/// <summary></summary>
        /// <param name="id">Session ID to update</param>
		public Session Refresh( Guid id )
        {
			if ( id == Guid.Empty )
				return null;

			Session current = this.Get( id );

			if ( current == null )
				return null;

			DateTime to = DateTimeOffset.UtcNow.DateTime;

			IMongoCollection<User> coll = this.Database.GetCollection<User>( this.Table );
			var filter = Builders<User>.Filter.Where( u => u.ID == current.UserID );
			filter &= Builders<User>.Filter.ElemMatch( u => u.Sessions, s => s.ID == current.ID  );
			UpdateDefinition<User> update = Builders<User>.Update.Set( u => u.Sessions[-1].LastModified, to );
			//var ops = new FindOneAndUpdateOptions<User>() { ReturnDocument = ReturnDocument.After };
			UpdateResult result = coll.UpdateOne( filter, update );

			if ( result.ModifiedCount < 1 )
				return null;

			current.LastModified = to;
			return current;
		}
    }
}
