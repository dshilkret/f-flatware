using System;
using System.Security.Cryptography;
using System.Text;
using MongoDB.Bson;
using Raydreams.Common.Data;
using Raydreams.Common.Security;
using Raydreams.Common.Services.Model;

namespace Raydreams.Common.Services.Data
{
    public class BinaryDataRepo : MongoDataManager<BinaryData, ObjectId>
    {
		#region [Fields]

		private string _table = "Data";

		/// <summary>The actual key password should something really long like
		/// 401b09eab3c013d4ca54922bb802bec8fd5318192b0a75f201d8b37279090fb337591abd3e44453b954555b7a0812e1081c39b740293f765eae731f5a65ed1
		/// </summary>
		private string _key = "bubbagumpshrimp";

		/// <summary>init vector to use</summary>
		private byte[] _iv = new byte[] { 0xaa, 0xb1, 0xf7, 0xa7, 0xee, 0xed, 0x12, 0x4f, 0xbf, 0x3f, 0x29, 0x55, 0xa7, 0x32, 0x24, 0xf0 };

		/// <summary>salt to use</summary>
		private byte[] _salt = new byte[] { 0xf6, 0x13, 0xab, 0x0F, 0x9a, 0x39, 0x9d, 0x19, 0x15, 0xe1, 0xf2, 0xaa, 0x7b, 0x92, 0x89, 0x55 };

		/// <summary>encryption iterations</summary>
		private int _iterations = 3;

		#endregion [Fields]


		#region [Constructors]

		/// <summary></summary>
		public BinaryDataRepo(string connStr, string db, string table) : base( connStr, db )
		{
			this.Table = table;
		}

		#endregion [Constructors]

		#region [Properties]

		/// <summary>The table to use for this data source</summary>
		public string Table
		{
			get { return this._table; }
			protected set { if (!String.IsNullOrWhiteSpace( value )) this._table = value.Trim(); }
		}

		#endregion [Properties]

		/// <summary>Inserts a new session</summary>
		/// <returns></returns>
		public bool Insert(BinaryData data)
		{
			return base.Insert( data, this.Table );
		}

		/// <summary>Inserts a new session</summary>
		/// <returns></returns>
		public BinaryData Get(ObjectId id)
		{
			return base.Get( id, this.Table );
		}

		// <summary>Constructs the PW to use with the token encrypt/decrypt based on the key, salt and iterations</summary>
		protected byte[] Key
		{
			get
			{
				byte[] key = Encoding.UTF8.GetBytes( this._key );
				Rfc2898DeriveBytes pwdGen = new Rfc2898DeriveBytes( key, this._salt, _iterations );
				return pwdGen.GetBytes( 16 );
			}
		}

		/// <summary>
        /// Encrypts a string of data using AES
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
		public byte[] EncryptField(string data)
		{
			if (String.IsNullOrWhiteSpace( data ) )
				return null;

			SymmetricEncryptor enc = new SymmetricEncryptor( SymmetricAlgoType.AES );
			byte[] coded = enc.Encrypt( data, this.Key, this._iv );

			return coded;
		}

		/// <summary>Decrypt the data back</summary>
		/// <returns>The decode.</returns>
		/// <param name="token">Token.</param>
		public string Decode(byte[] encData)
		{
			SymmetricEncryptor enc = new SymmetricEncryptor( SymmetricAlgoType.AES );
			string data = enc.Decrypt( encData, this.Key, this._iv );

			return data;
		}

	}
}
