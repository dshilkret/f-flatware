using System;
using System.Collections.Generic;
using System.Linq;
using Raydreams.Common.Services.Model;

namespace Raydreams.Common.Services.Data
{
    public class UsersMockRepo : IUsersRepository
    {
        public List<User> Data = new List<User>();
        public Dictionary<Guid, string> PWs = new Dictionary<Guid, string>();

        public UsersMockRepo()
        {
            this.Data = MockData.Users;
            this.PWs = MockData.PWs;
        }

        public bool Delete( Guid id )
        {
            throw new NotImplementedException();
        }

        public bool Disable( Guid oid )
        {
            throw new NotImplementedException();
        }

        public User Get( Guid oid )
        {
            return this.Data.Where( i => i.ID == oid ).FirstOrDefault();
        }

        public List<User> GetAll( bool enabledOnly = true )
        {
            return Data;    
        }

        public List<User> GetAllByDomain( string domain, bool enabledOnly = true )
        {
            return null;
        }

        public User GetByLogin( string userid, string pwhash )
        {
            var pw = this.PWs.Where( i => i.Value == pwhash ).FirstOrDefault();
            User u = this.Get( pw.Key );

            return u ?? null;
        }

        public User GetByUserID( string id )
        {
            return this.Data.Where( i => i.UserID == id ).FirstOrDefault(); 
        }

        public string GetHashedPWByUserID( string userID )
        {
            User user = this.GetByUserID( userID );

            return this.PWs[user.ID];
        }

        /// <summary>Returns the user by an active session ID</summary>
        public User GetBySessionID( Guid id )
        {
            return null;
        }

        public /// <summary></summary>
        User GetByResetRequest( string code )
        {
            return null;
        }

        //public UserPreferences GetPreferences( Guid oid )
        //{
        //    throw new NotImplementedException();
        //}

        public (Guid ID, string PlainPW) Insert( string userID, string name, string email, DomainRole role )
        {
            throw new NotImplementedException();
        }

        /// <summary></summary>
        public bool InsertRole( Guid id, DomainRole role )
        {
            return false;
        }

        public bool InsertFailedLogin( Guid oid )
        {
            User acct = this.Get( oid );

            acct.FailedLogins.Add( DateTimeOffset.UtcNow.DateTime );

            return true;
        }

        public bool ResetFailedLogins( Guid oid )
        {
            User acct = this.Get( oid );

            acct.FailedLogins = new List<DateTime>();

            return true;
        }

        public string ResetPassword( Guid oid )
        {
            throw new NotImplementedException();
        }

        public bool Update( Guid oid, string name, string email, string role )
        {
            throw new NotImplementedException();
        }

        public bool Update( Guid oid, string name, string email )
        {
            return false;
        }

        public bool ClearFailedLogins( Guid id )
        {
            return false;
        }

        public bool UpdateLastLogin( Guid id )
        {
            return false;
        }

        public bool UpdateLastLoginByOID( Guid id )
        {
            User acct = this.Get( id );

            acct.LastLogin = DateTimeOffset.UtcNow.DateTime;

            return true;
        }

        public bool UpdatePassword( Guid id, string newPW )
        {
            throw new NotImplementedException();
        }

        public bool UpdateRole( Guid oid, string role = "user" )
        {
            throw new NotImplementedException();
        }
    }
}
