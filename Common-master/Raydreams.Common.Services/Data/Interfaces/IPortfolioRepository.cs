using System;
using System.Collections.Generic;
using Raydreams.Common.Services.Model;

namespace Raydreams.Common.Services.Data
{
    public interface IPortfolioRepository
    {
        /// <summary></summary>
        List<Portfolio> GetAllByUser( Guid user );

        /// <summary></summary>
        Portfolio GetByUser( Guid user, string name );

        /// <summary></summary>
        List<LookupPair> GetNamesByUser( Guid user );

        /// <summary></summary>
        StockWatch GetWatched( Guid user, string name, string symbol );

        /// <summary></summary>
        int InsertOrUpdateWatched( Guid user, string name, StockWatch stock );

        /// <summary></summary>
        bool DeleteWatched( Guid user, string name, string symbol );

        /// <summary></summary>
        bool Insert( Portfolio list );
    }
}
