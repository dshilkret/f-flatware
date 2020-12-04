using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Raydreams.Common.Services.Model;

namespace Raydreams.Common.Services.Data
{
	/// <summary>Repo to Product Items stored in Azure Tables</summary>
    public class MarkerAzureRepo
    {
		#region [Fields]

		private string _connStr = String.Empty;

		private string _table = "ProductItems";

		#endregion [Fields]

		#region [Constructors]

		/// <summary></summary>
		public MarkerAzureRepo( string connStr, string table )
		{
			this._connStr = connStr;
			this.TableName = table;
		}

		#endregion [Constructors]

		#region [Properties]

		/// <summary>The physical Collection name</summary>
		public string TableName
		{
			get { return this._table; }
			protected set { if ( !String.IsNullOrWhiteSpace( value ) ) this._table = value.Trim(); }
		}

		/// <summary>The physical Collection name</summary>
		public CloudTable AzureTable
		{
			get
			{
				CloudStorageAccount account = CloudStorageAccount.Parse( this._connStr );
				CloudTableClient client = account.CreateCloudTableClient();
				CloudTable tbl = client.GetTableReference( this.TableName );
				return tbl;
			}
		}

		#endregion [Properties]

		/// <summary>Gets ALL the markers in the table</summary>
		/// <returns></returns>
		public List<ProductItem> GetAll()
        {
			TableQuerySegment<ProductItem> results = null;
			TableContinuationToken tok = new TableContinuationToken();
			results =  this.AzureTable.ExecuteQuerySegmentedAsync<ProductItem>( new TableQuery<ProductItem>(), tok ).GetAwaiter().GetResult();

			return results.Results;
		}

		/// <summary>Filters by product line</summary>
        /// <param name="line">The partition key in Azure Tables</param>
		/// <returns></returns>
		public List<ProductItem> GetAllByLine(string line)
		{
			if ( String.IsNullOrWhiteSpace( line ) )
				return new List<ProductItem>();

			TableQuerySegment<ProductItem> results = null;
			TableContinuationToken tok = new TableContinuationToken();

			var query = new TableQuery<ProductItem>().Where( TableQuery.GenerateFilterCondition( "PartitionKey", QueryComparisons.Equal, line ) );
			results = this.AzureTable.ExecuteQuerySegmentedAsync<ProductItem>( query, tok ).GetAwaiter().GetResult();

			return results.Results;
		}

		/// <summary>Updates a single product item</summary>
		public int UpdateProductItem( ProductItem item )
        {
			if ( item == null )
				return Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest;

			TableOperation op = TableOperation.Replace( item );
			TableResult results = this.AzureTable.ExecuteAsync( op ).GetAwaiter().GetResult();

			return results.HttpStatusCode;
		}

		/// <summary></summary>
		/// <param name="items"></param>
		public int WriteProductItems( List<ProductItem> items )
		{
			TableResult results = null;

			foreach ( ProductItem item in items )
			{
				try
				{
					TableOperation op = TableOperation.InsertOrMerge( item );
					results = this.AzureTable.ExecuteAsync( op ).GetAwaiter().GetResult();
				}
				catch ( System.Exception exp )
				{
					; // log it somehow and keep going
				}
			}

			return results.HttpStatusCode;
		}

	}
}
