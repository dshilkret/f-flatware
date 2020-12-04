using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;

namespace Raydreams.Common.Services.Model
{
	/// <summary></summary>
	public enum ImageSizeKey: byte
	{
		Thumbnail = 0,
		Small,
		Medium,
		Large,
		XLarge,
		Original
	}

	/// <summary>Adds properties to define preset image sizes.</summary>
	public class StockImageSize
	{
		private ImageSizeKey _key = ImageSizeKey.Original;
		private int _maxWidth = 0;
		private StringCollection _codes = null;
		private int _borderWidth = 0;

		/// <summary></summary>
		public StockImageSize( ImageSizeKey key, int width, int borderWidth, params string[] names )
		{
			this._key = key;
			this._maxWidth = width;
			this._codes = new StringCollection();
			this._codes.AddRange( names );
			this._borderWidth = borderWidth;
		}

		/// <summary></summary>
		public ImageSizeKey Key
		{
			get { return this._key; }
		}

		/// <summary></summary>
		public int MaxWidth
		{
			get { return this._maxWidth; }
			set { this._maxWidth = value; }
		}

		/// <summary></summary>
		public int InsetBorderWidth
		{
			get { return this._borderWidth; }
			set { this._borderWidth = value; }
		}

		/// <summary>Get the list of possible names used for this item in the collection</summary>
		public StringCollection Names
		{
			get { return this._codes; }
		}

		/// <summary></summary>
		public string DisplayName
		{
			get
			{
				//if ( this._codes != null && !String.IsNullOrEmpty( this._names[0] ) )
					//return this._names[0];

				return Enum.Format( typeof( ImageSizeKey ), this._key, "g" );
			}
		}
	}

	/// <summary></summary>
	public class StockImageSizeCollection: KeyedCollection<ImageSizeKey, StockImageSize>
	{
		private static StockImageSizeCollection _list = new StockImageSizeCollection();

		/// <summary></summary>
		static StockImageSizeCollection()
		{
			_list.Add( new StockImageSize( ImageSizeKey.Thumbnail, 70, 4, "thumbnail", "t", "0" ) );
			_list.Add( new StockImageSize( ImageSizeKey.Small, 160, 8, "small", "s", "1" ) );
			_list.Add( new StockImageSize( ImageSizeKey.Medium, 320, 16, "medium", "m", "2" ) );
			_list.Add( new StockImageSize( ImageSizeKey.Large, 640, 32, "large", "l", "3" ) );
			_list.Add( new StockImageSize( ImageSizeKey.XLarge, 1280, 64, "xlarge", "x", "4" ) );
			_list.Add( new StockImageSize( ImageSizeKey.Original, 0, 0, "original", "o", "5" ) );
		}

		/// <summary></summary>
		public StockImageSizeCollection() : base() { }

		/// <summary></summary>
		protected override ImageSizeKey GetKeyForItem( StockImageSize item )
		{
			return item.Key;
		}

		/// <summary>Get all the strat types.</summary>
		public static StockImageSizeCollection StockSizes
		{
			get { return _list; }
		}

		/// <summary>Get a specific item by a string name. Case insensitive and compares all the alt names to the specified input string.</summary>
		public static StockImageSize GetSize( string name )
		{
			if ( String.IsNullOrEmpty(name) )
				return null;

			foreach ( StockImageSize item in _list )
				foreach ( string n in item.Names )
					if ( String.Compare( n, name, true ) == 0 )
						return item;

			return null;
		}
	}
}
