using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raydreams.Common.Services.Security;
using Raydreams.Common.Services.Config;
using Raydreams.Common.Services.Data;
using Raydreams.Common.Model;
using Raydreams.Common.Services.Interface;
using Raydreams.Common.Services.Model;
using Raydreams.Common.Security;
using System.Drawing;

namespace Raydreams.Common.Services.Tests
{
    [TestClass]
    public class GatewayTests
    {
		public ICommonGateway Gate { get; set; }

		[ClassInitialize()]
		public static void ClassInit( TestContext context )
		{
		}

		[TestInitialize()]
		public void Initialize()
		{
			this.Gate = new CommonGateway( EnvironmentSettings.DEV, CommonGateway.DefaultDomain );
			//this.Gate.LoadMocks();
		}

		/// <summary></summary>
		[TestMethod]
		public void LogTest()
		{
			this.Gate.Log( "Test log", "Test", "Info" );

			Assert.IsTrue( true );
		}

		/// <summary></summary>
		[TestMethod]
		public void LoginTest()
		{
			LoginResponse login = this.Gate.Login( "jbob", "Password1", "formencrypt.com","192.168.1.10");

			ITokenManager tmgr = new SimpleTokenManager( EnvironmentSettings.DEV.TokenPassword );
			Session refresh = this.Gate.RefreshSession( tmgr.Decode(login.Token) );

			bool loggedout = this.Gate.Logout( tmgr.Decode( login.Token ) );

			Assert.IsNotNull( loggedout );
		}

		/// <summary></summary>
		[TestMethod]
		public void ColorMatchTest()
		{
			Color c = Color.Coral;

			ProductLine results = this.Gate.MatchColor( "copicCiao", c );

			Assert.IsTrue( results.Items.Count > 0 );
		}
	}
}
