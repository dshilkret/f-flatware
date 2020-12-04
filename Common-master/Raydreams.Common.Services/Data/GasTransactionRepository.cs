using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using Raydreams.Common.Data;
using Raydreams.Common.Services.Model;

namespace Raydreams.Common.Services.Data
{
    public class GasTransactionRepository : MongoDataManager<GasTransaction, Guid>
    {
        #region [Fields]

        private string _table = "GasTransactions";

        #endregion [Fields]

        public GasTransactionRepository(string connStr, string db, string table) : base( connStr, db )
        {
            this.Table = table;
        }

        #region [Properties]

        /// <summary></summary>
        public string Table
        {
            get { return this._table; }
            protected set { if (!String.IsNullOrWhiteSpace( value )) this._table = value.Trim(); }
        }

        #endregion [Properties]

        /// <summary>Insert new request</summary>
        /// <returns></returns>
        public bool Insert(GasTransaction trans)
        {
            if (trans == null)
                return false;

            return base.Insert( trans, this.Table );
        }

    }
}
