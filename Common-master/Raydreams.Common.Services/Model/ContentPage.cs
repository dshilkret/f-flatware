using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Raydreams.Common.Services.Model
{
	/// <summary>Enumerates possible types of Content for hinting the client</summary>
	public enum PageContentType
	{
		/// <summary>Non-specific</summary>
		Generic = 0,
		/// <summary>A page article</summary>
		Article = 1,
		/// <summary>A definition</summary>
		Definition = 2,
		/// <summary>A blog entry</summary>
		Blog = 3,
		/// <summary>A video</summary>
		Video = 5
	}

	/// <summary>This is a flattened summary version of a page to use for lists</summary>
	[BsonIgnoreExtraElements]
	public class ContentHeader
	{
		public ContentHeader()
		{
			this.Type = PageContentType.Generic;
		}

		/// <summary></summary>
		[BsonId()]
		[BsonElement( "_id" )]
		[BsonGuidRepresentation( GuidRepresentation.Standard )]
		[JsonProperty( "id" )]
		public Guid ID { get; set; }

		/// <summary>What type of content is this</summary>
		[BsonElement( "type" )]
		[JsonProperty( "type" )]
		[JsonConverter( typeof( StringEnumConverter ) )]
		public PageContentType Type { get; set; }

		/// <summary>Domain this for</summary>
		[BsonElement( "domain" )]
		[JsonProperty( "domain" )]
		public string Domain { get; set; }

		/// <summary></summary>
		[BsonElement( "lang" )]
		[JsonProperty( "lang" )]
		public string Language { get; set; }

		/// <summary>Title of the page</summary>
		[BsonElement( "title" )]
		[JsonProperty( "title" )]
		public string Title { get; set; }

		/// <summary>The is the primary path to the doc like a text ID</summary>
		[BsonElement( "path" )]
		[JsonProperty( "path" )]
		public string Path { get; set; }

		/// <summary>When this page was initially added</summary>
		[BsonElement( "updated" )]
		[BsonDateTimeOptions( Kind = DateTimeKind.Utc )]
		[JsonProperty( "updated" )]
		public DateTime Updated { get; set; }
	}

	/// <summary>This is the full page content entity with all localels</summary>
	[BsonIgnoreExtraElements]
	public class ContentPage
	{
		public ContentPage()
		{
			// create a null page
			this.ID = Guid.NewGuid();
			this.Locales = new List<ContentLocale>();
			this.Created = DateTimeOffset.UtcNow.DateTime;
			this.Updated = DateTimeOffset.UtcNow.DateTime;
			this.Tags = new List<string>();
			this.SecondaryPaths = new List<string>();
			this.Type = PageContentType.Generic;
		}

		/// <summary></summary>
		[BsonId()]
		[BsonElement( "_id" )]
		[BsonGuidRepresentation( GuidRepresentation.Standard )]
		[JsonProperty( "id" )]
		public Guid ID { get; set; }

		/// <summary>What type of content is this</summary>
		[BsonElement( "type" )]
		[JsonProperty( "type" )]
		[JsonConverter( typeof( StringEnumConverter ) )]
		public PageContentType Type { get; set; }

		/// <summary>Domain this for</summary>
		[BsonElement( "domain" )]
		[JsonProperty( "domain" )]
		public string Domain { get; set; }

		/// <summary>The is the primary path to the doc like a text ID</summary>
		[BsonElement( "path" )]
		[JsonProperty( "path" )]
		public string PrimaryPath { get; set; }

		/// <summary>Additional secondary alias paths to the same doc</summary>
		[BsonElement( "paths" )]
		[JsonProperty( "paths" )]
		public List<string> SecondaryPaths { get; set; }

		/// <summary>Any related docs of the parent doc</summary>
		/// <remarks>Relate by primary path</remarks>
		[BsonElement( "related" )]
		[JsonProperty( "related" )]
		public List<string> Related { get; set; }

		/// <summary></summary>
		[BsonElement( "locales" )]
		[JsonProperty( "locales" )]
		public List<ContentLocale> Locales { get; set; }

		/// <summary>These are root level elements like a banner or thumbnail</summary>
		[BsonElement( "media" )]
		[JsonProperty( "media" )]
		public ContentMedia Media { get; set; }

		/// <summary>When this page was initially added</summary>
		[BsonElement( "created" )]
		[BsonDateTimeOptions( Kind = DateTimeKind.Utc )]
		[JsonProperty( "created" )]
		public DateTime Created { get; set; }

		/// <summary>When this page was initially added</summary>
		[BsonElement( "updated" )]
		[BsonDateTimeOptions( Kind = DateTimeKind.Utc )]
		[JsonProperty( "updated" )]
		public DateTime Updated { get; set; }

		/// <summary></summary>
		[BsonElement( "active" )]
		[JsonProperty( "active" )]
		public bool Active { get; set; }

		/// <summary>The list of many to many category tags that go with this article</summary>
		[BsonElement( "tags" )]
		[JsonProperty( "tags" )]
		public List<string> Tags { get; set; }

		/// <summary></summary>
		[BsonElement( "meta" )]
		[JsonProperty( "meta" )]
		public dynamic Metadata { get; set; }

		/// <summary></summary>
		public void PrependCDN( string url )
		{
			// check for URL
			if ( String.IsNullOrWhiteSpace( url ) )
				return;

			if ( this.Media != null )
				this.Media.PrependCDN( url );

			if ( this.Locales != null && this.Locales.Count > 0 )
			{
				foreach ( ContentLocale loc in this.Locales )
				{
					if ( loc.Sections != null && loc.Sections.Count > 0 )
					{
						foreach ( ContentSection sec in loc.Sections )
							sec.PrependCDN( url );
					}
				}
			}
		}
	}

	/// <summary>A collection of media elements that associate with the page</summary>
	[BsonIgnoreExtraElements]
	public class ContentMedia
	{
		/// <summary>The Banner or main image associated with this article</summary>
		[BsonElement( "banner" )]
		[JsonProperty( "banner" )]
		public string Banner { get; set; }

		/// <summary>The thumbnail image associated with this article</summary>
		[BsonElement( "thumbnail" )]
		[JsonProperty( "thumbnail" )]
		public string Thumbnail { get; set; }

		/// <summary>A video associated with this article</summary>
		[BsonElement( "video" )]
		[JsonProperty( "video" )]
		public string Video { get; set; }

		/// <summary></summary>
		public void PrependCDN( string url )
		{
			// check for URL
			if ( String.IsNullOrWhiteSpace( url ) )
				return;

			if ( this.Banner != null )
				this.Banner = $"{url}{this.Banner}";

			if ( this.Thumbnail != null )
				this.Thumbnail = $"{url}{this.Thumbnail}";

			if ( this.Video != null )
				this.Video = $"{url}{this.Video}";
		}
	}

	/// <summary>The actual content by language</summary>
	[BsonIgnoreExtraElements]
	public class ContentLocale
	{
		public ContentLocale()
		{
			this.Language = "en";
			this.Sections = new List<ContentSection>();
		}

		/// <summary></summary>
		[BsonElement( "lang" )]
		[JsonProperty( "lang" )]
		public string Language { get; set; }

		/// <summary>The page title</summary>
		[BsonElement( "title" )]
		[JsonProperty( "title" )]
		public string Title { get; set; }

		/// <summary>The page summary</summary>
		[BsonElement( "summary" )]
		[JsonProperty( "summary" )]
		public string Summary { get; set; }

		/// <summary>Sections in the page</summary>
		[BsonElement( "sections" )]
		[JsonProperty( "sections" )]
		public List<ContentSection> Sections { get; set; }

		/// <summary>Adds a new section with the correct ID</summary>
		public bool AddSection( ContentSection sec )
		{
			if ( sec == null )
				return false;

			if ( this.Sections == null )
				this.Sections = new List<ContentSection>();

			sec.ID = this.Sections.Count + 1;
			this.Sections.Add( sec );

			return true;
		}
	}

	/// <summary>A section within the content</summary>
	[BsonIgnoreExtraElements]
	public class ContentSection
	{
		/// <summary>A sequential ID that can be used for ordering.</summary>
		[BsonElement( "id" )]
		[JsonProperty( "id" )]
		public int ID { get; set; }

		/// <summary>Each section can have an image usually set in the margin at the top of the section</summary>
		[BsonElement( "image" )]
		[JsonProperty( "image" )]
		public string Image { get; set; }

		/// <summary>Section title with markup</summary>
		[BsonElement( "title" )]
		[JsonProperty( "title" )]
		public string Title { get; set; }

		/// <summary>Section content with markup</summary>
		[BsonElement( "content" )]
		[JsonProperty( "content" )]
		public string Content { get; set; }

		/// <summary></summary>
		public void PrependCDN( string url )
		{
			// check for URL
			if ( String.IsNullOrWhiteSpace( url ) )
				return;

			if ( this.Image != null )
				this.Image = $"{url}{this.Image}";
		}
	}
}
