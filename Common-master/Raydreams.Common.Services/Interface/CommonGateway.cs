using System;
using System.Reflection;
using Raydreams.Common.Logging;
using Raydreams.Common.Model;
using Raydreams.Common.Services.Config;
using Raydreams.Common.Services.Model;
using Raydreams.Common.Services.Data;
using Raydreams.Common.Services.Security;
using Raydreams.Common.Extensions;
using System.Linq;
using System.Collections.Generic;
using Raydreams.Common.Logic;
using Raydreams.Common.Security;
using System.Threading.Tasks;

namespace Raydreams.Common.Services.Interface
{
	/// <summary>The Common API Gateway</summary>
	public partial class CommonGateway : ICommonGateway
    {
		#region [ Fields ]

		/// <summary>define a fall back domain to use</summary>
		public const string DefaultDomain = "formencrypt.com";

		/// <summary>The logger to use will use null logger if non is set</summary>
		private ILogger _logger = null;

		/// <summary>Default environment</summary>
		private EnvironmentType _config = EnvironmentType.Unknown;

		/// <summary>Default token hint</summary>
		//private string _hint = "please";

		/// <summary>Session timeout (secs) defaults to one hour</summary>
		private int _timeout = 3600 * 1;

		/// <summary>Set to true to load mock data</summary>
		/// <remarks>Currently not used</remarks>
		private bool _isMock = false;

		#endregion [ Fields ]

		#region [ Constructor ]

		public CommonGateway( EnvironmentSettings env, string domain )
		{
			// validate the domain
			if ( String.IsNullOrWhiteSpace( domain ) )
				domain = DefaultDomain;
			else
				domain = domain.Trim();

			// if you need to register the GUID serializer before any use
			//BsonSerializer.RegisterSerializer( new GuidSerializer( GuidRepresentation.Standard ) );

			this.Config = env;

			this.SettingsRepo = new SettingsRepository<Settings>( this.Config.ConnectionString, this.Config.Database, TableNames.Settings );
			this.UsersRepo = new UsersRepository( new BCryptPasswordMaker(), this.Config.ConnectionString, this.Config.Database, TableNames.Users );
			this.SessionRepo = new MongoSessionsRepo( this.Config.ConnectionString, this.Config.Database, TableNames.Users );
			this.StocksRepo = new PortfoliosRepository( this.Config.ConnectionString, this.Config.Database, TableNames.Portfolios);
			this.ResetRepo = new ResetRequestsRepository( this.Config.ConnectionString, this.Config.Database, TableNames.Users );
			this.CMSRepo = new PagesRepository( this.Config.ConnectionString, this.Config.Database, TableNames.ContentPages );
			this.AzureRepo = new MarkerAzureRepo( this.Config.AzureTableConnection, "ProductItems" );
			this.TagsRepo = new TagsRepository( this.Config.ConnectionString, this.Config.Database, TableNames.Tags );


			// always get the settings from some domain
			this.Settings = this.SettingsRepo.GetByDomain( domain );

			// setup the logger
			this.Logger = new MongoLogger( this.Config.ConnectionString, this.Config.Database, TableNames.Logs, domain)
			{
				Level = LogLevel.All
			};
		}

		#endregion [ Constructor ]

		#region [ Runtime Properties ]

		/// <summary>The user currently logged in to the gateway</summary>
		protected User CurrentUser { get; set; }

		/// <summary>The user session</summary>
		public Session CurrentSession { get; protected set; }

		#endregion [ Runtime Properties ]

		#region [ Properties ]

		// <summary></summary>
		protected EnvironmentSettings Config { get; set; }

		// <summary></summary>
		protected TokenPayload Token { get; set; }

		// <summary></summary>
		protected Settings Settings { get; set; }

		// <summary></summary>
		protected IUsersRepository UsersRepo { get; set; }

		// <summary></summary>
		protected ISessionsRepository SessionRepo { get; set; }

		// <summary></summary>
		protected PortfoliosRepository StocksRepo { get; set; }

		// <summary></summary>
		protected IResetRequestsRepository ResetRepo { get; set; }

		// <summary></summary>
		protected MarkerAzureRepo AzureRepo { get; set; }

		// <summary></summary>
		protected SettingsRepository<Settings> SettingsRepo { get; set; }

		// <summary></summary>
		protected PagesRepository CMSRepo { get; set; }

		// <summary></summary>
		protected TagsRepository TagsRepo { get; set; }

		// <summary></summary>
		//protected string TokenHint
		//{
		//	get { return this._hint; }
		//	set { this._hint = value; }
		//}

		// <summary>The session timeout in seconds</summary>
		protected int Timeout
		{
			get
			{
				if ( this.Settings != null )
					return this.Settings.SessionTimeout;

				return this._timeout;
			}
		}

		// <summary>The default logger. Use NullLoger if none specified.</summary>
		protected ILogger Logger
		{
			get
			{
				if ( this._logger == null )
					return new NullLogger();

				return this._logger;
			}
			set
			{
				this._logger = value;
			}
		}

		#endregion [ Properties ]

		#region [ Methods ]

		/// <summary>Replaces the normal repos with their mock versions</summary>
		public void LoadMocks()
		{
			this._isMock = true;

			this.UsersRepo = new UsersMockRepo();
			this.SessionRepo = new SessionsMockRepo();

			// setup the logger
			this.Logger = new NullLogger( LogLevel.All );
		}

