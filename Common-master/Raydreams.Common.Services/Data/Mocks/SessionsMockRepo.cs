using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using Raydreams.Common.Services.Model;

namespace Raydreams.Common.Services.Data
{
    /// <summary>Mocks sessions repo</summary>
    public class SessionsMockRepo : ISessionsRepository
    {
        public List<Session> Data = new List<Session>();

        public SessionsMockRepo()
        {
            this.Data = MockData.Sessions;
        }

        /// <summary></summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public bool Delete( Guid oid )
        {
            //if ( oid == ObjectId.Empty )
            //    return false;

            //Session s = this.Get( oid );

            //if ( s == null )
            //    return false;

            //return this.Data.Remove(s);
            return true;
        }

        /// <summary></summary>
        public long DeleteByUser( Guid userid, string domain )
        {
            //long count = 0;

            //List<Session> all = this.Data.Where( i => i.UserID == userid && i.Domain == domain ).ToList();

            //foreach ( Session s in all )
            //{
            //    this.Data.Remove( s );
            //    ++count;
            //}

            //return count;

            return 0;
        }

        /// <summary></summary>
        public long DeleteExpired( TimeSpan ts, string domain )
        {
            // never delete the mock sessions
            return 0;
        }

        /// <summary></summary>
        public Session Get( string id )
        {
            Guid oid = Guid.Parse( id );

            return this.Get(oid);
        }

        /// <summary></summary>
        public Session Get( Guid oid )
        {
            return this.Data.Where( i => i.ID == oid ).FirstOrDefault();
        }

        /// <summary></summary>
        public List<Session> GetByUser( Guid userid, string domain )
        {
            return this.Data.Where( i => i.UserID == userid && i.Domain == domain ).ToList();
        }

        /// <summary></summary>
        /// <remarks>All mock sessions must be preloaded so the ID remains the same</remarks>
        public Session Insert( Guid userid, string salt, string domain, string ip = null )
        {
            //Session s = new Session { UserID = userid, LastModified = DateTime.UtcNow, Created = DateTime.UtcNow, Salt = salt, Domain = domain, IPAddress = ip };

            //this.Data.Add( s );

            List<Session> s = GetByUser( userid, domain );

            return s[0];
        }

        /// <summary></summary>
        public Session Refresh( Guid oid )
        {
            if ( oid == Guid.Empty )
                return null;

            Session s = this.Get( oid );

            if ( s == null )
                return null;

            s.LastModified = DateTime.UtcNow;

            return s;
        }
    }
}
