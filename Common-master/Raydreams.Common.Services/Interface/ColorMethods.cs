using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Raydreams.Common.Logic;
using Raydreams.Common.Services.Data;
using Raydreams.Common.Services.Model;

namespace Raydreams.Common.Services.Interface
{
	/// <summary></summary>
	public partial class CommonGateway : ICommonGateway
	{
		/// <summary></summary>
		public List<ProductLine> GetProductLines()
        {
			Dictionary<string, ProductLine> lines = ProductLines.Get();

			// sort by the order property and return
			return lines.Values.OrderBy( i => i.Order ).ToList();
		}

		/// <summary></summary>
        /// <returns></returns>
		public List<ProductItem> GetProductItems()
        {
			return this.AzureRepo.GetAll();
        }

		/// <summary></summary>
		public ProductLine MatchColor( string lineKey, Color color, int top = 10 )
		{
			if ( String.IsNullOrWhiteSpace(lineKey) )
				return null;

			// get the product line
			Dictionary<string, ProductLine> lines = ProductLines.Get();

			if ( !lines.ContainsKey(lineKey) )
				return null;

			ProductLine line = lines[lineKey];

			// get Lab of input color
			double[] unknown = ColorConverters.ColorToLab( color );

			// query the product items from the repo
			List<ProductItem> items = this.AzureRepo.GetAllByLine(lineKey);

			// iterate each item and calc DeltaE
			foreach (ProductItem item in items)
            {
				item.DeltaE = ColorConverters.CalcDeltaE(item.Lab, unknown);
			}

			// sort and take top
			line.Items = items.OrderBy( p => p.DeltaE ).Take( top ).ToList();

			return line;
		}
	}
}
