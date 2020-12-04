using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Raydreams.Common.Services.Model
{
    /// <summary></summary>
    public enum ProductType
    {
        Unknown = 0,
        Marker = 1,
        ColoredPencil = 2
    }

    /// <summary></summary>
    public enum ColorSpace
    {
        Unknown = 0,
        Gray = 1,
        RGB = 2,
        ARGB = 3,
        CMYK = 4,
        Hex = 5,
        Lab = 6
    }

    /// <summary></summary>
    public class ProductLine
    {
        public ProductLine()
        {
            this.Items = new List<ProductItem>();
        }

        /// <summary></summary>
        [JsonProperty( "key" )]
        public string Key { get; set; }

        /// <summary>/summary>
        [JsonProperty( "Company" )]
        public string Company { get; set; }

        /// <summary>/summary>
        [JsonProperty( "productName" )]
        public string ProductName { get; set; }

        /// <summary>/summary>
        [JsonProperty( "Description" )]
        public string Description { get; set; }

        /// <summary>/summary>
        [JsonProperty( "mediaType" )]
        [JsonConverter( typeof( StringEnumConverter ) )]
        public ProductType MediaType { get; set; }

        /// <summary>/summary>
        [JsonIgnore]
        public ColorSpace ColorSpace { get; set; }

        /// <summary>/summary>
        [JsonProperty( "order" )]
        public int Order { get; set; }

        /// <summary>/summary>
        [JsonIgnore]
        public string SortField { get; set; }

        /// <summary>/summary>
        [JsonProperty( "items" )]
        public List<ProductItem> Items { get; set; }
    }

    /// <summary></summary>
    public static class ProductLines
    {
        /// <summary></summary>
        /// <returns></returns>
        public static Dictionary<string, ProductLine> Get()
        {
            Dictionary<string, ProductLine> lines = new Dictionary<string, ProductLine>()
            {
                { "copicSketch", new ProductLine() { Key="copicSketch", Company="Copic", ProductName="Sketch", Description="Copic Sketch", MediaType= ProductType.Marker, ColorSpace=ColorSpace.Hex, Order=1, SortField="code" }  },
                { "copicCiao", new ProductLine() { Key="copicCiao", Company="Copic", ProductName="Ciao", Description="Copic Ciao", MediaType=ProductType.Marker, ColorSpace=ColorSpace.Hex, Order=2, SortField="code" }  },
                { "copicClassic", new ProductLine() { Key="copicClassic", Company="Copic", ProductName="Classic", Description="Copic Classic", MediaType=ProductType.Marker, ColorSpace=ColorSpace.Hex, Order= 3, SortField="code" }  },
                { "copicWide", new ProductLine() { Key="copicWide", Company="Copic", ProductName="Wide", Description="Copic Wide", MediaType=ProductType.Marker, ColorSpace=ColorSpace.Hex, Order=4, SortField="code" }  },
                { "prismaPM", new ProductLine() { Key="prismaPM", Company="Prismacolor", ProductName="Premier", Description="Prismacolor Premier Chisel", MediaType=ProductType.Marker, ColorSpace=ColorSpace.CMYK, Order=5, SortField="name" }  },
                { "prismaPB", new ProductLine() { Key="prismaPB", Company="Prismacolor", ProductName="Premier", Description="Prismacolor Premier Brush", MediaType=ProductType.Marker, ColorSpace=ColorSpace.CMYK, Order=6, SortField="name" }  },
                { "prismaPC", new ProductLine() { Key="prismaPC", Company="Prismacolor", ProductName="Softcore", Description="Prismacolor Softcore", MediaType=ProductType.ColoredPencil, ColorSpace=ColorSpace.CMYK, Order=7, SortField="name" }  },
                { "polychromos", new ProductLine() { Key="polychromos", Company="Faber-Castell", ProductName="Polychromos", Description="Faber-Castell Polychromos", MediaType=ProductType.ColoredPencil, ColorSpace=ColorSpace.CMYK, Order=8, SortField="name" }  },
                { "spectrumMarkers", new ProductLine() { Key="spectrumMarkers", Company="Spectrum Noir", ProductName="Marker", Description="Spectrum Noir Markers", MediaType=ProductType.Marker, ColorSpace=ColorSpace.Hex, Order=9, SortField="code" }  },
                { "letrasetTria", new ProductLine() { Key="letrasetTria", Company="Letraset", ProductName="Tria", Description="Letraset Tria",MediaType=ProductType.Marker, ColorSpace=ColorSpace.Hex, Order=15, SortField="code" }  },
                { "winsorPromarker", new ProductLine() { Key="winsorPromarker", Company="Winsor & Newton", ProductName="ProMarker", Description="Winsor & Newton ProMarker", MediaType=ProductType.Marker, ColorSpace=ColorSpace.Hex, Order=17, SortField="name" }  },
                { "winsorBrushmarker", new ProductLine() { Key="winsorBrushmarker", Company="Winsor & Newton", ProductName="BrushMarker", Description="Winsor & Newton BrushMarker", MediaType=ProductType.Marker, ColorSpace=ColorSpace.Hex, Order=16, SortField="name" }  },
                { "blickBrush", new ProductLine() { Key="blickBrush", Company="Blick", ProductName="Brush Markers", Description="Blick Brush Markers", MediaType=ProductType.Marker, ColorSpace=ColorSpace.Hex, Order=18, SortField="name" }  },
                { "touchTwinMarker", new ProductLine() { Key="touchTwinMarker", Company="ShinHan", ProductName="Touch Twin", Description="Touch Twin Markers", MediaType=ProductType.Marker, ColorSpace=ColorSpace.Hex, Order=10, SortField="code" }  },
                { "touchTwinBrush", new ProductLine() { Key="touchTwinBrush", Company="ShinHan", ProductName="Touch Twin Brush", Description="Touch Twin Brush Markers", MediaType=ProductType.Marker, ColorSpace=ColorSpace.Hex, Order=11, SortField="code" }  },
                { "tombrowDualBrush", new ProductLine() { Key="tombrowDualBrush", Company="Tombrow", ProductName="Dual Brush Pen", Description="Tombrow Dual Brush Pen",MediaType=ProductType.Marker, ColorSpace=ColorSpace.Hex, Order=12, SortField="name" }  },
                { "zigBM", new ProductLine() { Key="zigBM", Company="Zig", ProductName="Brush Markers", Description="Zig Brush Markers",MediaType=ProductType.Marker, ColorSpace=ColorSpace.Hex, Order=19, SortField="name" }  },
                { "artistsLoftDTS", new ProductLine() { Key="artistsLoftDTS", Company="Artist's Loft", ProductName="Dual Tip Sketch", Description="Artist's Loft Dual Tip Sketch",MediaType=ProductType.Marker,ColorSpace=ColorSpace.RGB, Order=20, SortField="code"  }  }
            };

            return lines;
        }
    }
}
