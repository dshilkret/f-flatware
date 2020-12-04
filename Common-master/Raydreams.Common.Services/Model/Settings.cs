using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Raydreams.Common.Services.Model
{
    /// <summary></summary>
    [BsonIgnoreExtraElements]
    public class Settings
    {
        [BsonId()]
        [BsonElement( "_id" )]
        [JsonProperty( "id" )]
        public ObjectId ID { get; set; }

        [BsonElement( "sessionTimeout" )]
        [JsonProperty( "sessionTimeout" )]
        public int SessionTimeout { get; set; }

        [BsonElement( "imagesCDN" )]
        [JsonProperty( "imagesCDN" )]
        public string ImagesCDN { get; set; }
    }
}
