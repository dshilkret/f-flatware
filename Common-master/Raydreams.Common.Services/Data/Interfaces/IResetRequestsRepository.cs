using System;

namespace Raydreams.Common.Services.Data
{
    /// <summary></summary>
    public interface IResetRequestsRepository
    {
        /// <summary></summary>
        bool InsertResetRequest( Guid id, string code );

        /// <summary></summary>
        bool ClearResetRequests( Guid id );

        /// <summary></summary>
        long DeleteExpiredResetRequests( TimeSpan ts );
    }
}
