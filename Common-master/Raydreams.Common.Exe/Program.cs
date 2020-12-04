using System;
using System.Collections;
using System.Collections.Generic;
using Raydreams.Common.Logic;
using Raydreams.Common.Services.Data;
using Raydreams.Common.Services.Model;
using Newtonsoft.Json;

namespace Raydreams.Common.Exe
{
    class Program
    {
        public static readonly string DesktopPath = Environment.GetFolderPath( Environment.SpecialFolder.DesktopDirectory );
        // "/Users/lobsterpixel/Desktop/";

        //public static string connStr = @"DefaultEndpointsProtocol=https;AccountName=formencryptprod001;AccountKey=aBf93ZB4q3rADvz1LJ2oGO0BpsIQv+WYqEWBBVGkS29BPGHqBxXw9juNLGnxyjYWC3ZuwmJN6pVxn5kvjR4Xqg==;EndpointSuffix=core.windows.net";
        public static string connStr = @"DefaultEndpointsProtocol=https;AccountName=raydreamsdevsa002;AccountKey=VHRzONoual1tKzfOUEqhDHmPzBb/GUZ9uyLDv9kj698JcQAkIELDx6K/LtDXOIrUpmeJIrWECj493eyOpSCldQ==;EndpointSuffix=core.windows.net";

        static void Main(string[] args)
        {
            Program app = new Program();
            //app.LoadInAzure();
            //app.WriteToJsonFile();

            Console.WriteLine("Done");
        }

        /// <summary></summary>
        public Program()
        {
        }

        /// <summary>ETL from SQLite to Azure Table data</summary>
        public void LoadInAzure()
        {
            List<ProductItem> items = this.ReadFromSQLite();
            Dictionary<string, ProductLine> lines = this.Sort( items );

            this.WriteProductItems( items );
        }

        /// <summary>Write from Azure to a local JSON file</summary>
        private void WriteToJsonFile()
        {
            MarkerAzureRepo repo = new MarkerAzureRepo( connStr, "ProductItems" );

            // get the product line
            Dictionary<string, ProductLine> lines = ProductLines.Get();

            List<ProductLine> results = new List<ProductLine>();

            foreach ( KeyValuePair<string, ProductLine> kvp in lines)
            {
                List<ProductItem> items = repo.GetAllByLine(kvp.Key);
                lines[kvp.Key].Items = items;
                results.Add( lines[kvp.Key] );
            }

            string data = JsonConvert.SerializeObject( results );
            string file = $"{DesktopPath}/items.json";

            System.IO.File.WriteAllText( file , data );
        }

        /// <summary></summary>
        /// <returns></returns>
        private List<ProductItem> ReadFromSQLite()
        {
            ProductItemsRepository dm = new ProductItemsRepository( @"Data Source=/Users/tag/Desktop/MarkerMatch-en.db" )
            {
                TableName = "ProductItems"
            };

            List<ProductItem> items = dm.SelectAll();

            return items;
        }

        /// <summary>Sort into Lines hash and set Color based on colorpsace</summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private Dictionary<string, ProductLine> Sort( List<ProductItem> items )
        {
            Dictionary<string, ProductLine> lines = ProductLines.Get();

            foreach ( ProductItem item in items )
            {
                ProductLine line = lines[item.Line];

                // set actual color object
                if ( line.ColorSpace == ColorSpace.Hex )
                    item.Color = ColorConverters.HexToColor( item.Hex );
                else if ( line.ColorSpace == ColorSpace.RGB )
                    item.Color = ColorConverters.RGBStringToColor( item.RGB );
                else if ( line.ColorSpace == ColorSpace.CMYK )
                    item.Color = ColorConverters.CMYKStringToColor( item.CMYK );

                // calc lab
                item.Lab = ColorConverters.ColorToLab( item.Color );

                // check for uncalced values
                if ( String.IsNullOrWhiteSpace( item.CMYK ) || item.CMYK == "0,0,0,0" )
                    item.CMYK = ColorConverters.ColorToCMYKStr( item.Color );

                if ( String.IsNullOrWhiteSpace( item.RGB ) )
                {
                    item.RGB = ColorConverters.ColorToRGBStr( item.Color );
                }

                if ( String.IsNullOrWhiteSpace( item.Hex ) )
                    item.Hex = ColorConverters.ColorToHexStr( item.Color );

                line.Items.Add( item );
            }

            return lines;
        }

        /// <summary></summary>
        /// <param name="items"></param>
        private void WriteProductItems( List<ProductItem> items )
        {
            MarkerAzureRepo repo = new MarkerAzureRepo( connStr, "ProductItems" );
            repo.WriteProductItems( items );
        }

    }   
}
