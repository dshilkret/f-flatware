using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Raydreams.Common.Data;
using Raydreams.Common.Utils;

namespace Raydreams.Common.Services.Model
{
    /// <summary></summary>
    public class ProductItem : ITableEntity
    {
        /// <summary></summary>
        private long _id;

        /// <summary></summary>
        private string _line;

        /// <summary></summary>
        private DateTimeOffset _ts = DateTimeOffset.UtcNow;

        /// <summary>Marker Match ID</summary>
        [FieldSource( "id" )]
        [JsonProperty( "id" )]
        public long ID
        {
            get
            {
                return this._id;
            }
            set
            {
                this._id = value;
            }
        }

        /// <summary></summary>
        [FieldSource( "line" )]
        [JsonProperty( "line" )]
        public string Line
        {
            get
            {
                return this._line;
            }
            set
            {
                this._line = value;
            }
        }

        /// <summary></summary>
        [FieldSource( "code" )]
        [JsonProperty( "code" )]
        public string Code { get; set; }

        /// <summary></summary>
        [FieldSource( "name" )]
        [JsonProperty( "name" )]
        public string Name { get; set; }

        /// <summary></summary>
        [FieldSource( "hex" )]
        [JsonProperty( "hex" )]
        public string Hex { get; set; }

        /// <summary></summary>
        [JsonIgnore]
        public Color Color { get; set; }

        /// <summary>Calculated Delta E value</summary>
        [JsonProperty( "deltaE" )]
        public double DeltaE { get; set; }

        /// <summary></summary>
        [FieldSource( "rgb" )]
        [JsonProperty( "rgb" )]
        public string RGB { get; set; }

        /// <summary></summary>
        [FieldSource( "cmyk" )]
        [JsonProperty( "cmyk" )]
        public string CMYK { get; set; }

        /// <summary>Lab Color values</summary>
        [JsonProperty( "lab" )]
        public double[] Lab { get; set; }

        /// <summary></summary>
        [JsonIgnore]
        public string PartitionKey
        {
            get
            {
                return this._line;
            }
            set
            {
                this._line = value;
            }
        }

        /// <summary></summary>
        [JsonIgnore]
        public string RowKey
        {
            get
            {
                return this._id.ToString();
            }
            set
            {
                this._id = Int64.Parse( value );
            }
        }

        /// <summary></summary>
        [JsonIgnore]
        public DateTimeOffset Timestamp
        { get { return this._ts; } set { this._ts = value; } }

        /// <summary></summary>
        [JsonIgnore]
        public string ETag { get; set; }

        /// <summary>/summary>
        public void ReadEntity( IDictionary<string, EntityProperty> props, OperationContext operationContext )
        {
            this.Code = props["code"].StringValue;
            this.Name = props["name"].StringValue;
            this.Lab = ByteUtil.BytesToDoubles(props["lab"].BinaryValue);
            this.CMYK = props["cmyk"].StringValue;
            this.RGB = props["rgb"].StringValue;
            this.Hex = props["hex"].StringValue;
        }

        /// <summary></summary>
        /// <param name="operationContext"></param>
        /// <returns></returns>
        public IDictionary<string, EntityProperty> WriteEntity( OperationContext operationContext )
        {
            var props = new Dictionary<string, EntityProperty>();

            props["code"] = new EntityProperty( this.Code );
            props["name"] = new EntityProperty( this.Name );
            props["lab"] = new EntityProperty( ByteUtil.DoublesToBytes( this.Lab ) );
            props["cmyk"] = new EntityProperty( this.CMYK ?? String.Empty );
            props["rgb"] = new EntityProperty( this.RGB ?? String.Empty );
            props["hex"] = new EntityProperty( this.Hex ?? String.Empty );

            return props;
        }
    }
}
