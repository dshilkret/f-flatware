using System;
using MongoDB.Driver;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Raydreams.Common.Data;
using Raydreams.Common.Services.Model;
using Raydreams.Common.Services.Security;
using System.Linq;

namespace Raydreams.Common.Services.Data
{
    public class PortfoliosRepository : MongoDataManager<Portfolio, Guid>, IPortfolioRepository
	{
		#region [Fields]

		private string _table = "Stocks";

		#endregion [Fields]

		#region [Constructors]

		/// <summary></summary>
		public PortfoliosRepository(string connStr, string db, string table) : base(connStr, db)
		{
			this.Table = table;
		}

		#endregion [Constructors]

		#region [Properties]

		/// <summary>The physical Collection name</summary>
		public string Table
		{
			get { return this._table; }
			protected set { if (!String.IsNullOrWhiteSpace(value)) this._table = value.Trim(); }
		}

		#endregion [Properties]

		#region [Methods]

		/// <summary></summary>
		/// <returns></returns>
		public List<Portfolio> GetAllByUser( Guid user )
		{
			if (user == Guid.Empty)
				return null;

			IMongoCollection<Portfolio> col = this.Database.GetCollection<Portfolio>(this.Table);
			List<Portfolio> results = col.Find<Portfolio>(p => p.User == user).ToList();

			return results ?? new List<Portfolio>();
		}

		/// <summary></summary>
		/// <returns></returns>
		public Portfolio GetByUser(Guid user, string name)
		{
			if (user == Guid.Empty || String.IsNullOrWhiteSpace(name))
				return null;

			name = name.Trim();

			IMongoCollection<Portfolio> col = this.Database.GetCollection<Portfolio>(this.Table);
			List<Portfolio> results = col.Find<Portfolio>(p => p.User == user && p.Name == name ).ToList();

			return (results != null && results.Count > 0) ? results[0] : null;
		}

		/// <summary></summary>
		public List<LookupPair> GetNamesByUser( Guid user )
        {
			var results = new List<LookupPair>();

			if (user == Guid.Empty)
				return results;

			IMongoCollection <Portfolio> col = this.Database.GetCollection<Portfolio>(this.Table);
			var filter = Builders<Portfolio>.Filter.Where(p => p.User == user);
			List<Portfolio> query = col.Find<Portfolio>(p => p.User == user).ToList();

			if (query == null || query.Count < 0)
				return results;

			foreach (Portfolio doc in query)
            {
				results.Add(new LookupPair( doc.ID , doc.Name ));
            }

			return results.OrderBy( p => p.Value ).ToList();
		}

		/// <summary>Gets the specified watched stock</summary>
		/// <returns></returns>
		public StockWatch GetWatched(Guid user, string name, string symbol)
		{
			if ( user == Guid.Empty || String.IsNullOrWhiteSpace(name) || String.IsNullOrWhiteSpace(symbol))
				return null;

			symbol = symbol.Trim().ToUpperInvariant();
			name = name.Trim();

			IMongoCollection<Portfolio> col = this.Database.GetCollection<Portfolio>(this.Table);
			//var filter = Builders<Portfolio>.Filter.Regex("user", new BsonRegularExpression($"/^{userID}$/i"));
			var filter = Builders<Portfolio>.Filter.Where(p => p.User == user && p.Name == name);
			filter &= Builders<Portfolio>.Filter.Regex("watched.symbol", new BsonRegularExpression($"/^{symbol}$/i"));
			List<Portfolio> results = col.Find<Portfolio>(filter).ToList();

			if ( results == null || results.Count < 1 )
				return null;

			return results[0].Watchlist.Where(s => s.Symbol.ToUpper() == symbol).FirstOrDefault();
		}

		/// <summary>Inserts or udpates a new watched stock into the users portfolio</summary>
		/// <param name="userID"></param>
		/// <param name="symbol"></param>
		/// <param name="buyAlert"></param>
		/// <returns>-1 for input error, 0 if no data was changed, 1 if already exsited and was updated, 2 if was inserted as new</returns>
        /// <remarks>Needs a return structure</remarks>
		public int InsertOrUpdateWatched(Guid user, string name, StockWatch stock)
        {
			if (user == Guid.Empty || String.IsNullOrWhiteSpace(name) || stock == null || String.IsNullOrWhiteSpace(stock.Symbol) )
				return -1;

			if (stock.BuyAlert < 0)
				stock.BuyAlert = 0;

			name = name.Trim();

			StockWatch watched = this.GetWatched(user, name, stock.Symbol);

			IMongoCollection<Portfolio> col = this.Database.GetCollection<Portfolio>(this.Table);
			//var filter = Builders<Portfolio>.Filter.Regex("user", new BsonRegularExpression($"/^{userID}$/i"));
			var filter = Builders<Portfolio>.Filter.Where(p => p.User == user && p.Name == name);

			// insert
			if ( watched == null )
            {
				UpdateDefinition<Portfolio> update = Builders<Portfolio>.Update.AddToSet("watched", stock);
				UpdateResult result = col.UpdateOne(filter, update);
				return (result.ModifiedCount > 0) ? 2 : 0;
			}
			else if ( watched.BuyAlert != stock.BuyAlert ) // update price only
            {
				filter &= Builders<Portfolio>.Filter.ElemMatch(x => x.Watchlist, p => p.Symbol == watched.Symbol);
				UpdateDefinition<Portfolio> update = Builders<Portfolio>.Update.Set(c => c.Watchlist[-1].BuyAlert, stock.BuyAlert);
				UpdateResult result = col.UpdateOne(filter, update);
				return (result.ModifiedCount > 0) ? 1 : 0;
			}

			return 0;
        }

        /// <summary></summary>
        /// <param name="userID"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
		public bool DeleteWatched(Guid user, string name, string symbol)
		{
			if ( user == Guid.Empty || String.IsNullOrWhiteSpace(name) || String.IsNullOrWhiteSpace(symbol))
				return false;

			name = name.Trim();

			symbol = symbol.Trim().ToUpperInvariant();

			IMongoCollection<Portfolio> col = this.Database.GetCollection<Portfolio>(this.Table);

			//var filter = Builders<Portfolio>.Filter.Regex("user", new BsonRegularExpression($"/^{userID}$/i"));
			var filter = Builders<Portfolio>.Filter.Where( p => p.User == user && p.Name == name);
			filter &= Builders<Portfolio>.Filter.ElemMatch(x => x.Watchlist, p => p.Symbol.ToUpperInvariant() == symbol);

            UpdateDefinition<Portfolio> update = Builders<Portfolio>.Update.PullFilter("watched", Builders<StockWatch>.Filter.Eq("symbol", symbol ));

			UpdateResult result = col.UpdateOne(filter, update);
			return (result.ModifiedCount > 0);
		}

		/// <summary>Insert a new stock portfolio</summary>
		/// <returns></returns>
		public bool Insert(Portfolio port)
		{
			if (port == null || port.ID == Guid.Empty || String.IsNullOrWhiteSpace(port.Name) )
				return false;

			return base.Insert(port, this.Table);
		}

		#endregion [Methods]
	}
}
