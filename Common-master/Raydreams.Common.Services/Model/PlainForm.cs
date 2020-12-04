using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Raydreams.Common.Services.Model
{
    /// <summary></summary>
    public class PlainForm
    {
        public PlainForm()
        {
            this.Fields = new string[0];
            this.Format = "base64";
        }

        /// <summary>name of the form</summary>
        [JsonProperty( "name" )]
        public string Name { get; set; }

        /// <summary>Fields to encrypt</summary>
        [JsonProperty( "fields" )]
        public string[] Fields { get; set; }

        /// <summary>the actual form data</summary>
        [JsonProperty( "formData" )]
        public dynamic FormData { get; set; }

        /// <summary>Format such as byte, base64 or hex</summary>
        [JsonProperty( "format" )]
        public string Format { get; set; }
    }

    /// <summary>The encrypted response</summary>
    public class EncryptedForm
    {
        public EncryptedForm()
        {
            this.ID = Guid.NewGuid();
            this.FormData = new Dictionary<string, string>();
            this.IV = new byte[0];
            this.Fields = new string[0];
        }

        /// <summary>ID of data entry</summary>
        [BsonId()]
        [BsonElement( "_id" )]
        [JsonProperty( "id" )]
        public Guid ID { get; set; }

        /// <summary>name of the form</summary>
        [JsonProperty( "name" )]
        public string Name { get; set; }

        /// <summary>Fields to encrypt</summary>
        [JsonProperty( "fields" )]
        public string[] Fields { get; set; }

        /// <summary>the actual form data</summary>
        [JsonProperty( "formData" )]
        public Dictionary<string, string> FormData { get; set; }

        /// <summary>Format such as byte, base64 or hex</summary>
        [JsonProperty( "format" )]
        public string Format { get; set; }

        /// <summary></summary>
        [JsonProperty( "iv" )]
        public byte[] IV { get; set; }
    }
}
