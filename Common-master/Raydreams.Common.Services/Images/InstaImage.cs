using System;
using System.Collections.Generic;
using Raydreams.Common.Logic;
using SkiaSharp;
using SkiaTextRenderer;

namespace Raydreams.Common.Services.Images
{
	/// <summary></summary>
    public class InstaImage : SkiaImage
    {
		#region [ Fields ]

		#endregion [ Fields ]

		#region [ Constructors ]

		/// <summary></summary>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		public InstaImage(int size) : base (size, size)
		{
		}

		#endregion [ Constructors ]

		/// <summary></summary>
		public string Message { get; set; } = "The Quick Brown Fox Jumped Over The Lazy Dog!";

		/// <summary></summary>
		public SKColor TextColor { get; set; } = White;

		/// <summary></summary>
		public SKColor StarColor { get; set; } = White;

		/// <summary></summary>
		public string FontName { get; set; } = "Arial Black";

		/// <summary></summary>
		public float FontSize { get; set; } = 100.0F;

		/// <summary></summary>
		public int StarDensity { get; set; } = 50;

		#region [ Calculated Properties ]

		/// <summary>The origin/center of the graph canvas itself</summary>
		protected SKPoint Origin { get; set; }

		/// <summary>Rect inside the border</summary>
		protected SKRect InsetRect { get; set; }

		#endregion [ Calculated Properties ]

		/// <summary></summary>
		public override void Draw()
        {
			// get the canvas from the surface
			SKCanvas canvas = this._surface.Canvas;

			// clear the canvas
			canvas.Clear();

			// base calculations
			this.CalcFrame();

			// paint the background
			canvas.DrawRect( 0, 0, this.Info.Width - 1, this.Info.Height - 1, new SKPaint() { Color = this.BackgroundColor, StrokeWidth = 0, Style = SKPaintStyle.StrokeAndFill } );

			// draw stars
			this.DrawStars( canvas );

			// draw the label centered
			LabelMetrics font = this.CalculateLabelWidth( this.Message, ((float)this.Info.Width) - ((float)Math.Ceiling(this.BorderThickness * 2.0F)) );
			//this.DrawLabel( font, canvas );

			this.DrawText( this.Message, canvas );

			// paint the border
			canvas.DrawRect( 0, 0, this.Info.Width - 1, this.Info.Height - 1, new SKPaint() { Color = this.BorderColor, StrokeWidth = this.BorderThickness, Style = SKPaintStyle.Stroke } );

			// draw the debug grid
			if (this.LayoutGrid)
				this.DrawLayout( canvas );
		}

		/// <summary>Calculate major anchor points</summary>
		/// <param name="info"></param>
		/// <param name="canvas"></param>
		private void CalcFrame()
		{
			// center of the drawing area
			this.Origin = new SKPoint( this.Info.Width / 2.0F, this.Info.Height / 2.0F );

			this.InsetRect = new SKRect( this.BorderThickness, this.BorderThickness, this.Info.Width - this.BorderThickness, this.Info.Height - this.BorderThickness );
		}

		/// <summary>Draws the test layout area</summary>
		/// <param name="info"></param>
		/// <param name="canvas"></param>
		private void DrawLayout(SKCanvas canvas)
		{
			using (SKPaint debugPaint = new SKPaint() { Color = this.LayoutGridColor, StrokeWidth = 1, Style = SKPaintStyle.Stroke, StrokeCap = SKStrokeCap.Round })
			{
				// draw cross hairs
				canvas.DrawLine( this.Origin.X, 0, this.Origin.X, this.Info.Height, debugPaint );
				canvas.DrawLine( 0, this.Origin.Y, this.Info.Width, this.Origin.Y, debugPaint );

				debugPaint.StrokeWidth = 20;

				// draw origin and control area frame
				canvas.DrawPoint( this.Origin, debugPaint );
			}
		}

		/// <summary></summary>
        /// <param name="canvas"></param>
		private void DrawText(string text, SKCanvas canvas)
        {
			TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak;

			//float size = this.CalculateMaxFontSize(this.Message, flags);

			Font font = new Font( SKTypeface.FromFamilyName( this.FontName ), this.FontSize, FontStyle.Bold );

			SKSize s = TextRendererSk.MeasureText( text, font, this.InsetRect.Width, flags );

			TextRendererSk.DrawText( canvas, text, font, this.InsetRect, this.TextColor, flags);
		}

