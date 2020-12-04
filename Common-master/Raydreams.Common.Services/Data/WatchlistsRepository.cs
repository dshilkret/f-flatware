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
 //   public class WatchlistsRepository : MongoDataManager<Watchlist, Guid>
 //   {
	//	#region [Fields]

	//	private string _table = "Watchlists";

	//	#endregion [Fields]

	//	#region [Constructors]

	//	/// <summary></summary>
	//	public WatchlistsRepository(string connStr, string db, string table) : base(connStr, db)
	//	{
	//		this.Table = table;
	//	}

	//	#endregion [Constructors]

	//	#region [Properties]

	//	/// <summary>The physical Collection name</summary>
	//	public string Table
	//	{
	//		get { return this._table; }
	//		protected set { if (!String.IsNullOrWhiteSpace(value)) this._table = value.Trim(); }
	//	}

	//	#endregion [Properties]

	//	#region [Methods]

	//	/// <summary>Gets every user account</summary>
	//	/// <returns></returns>
	//	public Watchlist GetByUser(Guid userID)
	//	{
	//		if ( userID == Guid.Empty)
	//			return null;

	//		IMongoCollection<Watchlist> collection = this.Database.GetCollection<Watchlist>(this.Table);
	//		List<Watchlist> results = collection.Find<Watchlist>(p => p.User == userID).ToList();

	//		return (results != null && results.Count > 0) ? results[0] : null;
	//	}

	//	/// <summary>Gets every user account</summary>
	//	/// <returns></returns>
	//	public bool Insert(Watchlist list)
	//	{
	//		if (list == null)
	//			return false;

	//		return base.Insert(list, this.Table);
	//	}

	//	#endregion [Methods]
	//}
}
