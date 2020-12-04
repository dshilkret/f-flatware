using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Raydreams.Common.Services.Model
{
    /// <summary></summary>
    [BsonIgnoreExtraElements()]
    public class ContentTag
    {
        /// <summary>Primary ID</summary>
        [BsonId()]
        [BsonElement( "_id" )]
        [JsonProperty( "id" )]
        public string ID { get; set; }

        /// <summary></summary>
        [BsonElement( "pageID" )]
        [JsonProperty( "pageID" )]
        public int PageID { get; set; }

        /// <summary></summary>
        [BsonElement( "value" )]
        [JsonProperty( "value" )]
        public string Value { get; set; }

        /// <summary></summary>
        [BsonElement( "active" )]
        [JsonProperty( "active" )]
        public bool Active { get; set; }
    }
}
