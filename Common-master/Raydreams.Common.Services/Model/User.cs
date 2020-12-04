using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Raydreams.Common.Services.Model
{
	/// <summary>Enumerates possible domain roles</summary>
	public enum RoleLevel
	{
		/// <summary>Lowest level read only access</summary>
		[Description( "user" )]
		User = 0,
		/// <summary>Basic read/write user who can edit page content</summary>
		[Description( "editor" )]
		Editor = 10,
		/// <summary>Powerful user in the site who can modify most everything</summary>
		[Description( "admin" )]
		Admin = 90,
	}

	/// <summary>The basic details the clients needs to know about the User</summary>
	/// <remarks>This may be sent back after login or GetUser type methods</remarks>
	public class UserInfo
	{
		/// <summary>The User's identifier</summary>
		[JsonProperty( "id" )]
		public string ID { get; set; }

		/// <summary>The User's identifier</summary>
		[JsonProperty( "name" )]
		public string Name { get; set; }

		/// <summary></summary>
		[JsonProperty( "email" )]
		public string Email { get; set; }

		/// <summary>The User's role only in this domain</summary>
		[JsonProperty( "role" )]
		[JsonConverter( typeof( StringEnumConverter ) )]
		public RoleLevel Role { get; set; }
	}

	/// <summary>The main full user entity</summary>
	[BsonIgnoreExtraElements]
	public class User
	{
		public User()
		{
			this.ID = Guid.NewGuid();
			this.ResetRequests = new List<ResetRequest>();
			this.FailedLogins = new List<DateTime>();
			this.Sessions = new List<Session>();
		}

		/// <summary>The DB ID</summary>
		[BsonId()]
		[BsonElement( "_id" )]
		[BsonGuidRepresentation( GuidRepresentation.Standard )]
		[JsonProperty( "id" )]
		public Guid ID { get; set; }

		/// <summary>user selected string ID</summary>
		[BsonElement( "userID" )]
		[JsonProperty( "userID" )]
		public string UserID { get; set; }

		[BsonElement( "name" )]
		[JsonProperty( "name" )]
		public string DisplayName { get; set; }

		[BsonElement( "email" )]
		[JsonProperty( "email" )]
		public string Email { get; set; }

		[BsonElement( "roles" )]
		[JsonProperty( "roles" )]
		public List<DomainRole> Roles { get; set; }

		/// <summary>When true the user must reset their password</summary>
		[BsonElement( "resetPW" )]
		public bool ResetPassword { get; set; }

		/// <summary>The date the user last login in successfully, otherwise null</summary>
		[BsonElement( "lastLogin" )]
		[BsonDateTimeOptions( Kind = DateTimeKind.Utc )]
		public DateTime? LastLogin { get; set; }

		[BsonElement( "created" )]
		[BsonDateTimeOptions( Kind = DateTimeKind.Utc )]
		public DateTime Created { get; set; }

		[BsonElement( "updated" )]
		[BsonDateTimeOptions( Kind = DateTimeKind.Utc )]
		public DateTime Updated { get; set; }

		/// <summary>Global enabled flag. When false the user can never log in</summary>
		[BsonElement( "enabled" )]
		public bool Enabled { get; set; }

		[BsonElement( "fails" )]
		public List<DateTime> FailedLogins { get; set; }

		[BsonElement( "resets" )]
		[JsonIgnore]
		public List<ResetRequest> ResetRequests { get; set; }

		[BsonElement( "sessions" )]
		[JsonIgnore]
		public List<Session> Sessions { get; set; }

		//[BsonElement("prefs")]
		//[JsonProperty("prefs")]
		//public UserPreferences Preferences { get; set; }

		#region [ Convenience Methods ]

		/// <summary>Gets a current session by the Session ID</summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public Session GetSession( Guid id )
		{
			if ( id == Guid.Empty )
				return null;

			return this.Sessions.Where( s => s.ID == id ).FirstOrDefault();
		}

		/// <summary>Gets the users role in a specific domain</summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public DomainRole GetRole( string domain )
		{
			if ( String.IsNullOrWhiteSpace( domain ) )
				return null;

			return this.Roles.Where( r => r.Domain.Equals( domain.Trim(), StringComparison.CurrentCultureIgnoreCase ) ).FirstOrDefault();
		}

		#endregion [ Convenience Methods ]
	}

	/// <summary>Internal Session Object</summary>
	/// <remarks>Never return back to the client as is.</remarks>
	[BsonIgnoreExtraElements]
	public class Session
	{
		public Session()
		{
			this.ID = Guid.NewGuid();
			this.Created = DateTime.UtcNow;
			this.LastModified = DateTime.UtcNow;
			this.Salt = "00000000";
		}

		/// <summary>The token ID</summary>
		//[BsonId()]
		[BsonElement( "id" )]
		[BsonGuidRepresentation( GuidRepresentation.Standard )]
		public Guid ID { get; set; }

		/// <summary>Reference back to the parent User ID for convenience</summary>
		//[BsonElement( "user" )]
		//[BsonGuidRepresentation( GuidRepresentation.Standard )]
		[BsonIgnore]
		public Guid UserID { get; set; }

		/// <summary>A random code created with each sesssion to increase complexity.</summary>
		[BsonElement( "salt" )]
		public string Salt { get; set; }

		/// <summary>The DateTime the session was initally created.</summary>
		[BsonElement( "created" )]
		[BsonDateTimeOptions( Kind = DateTimeKind.Utc )]
		public DateTime Created { get; set; }

		/// <summary>The last time this record was changed in any way. When a session update occured.</summary>
		[BsonElement( "updated" )]
		[BsonDateTimeOptions( Kind = DateTimeKind.Utc )]
		public DateTime LastModified { get; set; }

		/// <summary>WHat domain is this token for</summary>
		[BsonElement( "domain" )]
		[BsonRepresentation( BsonType.String )]
		public string Domain { get; set; }

		/// <summary>The IP Address of the session user</summary>
		[BsonElement( "ip" )]
		public string IPAddress { get; set; }
	}

	/// <summary>User domain role child object</summary>
	[BsonIgnoreExtraElements]
	public class DomainRole
	{
		public DomainRole()
		{
			this.Role = RoleLevel.User;
		}

		[BsonElement( "domain" )]
		[JsonProperty( "domain" )]
		public string Domain { get; set; }

		[BsonElement( "role" )]
		[JsonProperty( "role" )]
		[JsonConverter( typeof( StringEnumConverter ) )]
		public RoleLevel Role { get; set; }
	}

	/// <summary>User password reset request child object</summary>
	/// <remarks>embed as part of a user object but allow for different domain</remarks>
	public class ResetRequest
	{
		/// <summary>The code sent to the user to to confirm the reset</summary>
		[BsonElement( "code" )]
		public string Code { get; set; }

		/// <summary>The DateTime the reset was initally created</summary>
		[BsonElement( "created" )]
		[BsonDateTimeOptions( Kind = DateTimeKind.Utc )]
		public DateTime Created { get; set; }
	}

	/// <summary>Additional User metadata</summary>
	/// <remarks>Anything here gets sent back to the client so be careful</remarks>
	[BsonIgnoreExtraElements]
	public class UserPreferences
	{
		/// <summary>Artifically add in a user identifier</summary>
		[JsonProperty( "userName" )]
		public string UserName { get; set; }

		/// <summary>FinHub API Key</summary>
		[BsonElement( "finKey" )]
		[JsonProperty( "finKey" )]
		public string FinKey { get; set; }

		/// <summary>Max number of portfolios a user can have</summary>
		[BsonElement( "maxList" )]
		[JsonProperty( "maxList" )]
		public int MaxList { get; set; }

		/// <summary>Max stock items allowed per list</summary>
		[BsonElement( "maxPerList" )]
		[JsonProperty( "maxPerList" )]
		public int MaxPerList { get; set; }
	}

}

