using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Raydreams.Common.Services.Config;
using Raydreams.Common.Services.Data;
using Raydreams.Common.Services.Model;

namespace Raydreams.Common.Services.Tests
{
	[TestClass]
	public class PagesRepoTests
	{
		[ClassInitialize()]
		public static void ClassInit( TestContext context )
		{
		}

		[TestInitialize()]
		public void Initialize()
		{
			EnvironmentSettings config = EnvironmentSettings.DEV;

			this.Repo = new PagesRepository( config.ConnectionString, config.Database, TableNames.ContentPages );
		}

		public PagesRepository Repo { get; set; }

		/// <summary></summary>
		[TestMethod]
		public void GetPageTest()
		{
			ContentPage results = this.Repo.GetByPath( "raydreams.com", "welcome", "en" );

			Assert.IsNotNull( results );
		}

		/// <summary></summary>
		[TestMethod]
		public void InsertPageTest()
		{
			//ContentPage page = new ContentPage();
			//page.Active = true;
			//page.Title = "A Brave New World";
			//page.Domain = "bouncingpixel.com";
			//page.Page = "Welcome";
			//page.Sections.Add( new ContentSection() { ID = "Section1", Contents = new List<ContentLocale>() } );
			//page.Paths = new List<string>() { "welcome" };
			//page.Summaries = new Dictionary<string, string>()
			//{
			//	{"en","Welcome to a brave new world" },
			//	{ "fr", "Bienvenue dans un nouveau monde courageux" }
			//};

			//bool results = this.Repo.Insert( page );

			ContentPage page = new ContentPage();
			page.Active = true;
			page.Domain = "raydreams.com";
			//page.Media = new ContentMedia() { Video = "172175388" };
			page.PrimaryPath = "fresheyes";
			page.Type = PageContentType.Blog;

			ContentLocale en = new ContentLocale()
			{
				Language = "en",
				Title = "Fresh Eyes: The Evolution Of Vision",
				Summary = "A fable by Daniel Greenberg"
			};
			en.Sections.Add( new ContentSection() { ID = 1, Title = "<h2>Fresh Eyes: The Evolution Of Vision</h2>", Image = null, Content = "The APR" } );

			//ContentLocale fr = new ContentLocale()
			//{
			//	Language = "fr",
			//	Title = "Bienvenue dans un nouveau monde courageux",
			//	Summary = "Une page de test"
			//};
			//en.Sections.Add( new ContentSection() { ID = "Section1", Title = "Section 1", Content = "C'est un test" } );

			page.Locales.Add( en );
			//page.Locales.Add( fr );

			bool results = this.Repo.Insert( page );

			Assert.IsTrue( results );
		}
	}
}
