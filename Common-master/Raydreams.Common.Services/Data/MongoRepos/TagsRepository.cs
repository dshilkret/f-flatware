using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using Raydreams.Common.Data;
using Raydreams.Common.Services.Model;

namespace Raydreams.Common.Services.Data
{
    public class TagsRepository : MongoDataManager<ContentTag, string>
	{
		#region [Fields]

		private string _table = "Tags";

		#endregion [Fields]

		#region [Constructors]

		/// <summary></summary>
		public TagsRepository( string connStr, string db, string table ) : base( connStr, db )
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

		/// <summary>Gets every user account</summary>
		/// <returns></returns>
		public List<ContentTag> GetAll( bool activeOnly = true )
		{
			List<ContentTag> results = base.GetAll( this.Table );

			if ( activeOnly )
				return results.Where( t => t.Active ).ToList();

			return results;
		}
	}
}
