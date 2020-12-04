using System;
using MongoDB.Driver;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Raydreams.Common.Data;
using Raydreams.Common.Services.Model;
using Raydreams.Common.Services.Security;
using System.Linq;

namespace Raydreams.Common.Services.Data
{
	/// <summary></summary>
	public class PagesRepository : MongoDataManager<ContentPage, Guid>
	{
		#region [Fields]

		private string _table = "Pages";

		#endregion [Fields]

		#region [Constructors]

		/// <summary></summary>
		public PagesRepository( string connStr, string db, string table ) : base( connStr, db )
		{
			this.Table = table;
		}

		#endregion [Constructors]

		#region [Properties]

		/// <summary>The physical Collection name</summary>
		public string Table
		{
			get { return this._table; }
			protected set { if ( !String.IsNullOrWhiteSpace( value ) ) this._table = value.Trim(); }
		}

		#endregion [Properties]

		/// <summary>Get by DB ID only mainly to look to see if it exists</summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public ContentPage Get( Guid id )
		{
			if ( id == Guid.Empty )
				return null;

			return base.Get( id, this.Table );
		}

		/// <summary>Gets an article by its DB ID and domain</summary>
		/// <returns></returns>
		public ContentPage GetByDomain( string site, Guid id, string lang = "en" )
		{
			if ( id == Guid.Empty || String.IsNullOrWhiteSpace( site ) )
				return null;

			if ( String.IsNullOrWhiteSpace( lang ) )
				lang = "en";

			// trim
			site = site.Trim().ToLowerInvariant();
			lang = lang.Trim().ToLowerInvariant();

			IMongoCollection<ContentPage> coll = this.Database.GetCollection<ContentPage>( this.Table );
			ContentPage results = coll.Find<ContentPage>( p => p.ID == id ).FirstOrDefault();

			if ( results == null || !results.Domain.Equals( site, StringComparison.InvariantCultureIgnoreCase ) )
				return null;

			ContentLocale c = results.Locales.Where( s => s.Language == lang ).FirstOrDefault();

			if ( c != null )
			{
				results.Locales = new List<ContentLocale>() { c };
			}
			else
			{
				c = results.Locales[0];
				results.Locales = new List<ContentLocale>() { c };

			}

			return results;
		}

		/// <summary>Get all pages in a site</summary>
		/// <param name="domain"></param>
		/// <returns></returns>
		public List<ContentHeader> GetAllByDomain( string domain, PageContentType? type )
		{
			List<ContentHeader> headers = new List<ContentHeader>();

			if ( String.IsNullOrWhiteSpace( domain ) )
				return headers;

			domain = domain.Trim().ToLowerInvariant();

			// base query for domain only
			IMongoCollection<ContentPage> coll = this.Database.GetCollection<ContentPage>( this.Table );
			var filter = Builders<ContentPage>.Filter.Where( p => p.Domain.ToLowerInvariant() == domain );

			// add a type filter
			if ( type != null )
				filter &= Builders<ContentPage>.Filter.Where( p => p.Type == type );

			// get
			List<ContentPage> results = coll.Find<ContentPage>( filter ).ToList();

			// transform
			foreach ( ContentPage c in results )
			{
				headers.Add( new ContentHeader()
				{
					ID = c.ID,
					Title = c.Locales[0].Title,
					Domain = c.Domain,
					Language = c.Locales[0].Language,
					Path = c.PrimaryPath,
					Updated = c.Updated,
					Type = c.Type
				} );
			}

			return headers;
		}

		/// <summary>Gets the page that matches the domain and path. Returns only the matching language</summary>
		/// <param name="site"></param>
		/// <param name="path"></param>
		/// <param name="lang"></param>
		/// <returns>Returns the page with ONLY the specified locale as the only content</returns>
		public ContentPage GetByPath( string site, string path = "/", string lang = "en" )
		{
			// validate
			if ( String.IsNullOrWhiteSpace( site ) )
				return null;

			if ( String.IsNullOrWhiteSpace( lang ) )
				lang = "en";

			// trim
			site = site.Trim().ToLowerInvariant();
			lang = lang.Trim().ToLowerInvariant();

			IMongoCollection<ContentPage> coll = this.Database.GetCollection<ContentPage>( this.Table );

			// filter on primary first
			var filter = Builders<ContentPage>.Filter.Where( p => p.Domain.ToLowerInvariant() == site && p.PrimaryPath.ToLowerInvariant() == path.Trim() );
			List<ContentPage> results = coll.Find<ContentPage>( filter ).ToList();

			// then try secondary
			if ( results == null || results.Count < 1 )
			{
				filter = Builders<ContentPage>.Filter.Where( p => p.Domain.ToLowerInvariant() == site );
				filter &= Builders<ContentPage>.Filter.Regex( "paths", new BsonRegularExpression( String.Format( "/{0}/i", path.Trim() ) ) );
				results = coll.Find<ContentPage>( filter ).ToList();
			}

			// get ONLY the expected locale
			if ( results.Count > 0 && results[0].Locales != null && results[0].Locales.Count > 0 )
			{
				ContentLocale c = results[0].Locales.Where( s => s.Language == lang ).FirstOrDefault();

				if ( c != null )
				{
					results[0].Locales = new List<ContentLocale>() { c };
				}
				else
				{
					c = results[0].Locales[0];
					results[0].Locales = new List<ContentLocale>() { c };

				}

				return results[0];
			}

			return null;

			//var filter = Builders<ContentPage>.Filter.Regex( "domain", new BsonRegularExpression( String.Format( "/{0}/i", site.Trim() ) ) );
			//filter &= Builders<ContentPage>.Filter.AnyEq( "paths", path.Trim() );
			//filter &= Builders<ContentPage2>.Filter.AnyEq( "locales.lang", lang );
		}

		/// <summary>Gets pages by a specific category tag</summary>
		/// <param name="tag">A specific tag string ID so it is case sensitive</param>
		/// <returns></returns>
		public List<ContentHeader> GetAllByTag( string tag )
		{
			if ( String.IsNullOrWhiteSpace( tag ) )
				return new List<ContentHeader>();

			IMongoCollection<ContentHeader> coll = this.Database.GetCollection<ContentHeader>( this.Table );

			var filter = Builders<ContentHeader>.Filter.AnyEq( "tags", tag.Trim() );
			List<ContentHeader> results = coll.Find<ContentHeader>( filter ).Sort( "{created: -1}" ).ToList();

			return results ?? new List<ContentHeader>();
		}

		/// <summary>Inserts a new content page</summary>
		/// <param name="page">A new ContenPage which MUST have an ID, domin, at least one locale section and a primary path</param>
		/// <returns></returns>
		public bool Insert( ContentPage page )
		{
			if ( page == null )
				return false;

			// needs a domain
			if ( String.IsNullOrWhiteSpace( page.Domain ) )
				return false;

			// at least 1 locale
			if ( page.Locales == null || page.Locales.Count < 1 )
				return false;

			// a primary path
			if ( String.IsNullOrWhiteSpace( page.PrimaryPath ) )
				return false;

			// last chance to create a GUID
			if ( page.ID == Guid.Empty )
				page.ID = Guid.NewGuid();

			bool results = base.Insert( page, this.Table );

			return results;
		}
	}
}