		/// <summary></summary>
        /// <param name="text"></param>
        /// <param name="flags"></param>
        /// <param name="startFontSize"></param>
        /// <returns></returns>
		protected float CalculateMaxFontSize(string text, TextFormatFlags flags, float startFontSize = 1000.0F)
        {
			float curSize = startFontSize;

			Font font = new Font( SKTypeface.FromFamilyName( this.FontName ), curSize, FontStyle.Bold );
			SKSize s = TextRendererSk.MeasureText( text, font, this.InsetRect.Width, flags );

			// if height exceeds insetRect height we need to make it smaller
			if (s.Width > this.InsetRect.Width)
				return ( this.InsetRect.Width / s.Width ) * startFontSize;
			else
				return startFontSize;
		}

		/// <summary>Draws the X axis labels</summary>
		private void DrawLabel(LabelMetrics label, SKCanvas canvas)
		{
			// draw prime label
			if (!String.IsNullOrWhiteSpace( this.Message ))
			{
				using (SKPaint textPaint = new SKPaint() { Color = this.TextColor, TextSize = label.FontSize, IsAntialias = true })
				{
					textPaint.TextAlign = SKTextAlign.Center;

					SKRect rect = new SKRect( 0, this.Origin.Y - ( label.Height / 2.0F ), this.Info.Width, this.Origin.Y + ( label.Height / 2.0F ) );

					// show the label rect
					if (this.LayoutGrid)
						canvas.DrawRect( rect, new SKPaint() { Color = Debug, StrokeWidth = 1, Style = SKPaintStyle.Stroke } );

					// draw the text centered on X
					canvas.DrawText( this.Message, rect.MidX, rect.Bottom - label.Descent, textPaint );
				}
			}

		}

		/// <summary>Draws the stars</summary>
		private void DrawStars(SKCanvas canvas)
        {
			if (this.StarDensity <= 0)
				return;

			if (this.StarDensity > 100)
				this.StarDensity = 100;

			double den = ((double)this.StarDensity) / 10000.0;

			Randomizer rnd = new Randomizer();
			Point[] stars = rnd.RandomPoints( this.Info.Width, this.Info.Height, den );

			using (SKPaint paint = new SKPaint() { Color = this.StarColor, Style = SKPaintStyle.Stroke, StrokeCap = SKStrokeCap.Round })
            {
				// min/max star sizes
				double min = this.Info.Width * 0.0010;
				double max = this.Info.Width * 0.0050;

				foreach (Point pt in stars)
				{
					double d = rnd.RandomDouble();

					// set diameter between .0025 to .0075% of the dimensions
					paint.StrokeWidth = (float)(( ( max - min ) * d ) + min);
					canvas.DrawPoint( new SKPoint((float)pt.X, (float)pt.Y), paint );
				}
			}

			// calculate a star size based on 1/10th the width of the image
			double starRad = this.Info.Width / 10.0;
			Point[] poly = Shapes.Star( starRad );

			// pick a random origin
			int thick = (int)(Math.Ceiling( this.BorderThickness ) + starRad );
			int newX = rnd.RandomInt( thick, this.Info.Width - thick );
			int newy = rnd.RandomInt( thick, this.Info.Height - thick );

			// translate all the points to the new origin
			poly = Shapes.Translate( new Point(newX, newy), poly );

			// draw the big star
			using (SKPaint paint = new SKPaint { IsAntialias = true, Style = SKPaintStyle.StrokeAndFill, Color = this.BorderColor, StrokeWidth = 1 })
			{
				SKPath bigStar = new SKPath { FillType = SKPathFillType.EvenOdd };
				bigStar.MoveTo( new SKPoint( (float)poly[0].X, (float)poly[0].Y ) );

				for (int i = 1; i < poly.Length; ++i)
				{
					SKPoint cur = new SKPoint( (float)poly[i].X, (float)poly[i].Y );
					bigStar.LineTo( cur );
				}

				bigStar.Close();
				canvas.DrawPath( bigStar, paint );
			}

		}
	}
}
