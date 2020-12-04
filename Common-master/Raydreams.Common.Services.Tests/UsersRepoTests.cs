using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raydreams.Common.Services.Data;
using Raydreams.Common.Services.Config;
using Raydreams.Common.Model;
using Raydreams.Common.Services.Model;
using Raydreams.Common.Services.Security;

namespace Raydreams.Common.Services.Tests
{
	[TestClass]
	public class UsersRepoTests
	{
		[ClassInitialize()]
		public static void ClassInit( TestContext context )
		{
		}

		[TestInitialize()]
		public void Initialize()
		{
			EnvironmentSettings env = EnvironmentSettings.GetSettings(EnvironmentType.Development);
			this.Repo = new UsersRepository(new MD5PassworMaker(), env.ConnectionString, env.Database, TableNames.Users);
			this.SessionsRepo = new MongoSessionsRepo(env.ConnectionString, env.Database, TableNames.Sessions);
		}

		public UsersRepository Repo { get; set; }
		public MongoSessionsRepo SessionsRepo { get; set; }

		/// <summary></summary>
		[TestMethod]
		public void GetUserTest()
		{
			Guid userID = new Guid("2501987e-1c35-4513-9a59-5f61dcae54fe");

			User results = this.Repo.Get(userID);

			Assert.IsNotNull(results);
		}

		/// <summary></summary>
		[TestMethod]
		public void InsertUserTest()
		{
			(Guid ID, string PlainPW) results1 = this.Repo.Insert( "jbob", "Joe Bob", "tguillory@gmail.com", new DomainRole() { Role = RoleLevel.Admin, Domain = "bouncingpixel.com" } );

			(Guid ID, string PlainPW) results2 = this.Repo.Insert( "sride", "Sally Ride", "tguillory@gmail.com", new DomainRole() { Role = RoleLevel.Admin, Domain = "bouncingpixel.com" } );

			bool results = this.Repo.UpdatePassword( results1.ID, "Password1" );

			results = this.Repo.UpdatePassword( results2.ID, "Password1" );

			Assert.IsNotNull( results2 );
		}

		/// <summary></summary>
		[TestMethod]
		public void GetSessionsTest()
		{
			string sesID = "473cd641-90c4-40a1-92cd-980c1f037e2d";

			Session results = this.SessionsRepo.Get(sesID);

			Assert.IsNotNull(results);
		}

		/// <summary></summary>
		//[TestMethod]
		//public void GetUserPreferencesTest()
		//{
		//	Guid userID = new Guid("3bbabeed-5721-493f-9d84-4259584dd365");

		//	UserPreferences results = this.Repo.GetPreferences(userID);

		//	Assert.IsNotNull(results);
		//}

		/// <summary></summary>
		[TestMethod]
		public void UpdatePWTest()
		{
			Guid userID = new Guid("e5cfae73-d823-b749-9280-05e008fa2e65");

			bool results = this.Repo.UpdatePassword(userID, "Password1");

			Assert.IsTrue(results);
		}

	}
}
