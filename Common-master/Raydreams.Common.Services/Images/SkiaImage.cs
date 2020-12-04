using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using SkiaSharp;

namespace Raydreams.Common.Services.Images
{
	public abstract class SkiaImage
	{
		#region [ Fields ]

		// get the MIME type string for an image type
		public static Dictionary<ImageFormat, string> ContentTypes = null;

		protected static string defaultFontFamily = "Helvetica";

		public static readonly SKColor Debug = new SKColor( 255, 0, 0 );

		public static readonly SKColor White = new SKColor( 255, 255, 255 );

		public static readonly SKColor Black = new SKColor( 0, 0, 0 );

		public static SKColor DefaultColor = Black;

		// the image to draw on
		protected SKSurface _surface = null;

		protected SKImageInfo _info;

		#endregion [ Fields ]

		/// <summary></summary>
		public SkiaImage(int width = 100, int height = 100)
		{
			this.Init( width, height );
		}

		/// <summary></summary>
		private void Init(int width, int height)
		{
			width = BoundInt( width, 100, 4000 );
			height = BoundInt( height, 100, 4000 );

			// define the surface properties
			this._info = new SKImageInfo( width, height, SKColorType.Rgba8888 );

			SKImage img = SKImage.Create( this._info );

			// construct a new surface
			this._surface = SKSurface.Create( this._info );
		}

		static SkiaImage()
		{
			ContentTypes = new Dictionary<ImageFormat, string>();
			ContentTypes.Add( ImageFormat.Gif, @"image/gif" );
			ContentTypes.Add( ImageFormat.Bmp, @"image/bmp" );
			ContentTypes.Add( ImageFormat.Emf, @"image/emf" );
			ContentTypes.Add( ImageFormat.Exif, @"image/exif" );
			ContentTypes.Add( ImageFormat.Png, @"image/png" );
			ContentTypes.Add( ImageFormat.Icon, @"image/icon" );
			ContentTypes.Add( ImageFormat.Jpeg, @"image/jpeg" );
			ContentTypes.Add( ImageFormat.MemoryBmp, @"image/bmp" );
			ContentTypes.Add( ImageFormat.Tiff, @"image/tiff" );
			ContentTypes.Add( ImageFormat.Wmf, @"image/wmf" );
		}

		#region [ Properties ]

		/// <summary></summary>
		public SKSurface Surface
		{
			get { return this._surface; }
		}

		/// <summary></summary>
		public SKImageInfo Info
		{
			get { return this._info; }
		}

		/// <summary></summary>
		public float BorderThickness { get; set; } = 1.0F;

		/// <summary></summary>
		public SKColor LayoutGridColor { get; set; } = Debug;

		/// <summary></summary>
		public SKColor BackgroundColor { get; set; } = White;

		/// <summary>Draw the layout helper grid</summary>
		public bool LayoutGrid { get; set; } = true;

		/// <summary></summary>
		public SKColor BorderColor { get; set; } = Black;

		#endregion [ Properties ]

		/// <summary>Normailizes a month integer to [1,12]</summary>
		public static int BoundInt(int value, int min, int max)
		{
			if ( max < min )
            {
				int temp = min;
				min = max;
				max = temp;
            }

			if (value < min)
				return min;

			if (value > max)
				return max;

			return value;
		}

		/// <summary>Converts a string to a SKColor</summary>
		public static SKColor GetColor(string hexColor )
        {
			SKColor results = DefaultColor;

			if (String.IsNullOrWhiteSpace( hexColor ) )
				return results;

			SKColor.TryParse( hexColor, out results );

			return results;
		}

		/// <summary>Gets the drawing as a PNG</summary>
		public SKData GetImage(int quality = 75)
		{
			return this._surface.Snapshot().Encode( SKEncodedImageFormat.Png, quality );
		}

		/// <summary></summary>
		/// <returns></returns>
		public abstract void Draw();

		/// <summary>Minimum font that will fit in the label width area</summary>
		/// <returns></returns>
		protected LabelMetrics CalculateLabelWidth(string label, float maxWidth, float startFontSize = 100.0F)
		{
			float minScale = 1000.0F;
			float maxHeight = 0.0F;
			float fontSize = startFontSize;
			float desc = 0.0F;

			using (SKPaint textPaint = new SKPaint() { TextSize = fontSize, IsAntialias = true, IsStroke = false })
			{
				if (label == null || String.IsNullOrWhiteSpace( label ))
					return new LabelMetrics();

				textPaint.Typeface = SKTypeface.FromFamilyName( "Courier" );

				SKRect textBounds = new SKRect();
				textPaint.MeasureText( label, ref textBounds );

				float scale = maxWidth / textBounds.Width;

				if (scale < minScale)
					minScale = scale;

				// reduce the font size using optional scaling
				fontSize = textPaint.TextSize * minScale;
				textPaint.TextSize = fontSize;

				SKFontMetrics metrics;
				textPaint.GetFontMetrics( out metrics );

				if (metrics.Descent > desc)
					desc = metrics.Descent;

				// sets the distance from Ascent to Descent
				if (metrics.Descent - metrics.Ascent > maxHeight)
					maxHeight = metrics.Descent - metrics.Ascent;
			}

			return new LabelMetrics( fontSize, maxHeight, desc );
		}
    }

	/// <summary>Captures sizes needed for the X and Y label from pre calculations for later usage.</summary>
	public struct LabelMetrics
	{
		public LabelMetrics(float fs, float h, float desc)
		{
			this.FontSize = fs;
			this.Height = h;
			this.Descent = desc;
		}

		public static LabelMetrics Default()
		{
			return new LabelMetrics( 10, 10, 1 );
		}

		/// <summary>The calculated font size</summary>
		public float FontSize { get; set; }

		/// <summary>The calculcated height needed from Ascent to Descent to cover the max height of any possible text.</summary>
		public float Height { get; set; }

		/// <summary>The calculcated descent of the font needed</summary>
		public float Descent { get; set; }
	}

}