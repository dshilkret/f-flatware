using System;
using System.Collections.Generic;
using Raydreams.Common.Services.Model;
using Raydreams.Common.Extensions;

namespace Raydreams.Common.Services.Interface
{
	/// <summary></summary>
	public partial class CommonGateway : ICommonGateway
	{
		/// <summary>Gets a specifiec page by its DB ID</summary>
		public ContentPage GetContentByID( string domain, Guid id, string lang = "en" )
		{
			if ( id == Guid.Empty )
				return null;

			ContentPage page = this.CMSRepo.GetByDomain( domain, id, lang );

			if ( this.Settings != null )
				page.PrependCDN( this.Settings.ImagesCDN );

			if ( page == null )
				return null;

			return page;
		}

		/// <summary>Gets a specifiec page</summary>
		public ContentPage GetContentByPath( string domain, string path = "/", string lang = "en" )
		{
			if ( String.IsNullOrWhiteSpace( domain ) )
				return null;

			ContentPage page = this.CMSRepo.GetByPath( domain, path, lang );

			if ( page == null )
				return null;

			if ( this.Settings != null )
				page.PrependCDN( this.Settings.ImagesCDN );

			return page;
		}

		/// <summary>Gets all content pages that fall within a speified domain as a list of headers</summary>
		public List<ContentHeader> GetContentList( string domain, string type = null )
		{
			if ( String.IsNullOrWhiteSpace( domain ) )
				return new List<ContentHeader>();

			if ( !String.IsNullOrWhiteSpace( type ) )
			{
				// parse the type if any
				PageContentType lvl = PageContentType.Generic;
				type.TryParseToEnum<PageContentType>( out lvl, true );
				return this.CMSRepo.GetAllByDomain( domain, lvl );
			}

			return this.CMSRepo.GetAllByDomain( domain, null );
		}

		/// <summary>Gets all the Page Content Tags</summary>
		public List<ContentTag> GetContentTags()
		{
			return this.TagsRepo.GetAll();
		}

		/// <summary>Inserts a new page</summary>
		public bool InsertContentPage( ContentPage page )
		{
			if ( page == null || String.IsNullOrWhiteSpace( page.PrimaryPath ) )
				return false;

			ContentPage match = null;

			// look for a page with this path or ID
			match = this.CMSRepo.GetByPath( page.Domain, page.PrimaryPath );

			if ( match != null )
				return false;

			if ( page.ID != Guid.Empty )
				match = this.CMSRepo.Get( page.ID );

			if ( match != null )
				return false;

			// finally insert
			return this.CMSRepo.Insert( page );
		}

	}
}
