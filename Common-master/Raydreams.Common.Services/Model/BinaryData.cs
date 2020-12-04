using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Raydreams.Common.Services.Model
{
    public class BinaryData
    {
		[BsonId()]
		[BsonElement( "_id" )]
		public ObjectId ID { get; set; }

		[BsonElement( "data" )]
		public byte[] Data { get; set; }

		[BsonElement( "encField" )]
		public byte[] EncryptedField { get; set; }
	}
}
