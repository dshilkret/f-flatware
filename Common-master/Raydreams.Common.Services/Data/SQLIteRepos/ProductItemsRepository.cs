using System;
using System.Collections.Generic;
using Raydreams.Common.Data;
using Raydreams.Common.Services.Model;

namespace Raydreams.Common.Services.Data
{
    public class ProductItemsRepository : SQLiteDataManager
    {
        #region [Fields]

        private static readonly string _selectLogs = "SELECT * FROM {{Table}}";

        private static readonly string _cleanse = "DELETE FROM {{Table}} WHERE [Timestamp] < @expire";

        private string _tableName = null;

        #endregion [Fields]

        #region [Constructors]

        /// <summary></summary>
        /// <param name="connStr">The Connection Str to the DB</param>
        public ProductItemsRepository(string connStr) : base(connStr)
        {
        }

        #endregion [Constructors]

        #region [Properties]

        /// <summary></summary>
        public string TableName
        {
            get { return this._tableName; }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                    this._tableName = value.Trim().ToLower();
            }
        }

        #endregion [Properties]

        public List<ProductItem> SelectAll()
        {
            return this.SelectAll<ProductItem>(this.TableName);
        }
    }
}
