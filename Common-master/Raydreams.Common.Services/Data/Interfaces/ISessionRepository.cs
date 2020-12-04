using System;
using System.Collections.Generic;
using MongoDB.Bson;
using Raydreams.Common.Services.Model;

namespace Raydreams.Common.Services.Data
{
	public interface ISessionsRepository
	{
		/// <summary>Get a session by its ID as a string</summary>
		Session Get( string id );

		/// <summary>Get a session</summary>
		Session Get( Guid id );

		/// <summary>Get a session by a user for a specific app</summary>
		List<Session> GetByUser( Guid userid, string domain );

		/// <summary>Delete all session for a specific user but only for the specified app</summary>
		long DeleteByUser( Guid userid, string domain );

		/// <summary>Delete a session</summary>
		bool Delete( Guid id );

		/// <summary>Insert a new sessoin</summary>
		Session Insert( Guid userid, string salt, string domain, string ip = null );

		/// <summary>refresh a session by updating last modified to now</summary>
		Session Refresh( Guid id );

		/// <summary>Remove any expired session based on the specified timespan</summary>
		long DeleteExpired( TimeSpan ts, string domain );
	}
}
