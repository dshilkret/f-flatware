using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raydreams.Common.Services.Data;
using Raydreams.Common.Services.Config;
using Raydreams.Common.Model;
using Raydreams.Common.Services.Model;

namespace Raydreams.Common.Services.Tests
{
	[TestClass]
	public class PortfoliosRepoTests
	{
		[ClassInitialize()]
		public static void ClassInit( TestContext context )
		{
		}

		[TestInitialize()]
		public void Initialize()
		{
			EnvironmentSettings env = EnvironmentSettings.GetSettings(EnvironmentType.Development);
			this.Repo = new PortfoliosRepository(env.ConnectionString, env.Database, TableNames.Portfolios);
		}

		public PortfoliosRepository Repo { get; set; }

		/// <summary></summary>
		[TestMethod]
		public void GetPortfolioTest()
		{
			Guid user = new Guid("f9bead5c-ae42-44c8-82c1-1e9135279d5c");

			Portfolio results = this.Repo.GetByUser(user, "Portfolio 1");

			Assert.IsNotNull(results);
		}

		/// <summary></summary>
		[TestMethod]
		public void GetPortfolioNamesTest()
		{
			Guid user = new Guid("f9bead5c-ae42-44c8-82c1-1e9135279d5c");

			List<LookupPair> results = this.Repo.GetNamesByUser(user);

			Assert.IsNotNull(results);
		}

		/// <summary></summary>
		[TestMethod]
		public void GetWatchedStockTest()
		{
			Guid user = new Guid("f9bead5c-ae42-44c8-82c1-1e9135279d5c");
			string symbol = "DOCU";

			StockWatch results = this.Repo.GetWatched( user, "Portfolio 1", symbol );

			Assert.IsNotNull( results );
		}

		/// <summary></summary>
		[TestMethod]
		public void UpdateWatchedStockTest()
		{
			Guid user = new Guid("f9bead5c-ae42-44c8-82c1-1e9135279d5c");
			
			StockWatch stock = new StockWatch("OXY") { BuyAlert = 151.1 };

			int results = this.Repo.InsertOrUpdateWatched(user, "Portfolio 1", stock);

			Assert.IsTrue(results > 0);
		}

		/// <summary></summary>
		[TestMethod]
		public void DeleteWatchedStockTest()
		{
			Guid user = new Guid("f9bead5c-ae42-44c8-82c1-1e9135279d5c");
			Stock stock = new Stock("OXY");

			bool results = this.Repo.DeleteWatched(user, "Portfolio 1", stock.Symbol);

			Assert.IsTrue(results);
		}

		/// <summary></summary>
		[TestMethod]
		public void InsertPortfolioTest()
		{
			Guid user = new Guid("e5cfae73-d823-b749-9280-05e008fa2e65");

			Portfolio p = new Portfolio(user, "Portfolio 2");

			bool results = this.Repo.Insert(p);

			Assert.IsTrue(results);
		}

	}
}
