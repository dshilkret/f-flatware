using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raydreams.Common.Services.Data;
using Raydreams.Common.Services.Config;
using Raydreams.Common.Model;
using Raydreams.Common.Services.Model;
using MongoDB.Bson;

namespace Raydreams.Common.Services.Tests
{
	[TestClass]
	public class DataRepoTests
	{
		[ClassInitialize()]
		public static void ClassInit(TestContext context)
		{
		}

		[TestInitialize()]
		public void Initialize()
		{
			EnvironmentSettings env = EnvironmentSettings.GetSettings( EnvironmentType.Development );
			this.Repo = new BinaryDataRepo( env.ConnectionString, env.Database, "Data" );
		}

		public BinaryDataRepo Repo { get; set; }

		/// <summary></summary>
		[TestMethod]
		public void GetTest()
		{
			BinaryData test = new BinaryData();
			//new BinaryData() { Data = new byte[] { 0x5b, 0x38, 0xb9, 0xdf, 0x27, 0xef, 0x6d, 0xbc, 0xcb, 0xb8, 0x23, 0xcc, 0x6a, 0x3e, 0x8d, 0x0c } }
			test.EncryptedField = this.Repo.EncryptField( "fred" );

			BinaryData results = this.Repo.Get( new ObjectId( "5ee7d74b044a7bce0e0c95cb" ) );

			string field = this.Repo.Decode( results.EncryptedField );

			Assert.IsNotNull( results );
		}

		/// <summary></summary>
		[TestMethod]
		public void InsertTest()
		{
			BinaryData test = new BinaryData();
			//new BinaryData() { Data = new byte[] { 0x5b, 0x38, 0xb9, 0xdf, 0x27, 0xef, 0x6d, 0xbc, 0xcb, 0xb8, 0x23, 0xcc, 0x6a, 0x3e, 0x8d, 0x0c } }
			test.EncryptedField = this.Repo.EncryptField( "fred" );

			bool results = this.Repo.Insert( test );

			Assert.IsNotNull( results );
		}

	}
}
