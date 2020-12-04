using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raydreams.Common.Services.Data;
using Raydreams.Common.Services.Config;
using Raydreams.Common.Model;
using Raydreams.Common.Services.Model;
using Raydreams.Common.Services.Images;
using System.IO;
using SkiaSharp;

namespace Raydreams.Common.Services.Tests
{
    [TestClass]
    public class DrawingTests
    {
		public static readonly string DesktopPath = Environment.GetFolderPath( Environment.SpecialFolder.DesktopDirectory );// "/Users/lobsterpixel/Desktop";

		[ClassInitialize()]
		public static void ClassInit(TestContext context)
		{
		}

		[TestInitialize()]
		public void Initialize()
		{
		}

		/// <summary></summary>
		[TestMethod]
		public void InstatImageTest()
		{
			InstaImage img = new InstaImage( 800 );
			img.BorderThickness = 50;
			img.BackgroundColor = SKColor.Parse( "007A99" );
			img.BorderColor = SKColor.Parse( "FF0000" );
			img.Message = "The Quick Brown Fox Jumped Over The Lazy Dog!";
			img.StarDensity = 25;
			img.LayoutGrid = false;
			img.TextColor = SKColor.Parse( "123456" );

			img.Draw();

			using (FileStream s = File.Open( DesktopPath + "/Insta.png", FileMode.Create ))
			{
				img.GetImage(100).SaveTo( s );
			}
		}
	}
}
