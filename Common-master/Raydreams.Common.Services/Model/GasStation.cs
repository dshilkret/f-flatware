using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Raydreams.Common.Services.Model
{
	public class GasStation : ITableEntity
	{
		public string PartitionKey { get; set; }

		public string RowKey { get; set; }

		public DateTimeOffset Timestamp { get; set; }

		public string ETag { get; set; }

		[JsonProperty( PropertyName = "vendor" )]
		public string Vendor { get; set; }

		[JsonProperty( PropertyName = "address" )]
		public string Address { get; set; }

		[JsonProperty( PropertyName = "city" )]
		public string City { get; set; }

		[JsonProperty( PropertyName = "state" )]
		public string State { get; set; }

		[JsonProperty( PropertyName = "zip" )]
		public string Zip { get; set; }

		[JsonProperty( PropertyName = "latitude" )]
		public double Latitude { get; set; }

		[JsonProperty( PropertyName = "longitude" )]
		public double Longitude { get; set; }

		[JsonProperty( PropertyName = "secure" )]
		public bool Secure { get; set; }

		public void ReadEntity( IDictionary<string, EntityProperty> props, OperationContext operationContext )
		{
			this.Vendor = props["Vendor"].StringValue;
			this.Address = props["Address"].StringValue;
			this.City = props["City"].StringValue;
			this.State = props["State"].StringValue;
			this.Zip = props["Zip"].StringValue;
			this.Secure = (props["Secure"].BooleanValue == null) ? false : (Boolean)props["Secure"].BooleanValue;
			this.Latitude = (props["Latitude"].DoubleValue == null) ? 0 : (Double)props["Latitude"].DoubleValue;
			this.Longitude = (props["Longitude"].DoubleValue == null) ? 0 : (Double)props["Longitude"].DoubleValue;
		}

		public IDictionary<string, EntityProperty> WriteEntity( OperationContext operationContext )
		{
			throw new NotImplementedException();
		}
	}
}