		/// <summary></summary>
		public string CreateCharPIN(string fname, string lname, string mname = null)
        {
			if (String.IsNullOrWhiteSpace( fname ) && String.IsNullOrWhiteSpace( lname ))
				return null;

			CharPINGenerator.VulgarWords = ProfanityDictionary.All;
			CharPINGenerator logic = new CharPINGenerator( 5 );

			return logic.Generate( fname, lname, mname );
        }

		#endregion [ Methods ]

		#region [ Logging ]

		/// <summary>Just returns a simple signature string for testing</summary>
		/// <returns></returns>
		public string Signature()
		{
			// default values
			string version = "unknown";

			// interate through the assembly attributes to get version
			object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes( false );

			foreach ( object attr in attributes )
			{
				if ( attr is AssemblyFileVersionAttribute )
					version = ((AssemblyFileVersionAttribute)attr).Version;
			}

			// create the signature
			return  $"Service : {this.GetType().FullName}; Version : {version}; Env : {this.Config.EnvironmentType}; ";
		}

		/// <summary>Get the settings for the specified domain</summary>
		/// <param name="domain"></param>
		/// <returns></returns>
		public Settings GetSettings()
		{
			return this.Settings;
		}

		/// <summary>Public logger</summary>
		public void Log( string message, string category, string level = null )
        {
			// parse the level
			LogLevel lvl = LogLevel.Info;

			if ( String.IsNullOrWhiteSpace( level ) )
				level.TryParseToEnum<LogLevel>( out lvl, true );

			this.Log( message, category, lvl );
		}

		/// <summary>Log a message the standard logger on a background task</summary>
		protected void Log( string message, string category, LogLevel level = LogLevel.Info )
		{
			if ( String.IsNullOrWhiteSpace( message ) )
				return;

			// run the logging task
			Task.Run( () => {

				// default category
				category = ( !String.IsNullOrWhiteSpace( category ) ) ? category.Trim() : "None";

				// log it
				this.Logger.Log( message.Trim(), category, level );
			} );
		}

		/// <summary>Log an exception the standard logger on a background task</summary>
		protected void Log( System.Exception exp )
		{
			// run the logging task
			Task.Run( () => {
				this.Logger.Log( exp );
			} );
		}

		#endregion [ Logging ]

		#region [ Stock Methods ]

		/// <summary></summary>
		public List<LookupPair> GetPortfolioList()
        {
			if ( this.CurrentUser == null )
				return new List<LookupPair>();

			return this.StocksRepo.GetNamesByUser( this.CurrentUser.ID );
        }

		/// <summary>Get the logged in users portfolio</summary>
        /// <param name="name">Name of the portfolio to get</param>
		public Portfolio GetPortfolio(string name)
        {
			if ( this.CurrentUser == null )
				return null;

            Portfolio p = this.StocksRepo.GetByUser(this.CurrentUser.ID, name);

			p.OwnedStocks = p.OwnedStocks.OrderBy(s => s.Symbol).ToList();
            p.Watchlist = p.Watchlist.OrderBy(s => s.Symbol).ToList();

			return p;
        }

		/// <summary></summary>
		public int InsertWatchedStock(string name, string symbol, double price)
        {
			if ( this.CurrentUser == null )
				return 0;

			return this.StocksRepo.InsertOrUpdateWatched( this.CurrentUser.ID, name, new StockWatch(symbol) { BuyAlert = price } );
		}

		/// <summary></summary>
		public bool DeleteWatchedStock(string name, string symbol)
        {
			if ( this.CurrentUser == null )
				return false;

			return this.StocksRepo.DeleteWatched( this.CurrentUser.ID, name, symbol);
		}

		/// <summary></summary>
		public bool InsertPortfolio( string name )
        {
			if ( this.CurrentUser == null || String.IsNullOrWhiteSpace(name))
				return false;

			Guid userID = this.CurrentUser.ID;

			// check for max lists
			//UserPreferences prefs = this.UsersRepo.GetPreferences(userID);

			// need to know how many portfolios this user already has

			Portfolio port = new Portfolio(userID, name);
			port.Watchlist = new List<StockWatch>(new StockWatch[] { new StockWatch("AAPL",300), new StockWatch("XOM", 45), new StockWatch("WMT", 120) });

			return this.StocksRepo.Insert(port);
        }

		#endregion [ Stocks Methods ]

		#region [ Mailing ]

		/// <summary>Sends all the emails from one function</summary>
		/// <param name="template"></param>
		/// <param name="values"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		//public bool MailIt( EmailTemplate template, Dictionary<string, string> values, string[] to )
		//{
		//	if ( template == null || to == null || to.Length < 1 )
		//		return false;

		//	IMailer mailer = this.Settings.Mail.Enabled ? (IMailer)new SendGridMailer( this.Settings.Mail.Key ) : (IMailer)new LogMailer( this.Logger );
		//	mailer.To = (String.IsNullOrWhiteSpace( this.Settings.Mail.DebugTo )) ? to : new string[] { this.Settings.Mail.DebugTo };
		//	mailer.IsHTML = true;

		//	bool sent = mailer.Send( this.Settings.Mail.From, template.Subject, Templator.Prepare( template.Template, values ) );

		//	return sent;
		//}

		#endregion [ Mailing ]


	}
}
