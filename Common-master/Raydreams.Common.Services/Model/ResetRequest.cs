using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Raydreams.Common.Services.Model
{
	/// <summary></summary>
    /// <remarks>embed as part of a user object but allow for different domain</remarks>
	//public class ResetRequest
	//{
	//	/// <summary>ID</summary>
	//	[BsonId()]
	//	[BsonElement( "_id" )]
	//	[JsonProperty( "id" )]
	//	public Guid ID { get; set; }

	//	/// <summary>The code sent to the user to to confirm the reset</summary>
	//	[BsonElement( "code" )]
	//	public string Code { get; set; }

	//	/// <summary>The ID of the user</summary>
	//	[BsonElement( "userID" )]
	//	public string UserID { get; set; }

	//	/// <summary>The DateTime the reset was initally created</summary>
	//	[BsonElement( "created" )]
	//	[BsonDateTimeOptions( Kind = DateTimeKind.Utc )]
	//	public DateTime Created { get; set; }

	//	/// <summary>Identifies the session's domain</summary>
	//	/// <summary></summary>
	//	[BsonElement( "domain" )]
	//	[BsonRepresentation( BsonType.String )]
	//	public string Domain { get; set; }
	//}
}
