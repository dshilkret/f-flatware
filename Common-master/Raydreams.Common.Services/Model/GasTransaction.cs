using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Raydreams.Common.Services.Model
{
    [BsonIgnoreExtraElements]
    public class GasTransaction
    {
        public GasTransaction()
        {
			this.ID = Guid.NewGuid();
			this.TransactionDate = DateTime.UtcNow;
		}

		[BsonId()]
		[BsonElement( "_id" )]
		[JsonProperty("id")]
		public Guid ID { get; set; }

		/// <summary></summary>
		[BsonElement( "station" )]
		[JsonProperty( "station" )]
		public string Station { get; set; }

		/// <summary></summary>
		[BsonElement( "location" )]
		[JsonProperty( "location" )]
		public string Location { get; set; }

		/// <summary>The DateTime the session was initally created.</summary>
		[BsonElement( "transactionDate" )]
		[BsonDateTimeOptions( Kind = DateTimeKind.Utc )]
		[JsonProperty( "transactionDate" )]
		public DateTime TransactionDate { get; set; }

		/// <summary>Price Per Gallon</summary>
		[BsonElement( "price" )]
		[JsonProperty( "price" )]
		public double Price { get; set; }

		/// <summary></summary>
		[BsonElement( "gallons" )]
		[JsonProperty( "gallons" )]
		public double Gallons { get; set; }

		/// <summary></summary>
		[JsonProperty( "total" )]
		public double Total
        {
			get
            {
				return this.Price * this.Gallons;
            }
        }

		/// <summary></summary>
		[BsonElement( "totalMiles" )]
		[JsonProperty( "totalMiles" )]
		public int TotalMiles { get; set; }

		/// <summary></summary>
		[BsonElement( "octane" )]
		[JsonProperty( "ocatane" )]
		public int Octane { get; set; }
	}
}
