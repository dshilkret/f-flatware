using System;
using Raydreams.Common.Model;
using Raydreams.Common.Extensions;
using System.Collections.ObjectModel;

namespace Raydreams.Common.Services.Config
{
	/// <summary>Encapsulates the settings are various environments. These could be loaded from runtime config instead of hard coded</summary>
	public class EnvironmentSettings
	{
		public static readonly string DefaultDomain = "formencrypt.com";

		/// <summary></summary>
		public static EnvironmentSettings GetSettings( string type )
		{
			EnvironmentSettings set = null;

			// send the type to the other constructor
			if ( String.IsNullOrWhiteSpace( type ) )
				set = GetSettings( EnvironmentType.Unknown );
			else
				set = GetSettings( type.EnumByDescription<EnvironmentType>() );

			set.EnvironmentKey = type;
			return set;
		}

		/// <summary></summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static EnvironmentSettings GetSettings( EnvironmentType type )
		{
			if ( type == EnvironmentType.Production )
				return PROD;
			else if ( type == EnvironmentType.Development )
				return DEV;
			else
				return UNKNOWN;
		}

		#region [ Properties ]

		/// <summary>The enumerated environment type</summary>
		public EnvironmentType EnvironmentType { get; set; }

		/// <summary>The actual key used to load the environment</summary>
		public string EnvironmentKey { get; set; }

		/// <summary>The connection string to use to access the DB.</summary>
		public string ConnectionString { get; set; }

		/// <summary>The connection string to use to access the Azure Tables.</summary>
		public string AzureTableConnection { get; set; }

		/// <summary>The name of the database to use with the Connection String</summary>
		public string Database { get; set; }

		/// <summary>The number of minutes for x-timestamp to be acceppted into the past</summary>
		/// <remarks>1 minute is into the future to avoid mis-synced clocks</remarks>
		public int TimestampTimeout { get; set; }

		/// <summary>The password to use to generate a token key</summary>
		/// <remarks>This is NOT a PROD solution</remarks>
		public string TokenPassword { get; set; }

		#endregion [ Properties ]

		/// <summary>Unknown environment settings</summary>
		public static EnvironmentSettings UNKNOWN
		{
			get
			{
				return new EnvironmentSettings()
				{
					EnvironmentType = EnvironmentType.Unknown
				};
			}
		}

		/// <summary>DEV environment settings</summary>
		public static EnvironmentSettings DEV
		{
			get
			{
				return new EnvironmentSettings()
                {
					EnvironmentType = EnvironmentType.Development,
                    //PublicSubdomain = "www-dev",
                    ConnectionString = @"mongodb+srv://consim-rw:Railgun08@cluster0-vjgjo.azure.mongodb.net/EncryptForm?retryWrites=true&w=majority",
                    Database = "EncryptForm",
					//AzureTableConnection = @"DefaultEndpointsProtocol=https;AccountName=formencryptprod001;AccountKey=aBf93ZB4q3rADvz1LJ2oGO0BpsIQv+WYqEWBBVGkS29BPGHqBxXw9juNLGnxyjYWC3ZuwmJN6pVxn5kvjR4Xqg==;EndpointSuffix=core.windows.net",
					AzureTableConnection = @"DefaultEndpointsProtocol=https;AccountName=raydreamsdevsa002;AccountKey=VHRzONoual1tKzfOUEqhDHmPzBb/GUZ9uyLDv9kj698JcQAkIELDx6K/LtDXOIrUpmeJIrWECj493eyOpSCldQ==;EndpointSuffix=core.windows.net",
					TimestampTimeout = 5,
					TokenPassword = "password",
					//AppClientID = "",
					//AppClientPW = "",
					//TokenKeyName = "token-key-dev",
				};
            }
		}

		/// <summary>PROD environment settings</summary>
		public static EnvironmentSettings PROD
		{
			get
			{
				return new EnvironmentSettings()
                {
					EnvironmentType = EnvironmentType.Production,
                    //PublicSubdomain = "www",
                    ConnectionString = @"mongodb+srv://consim-rw:Railgun08@cluster0-vjgjo.azure.mongodb.net/EncryptForm?retryWrites=true&w=majority",
                    Database = "EncryptForm",
					//AzureTableConnection = @"DefaultEndpointsProtocol=https;AccountName=formencryptprod001;AccountKey=aBf93ZB4q3rADvz1LJ2oGO0BpsIQv+WYqEWBBVGkS29BPGHqBxXw9juNLGnxyjYWC3ZuwmJN6pVxn5kvjR4Xqg==;EndpointSuffix=core.windows.net",
					AzureTableConnection = @"DefaultEndpointsProtocol=https;AccountName=raydreamsdevsa002;AccountKey=VHRzONoual1tKzfOUEqhDHmPzBb/GUZ9uyLDv9kj698JcQAkIELDx6K/LtDXOIrUpmeJIrWECj493eyOpSCldQ==;EndpointSuffix=core.windows.net",
					TimestampTimeout = 5,
					TokenPassword = "password",
					//AppClientID = "",
					//AppClientPW = "",
					//TokenKeyName = "token-key-prod",
				};
            }
		}
	}
}
