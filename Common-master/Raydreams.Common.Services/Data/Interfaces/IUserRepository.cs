using System;
using System.Collections.Generic;
using Raydreams.Common.Services.Model;

namespace Raydreams.Common.Services.Data
{
    public interface IUsersRepository
    {
        /// <summary></summary>
        List<User> GetAllByDomain( string domain, bool enabledOnly = true );

        /// <summary></summary>
        User Get( Guid id );

        /// <summary></summary>
        User GetByUserID( string id );

        /// <summary></summary>
        string GetHashedPWByUserID( string userID );

        /// <summary></summary>
        User GetByLogin( string userid, string pwhash );

        /// <summary>Returns the user by an active session ID</summary>
        User GetBySessionID( Guid id );

        /// <summary></summary>
        User GetByResetRequest( string code );

        //UserPreferences GetPreferences( Guid oid );

        /// <summary></summary>
        (Guid ID, string PlainPW) Insert( string userID, string name, string email, DomainRole role );

        /// <summary></summary>
        bool Update( Guid oid, string name, string email );

        /// <summary></summary>
        bool UpdatePassword( Guid id, string newPW );

        /// <summary></summary>
        string ResetPassword( Guid id );

        /// <summary></summary>
        bool Disable( Guid id );

        /// <summary></summary>
        bool InsertRole( Guid id, DomainRole role );

        /// <summary></summary>
        bool UpdateLastLogin( Guid id );

        /// <summary></summary>
        bool ClearFailedLogins( Guid id );

        /// <summary></summary>
        bool InsertFailedLogin( Guid id );

        /// <summary>Completely deletes a user.</summary>
        bool Delete( Guid id );
    }
}
