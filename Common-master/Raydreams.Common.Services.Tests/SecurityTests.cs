using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Raydreams.Common.Security;
using Raydreams.Common.Services.Model;
using Raydreams.Common.Services.Security;
using Raydreams.Common.Extensions;
using System.Reflection;
using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Raydreams.Common.Services.Tests
{
	[TestClass]
	public class SecurityTests
	{
		//[TestMethod]
		//public void TestMethod1()
		//{
		//	string connStr = @"mongodb+srv://consim-rw:Railgun08@cluster0-vjgjo.azure.mongodb.net/test?retryWrites=true&w=majority";

		//	MongoClient client = new MongoClient( connStr );
		//	IMongoCollection<ContentPage> coll = client.GetDatabase( "CMS" ).GetCollection<ContentPage>( "Pages" );
		//	var filter = Builders<ContentPage>.Filter.Eq( "site", "raydreams.com" );
		//	filter &= Builders<ContentPage>.Filter.AnyEq( "paths", "home" );
		//	List<ContentPage> results = coll.Find<ContentPage>( filter ).ToList();

		//	ContentPage content = (results.Count > 0) ? results[0] : new ContentPage();

		//	Assert.Inconclusive( "No test yet" );
		//}


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
		public void MakeSaltTest()
		{
			SimpleTokenManager mgr = new SimpleTokenManager( "password" );
		}

		[TestMethod]
		public void RefreshSessionTest()
        {
			//SimpleTokenManager mgr = new SimpleTokenManager( "password" );

			//Guid id = Guid.NewGuid();

			//CipherMessage? cipher = mgr.Create( id.ToString(), "please", "ABCD1234" );
			//string token = $"{Convert.ToBase64String( cipher?.IV )}${Convert.ToBase64String( cipher?.CipherBytes )}";

			//string[] parts = token.Split( '$', StringSplitOptions.RemoveEmptyEntries );

			//TokenPayload payload = mgr.Decode( parts[1], Convert.FromBase64String( parts[0] ) );

			int x = 5;
		}

		/// <summary>Tests on a Reward calculation from the database</summary>
		[TestMethod]
		public void EncryptFormTest()
		{
			string json = "{\"name\":\"TestForm\",\"fields\":[\"ssn\",\"dob\"],\"formData\":{\"fname\":\"Bob\",\"lname\":\"Smith\",\"ssn\":\"111-22-3333\",\"dob\":\"01012000\"}}";


			PlainForm form = JsonConvert.DeserializeObject<PlainForm>( json );

			dynamic outForm = new ExpandoObject();
			var dict = outForm as IDictionary<string, Object>;

			foreach ( JProperty pi in form.FormData.Properties() )
			{
				string plain = pi.Value.ToString();

				string encrypted = Convert.ToBase64String( plain.HashToMD5(), Base64FormattingOptions.None );

				dict.Add( pi.Name, encrypted );
			}


			//foreach ( PropertyInfo pi in form.FormData.GetType().GetProperties( BindingFlags.Instance | BindingFlags.Public ) )
			//{
			//	string plain = pi.GetValue( form.FormData ).ToString();

			//	string encrypted = Convert.ToBase64String( plain.HashToMD5(), Base64FormattingOptions.None );

			//	// can't set anon obj properties
			//	//pi.SetValue( form.FormData, encrypted );
			//	dict.Add( pi.Name, encrypted );
			//}

			//foreach ( var key in form.FormData.Keys )
			//{
			//	// check if the value is not null or empty.
			//	if ( !String.IsNullOrWhiteSpace( form.FormData[key] ) )
			//	{
			//		string value = form.FormData[key] as string;

			//		form.FormData[key] = Convert.ToBase64String( value.HashToMD5(), Base64FormattingOptions.None );
			//	}
			//}

			int x = 5;
		}

		/// <summary></summary>
		[TestMethod]
		public void SymmetricKeyTest()
        {
			byte[] _iv = new byte[] { 0x86, 0x6a, 0x8f, 0xe4, 0xc4, 0xae, 0x46, 0x7f, 0x7b, 0x6f, 0x94, 0x34, 0xd4, 0xf8, 0xac, 0x66 };

			byte[] salt = new byte[] { 0x9f, 0x57, 0x8b, 0xc5, 0xe9, 0xfc, 0x13, 0x20, 0xc2, 0x42, 0x79, 0x44, 0x74, 0xff, 0xc5, 0x7e };

			byte[] pw = Encoding.UTF8.GetBytes( "parrot" );
			Rfc2898DeriveBytes pwdGen = new Rfc2898DeriveBytes( pw, salt, 10 );

			// size of the key has to match the algorithm
			byte[] key = pwdGen.GetBytes( 32 );

			Assert.IsNotNull( key );
		}

		/// <summary></summary>
		[TestMethod]
		public void GetAsymKeyTest()
		{
			RSAKeyGenerator gen = new RSAKeyGenerator();
			int ks = gen.GenerateKeys( RSAKeySize.Key1024, false );

			string encrypted = AsymmetricEncryptor.Encrypt( "hello, you fool!", ks, gen.PublicKey );
			string original = AsymmetricEncryptor.Decrypt( encrypted, ks, gen.PrivateKey );

			Assert.IsNotNull(original);
		}

		/// <summary>Tests on a Reward calculation from the database</summary>
		[TestMethod]
		public void AsymEncryptTest()
		{
			string pk = @"<RSAKeyValue><Modulus>yDOHwWnVQr/eErY298lySN9cffp55ZsUbYoL1gGt38t89OzTNvSiUMe7FT/WBoEbx4FvqExM3nhcojQpS17QLDoPpthKA5BB1gyG5DULPtklvnBMSrCj0db/HwV54n8SBg2dLl+LgpiJ3kY0aayXp/vxz+EW7mLzg1rnpj54/s8=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
			int ks = 1024;
			string msg = "Star05tide";

			string encrypted = AsymmetricEncryptor.Encrypt( msg, ks, pk );

			Assert.IsNotNull( encrypted );
		}

	}
}