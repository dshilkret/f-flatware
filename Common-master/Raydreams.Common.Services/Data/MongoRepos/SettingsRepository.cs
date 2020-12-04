using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using Raydreams.Common.Data;

namespace Raydreams.Common.Services.Data
{
	/// <summary>For CRUD operations on the Settings physical data source.</summary>
	public class SettingsRepository<T> : MongoDataManager<T, string>
	{
		#region [Fields]

		private string _table = "Settings";

		#endregion [Fields]

		#region [Constructors]

		/// <summary></summary>
		public SettingsRepository( string connStr, string db, string table ) : base( connStr, db )
		{
			this.Table = table;
		}

		#endregion [Constructors]


		#region [Properties]

		/// <summary></summary>
		public string Table
		{
			get { return this._table; }
			protected set { if ( !String.IsNullOrWhiteSpace( value ) ) this._table = value.Trim(); }
		}

		#endregion [Properties]

		#region [Methods]

		/// <summary>Gets the settings for the specific domain</summary>
		/// <returns>Returns the domain settings</returns>
		public T GetByDomain( string domain )
		{
			if ( String.IsNullOrWhiteSpace( domain ) )
				return default;

			domain = domain.Trim().ToLowerInvariant();

			List<T> results = null;

			try
			{
				IMongoCollection<T> collection = this.Database.GetCollection<T>( this.Table );
				FilterDefinition<T> filter = Builders<T>.Filter.Eq( "_id", domain );
				results = collection.Find<T>( filter ).ToList();
			}
			catch
			{
				return default;
			}

			return ( results == null || results.Count < 1 ) ? default : results[0];
		}

		#endregion [Methods]
	}
}

