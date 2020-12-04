using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raydreams.Common.IO
{
	/// <summary>Defines a delegate method that can be used to parse a CSV line</summary>
	public delegate string[] TextLineParser( string line );

	/// <summary>A temp class for holding CSV specific helper utilities</summary>
	public static class ParserUtil
	{
		/// <summary>
		/// Sniffer the specified path.
		/// </summary>
		/// <returns>The sniffer.</returns>
		/// <param name="path">Path.</param>
		public static Delim Sniffer( string path )
		{
			Delim results = Delim.Unknown;

			if ( String.IsNullOrWhiteSpace( path ) )
				return results;

			FileInfo fi = new FileInfo( path );

			if ( !fi.Exists )
				return results;

			using ( StreamReader reader = new StreamReader( path, Encoding.UTF8 ) )
			{	
				string next = reader.ReadLine();

				if ( !String.IsNullOrWhiteSpace( next ) )
				{
					string[] tabs = next.Split( new char[] { '\t' }, StringSplitOptions.None );
					string[] commas = next.Split( new char[] { ',' }, StringSplitOptions.None );

					results = (tabs.Length > commas.Length) ? Delim.Tab : Delim.Comma;
				}
			}

			return results;
		}

		/// <summary>Processes is a single line comma delimited line using CSV parsing formats</summary>
		/// <param name="line"></param>
		/// <returns>Values of the line in a string array</returns>
		/// <remarks>
		/// " ","HESSPW00001073"," ","10100055abcd","Active","Leslie","McCain","Leslie.mccain@hrgworldwide.com","Business Manager, HRG North America"," ","00609300_Hess","00609300","10005206","Strategic Sourcing & Category Mgmt - Indirects","T589","TAPFIN PW"," ",,"Procurement ","AA40011290"," ","03/23/2015",," ","60000951","TX-Houston-1501 McKinney Street","92000025","Hr","No","lmccain","No","Yes","No","No","Yes"
		/// AX40JS00000003,HESSWK00000003,z12082320345436806102a71,01055940heja,Active,Jamie,Herrick,jherrick@hess.com,Contract Analyst - Hourly,HESSWO00000003,00031201_Hess,00031201,10005732,OAWA - Proj Stampede Procurement,AX40,Administrative Exchange Inc.,HESS,,GOM Projects Supply Chain,AP40012430,0.00,01/01/2016,12/31/2016,USD,60000951,TX-Houston-1501 McKinney Street,00609136,Hr,Yes,jherrick,No,No,No,No,
		/// </remarks>
		public static string[] CSVLineReader( string line )
		{
			StringBuilder temp = new StringBuilder();
			List<string> values = new List<string>();

			// walk the string
			for (int i = 0; i < line.Length;)
			{
				// if the record starts on a quote, then it ends on the next quote
				if (line[i] == '"')
				{
					++i; // move off the current "

					// continue to the last quote
					while (line[i] != '"')
					{
						temp.Append( line[i++] );
					}

					// found an end
					values.Add( temp.ToString().Trim() );
					temp.Length = 0;
					i += 2;
				}
				else if (line[i] == ' ') // stray space
				{ ++i; }
				else if (line[i] == ',') // found two ,, next to each other or with only space between
				{
					++i;
					values.Add( String.Empty );
				}
				else // non token case, end on the next ,
				{
					// continue to the last ,
					while ( i < line.Length && line[i] != ',')
					{
						temp.Append( line[i++] );
					}

					// found an end
					values.Add( temp.ToString().Trim() );
					temp.Length = 0;

					// look from here to EOL of line and see if there is anthing else
					if ( i == line.Length - 1 && line[i] == ',' )
						values.Add( String.Empty );

					i += 1;
				}
			}

			return values.ToArray();
		}

		/// <summary>
		/// Tabbeds the line reader.
		/// </summary>
		/// <returns>The line reader.</returns>
		/// <param name="line">Line.</param>
		public static string[] TabbedLineReader( string line )
		{
			string[] values = line.Split( new char[] { '\t' }, StringSplitOptions.None );

			Array.ForEach<string>( values, s => s = s.Trim() );

			return values;
		}

	}

}
