using System;
using MongoDB.Driver;
using System.Collections.Generic;
using Raydreams.Common.Data;
using Raydreams.Common.Services.Model;

namespace Raydreams.Common.Services.Data
{
	/// <summary>For CRUD operations on the User physical data source.</summary>
	public class ResetRequestsRepository : MongoDataManager<User,Guid>, IResetRequestsRepository
	{
		#region [Fields]

		private string _table = "Users";

		#endregion [Fields]

		#region [Constructors]

		/// <summary>Constrcutor with a password maker</summary>
		public ResetRequestsRepository( string connStr, string db, string table ) : base( connStr, db )
		{
			this.Table = table;
		}

		#endregion [Constructors]

		#region [Properties]

		/// <summary>The physical Collection name</summary>
		public string Table
		{
			get { return this._table; }
			protected set { if ( !String.IsNullOrWhiteSpace( value ) ) this._table = value.Trim(); }
		}

		#endregion [Properties]

		#region [Methods]

		/// <summary>Insert new request</summary>
		/// <returns></returns>
		public bool InsertResetRequest( Guid id, string code )
        {
			if ( String.IsNullOrWhiteSpace( code ) || id == Guid.Empty )
				return false;

			ResetRequest req = new ResetRequest() { Code = code.Trim(), Created = DateTime.UtcNow };

			IMongoCollection<User> coll = this.Database.GetCollection<User>( this.Table );
			FilterDefinition<User> filter = Builders<User>.Filter.Eq( "_id", id );
			UpdateDefinition<User> update = Builders<User>.Update.AddToSet( "resets", DateTime.UtcNow ).Set( "updated", DateTime.UtcNow );

			UpdateResult result = coll.UpdateOne( filter, update );

			return ( result.ModifiedCount > 0 );
		}

		/// <summary></summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public bool ClearResetRequests( Guid id )
		{
			if ( id == Guid.Empty )
				return false;

			IMongoCollection<User> collection = this.Database.GetCollection<User>( this.Table );
			FilterDefinition<User> filter = Builders<User>.Filter.Eq( "_id", id );
			UpdateDefinition<User> update = Builders<User>.Update.Set( "resets", new List<ResetRequest>() ).Set( "updated", DateTime.UtcNow );

			UpdateResult result = collection.UpdateOne( filter, update );

			return ( result.ModifiedCount > 0 );
		}

		/// <summary>Removes all requests whose created are older by >= the specified timespan</summary>
		/// <returns>Sessions removed</returns>
		public long DeleteExpiredResetRequests( TimeSpan ts )
		{
			if ( ts.Ticks < 0 )
				ts = new TimeSpan( 0 );

			DateTime now = DateTime.UtcNow.Subtract( ts );

			IMongoCollection<User> coll = this.Database.GetCollection<User>( this.Table );
			FilterDefinition<User> filter = Builders<User>.Filter.AnyLte( "resets.created", now );
			UpdateDefinition<User> update = Builders<User>.Update.PullFilter( "resets", Builders<ResetRequest>.Filter.Lte( "created", now ) ).Set( "updated", DateTime.UtcNow );

			UpdateResult result = coll.UpdateMany( filter, update );

			return result.ModifiedCount;
		}

		#endregion [Methods]
	}
}
