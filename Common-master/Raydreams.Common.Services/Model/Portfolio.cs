using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Raydreams.Common.Services.Model
{
    /// <summary></summary>
    public class LookupPair
    {
        public LookupPair(Guid id, string name)
        {
            this.ID = id;
            this.Value = (!String.IsNullOrWhiteSpace(name)) ? name.Trim() : String.Empty;
        }

        public LookupPair() : this(Guid.Empty, String.Empty)
        {}

        /// <summary></summary>
        [JsonProperty("id")]
        public Guid ID { get; set; }

        /// <summary></summary>
        [JsonProperty("value")]
        public string Value { get; set; }
    }

    /// <summary></summary>
    [BsonIgnoreExtraElements()]
    public class Portfolio
    {
        public Portfolio(Guid user, string name)
        {
            this.ID = Guid.NewGuid();
            this.OwnedStocks = new List<Stock>();
            this.Watchlist = new List<StockWatch>();

            if (user != Guid.Empty)
                this.User = user;

            if ( !String.IsNullOrWhiteSpace(name) )
                this.Name = name.Trim();
        }

        public Portfolio() : this(Guid.Empty, "My Portfolio")
        { }

        /// <summary>ID of the portfolio</summary>
        [BsonId()]
        [BsonElement("_id")]
        [JsonProperty("id")]
        public Guid ID { get; set; }

        /// <summary>ID of the actual user</summary>
        [BsonElement("userID")]
        [JsonProperty("userID")]
        public Guid User { get; set; }

        /// <summary>The name of the Portfolio</summary>
        [BsonElement("name")]
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary></summary>
        [BsonElement("cash")]
        [JsonProperty("cash")]
        public double Cash { get; set; }

        /// <summary></summary>
        [BsonElement("owned")]
        [JsonProperty("owned")]
        public List<Stock> OwnedStocks { get; set; }

        /// <summary></summary>
        [BsonElement("watched")]
        [JsonProperty("watched")]
        public List<StockWatch> Watchlist { get; set; }
    }

    /// <summary></summary>
    [BsonIgnoreExtraElements()]
    public class Stock
    {
        private string _sym = String.Empty;

        public Stock() : this(String.Empty)
        { }

        public Stock(string symbol)
        {
            this.Symbol = symbol;
            this.SellPrice = 0.0;
            this.Purchases = new List<StockPurchase>();
        }

        /// <summary></summary>
        [BsonElement("symbol")]
        [JsonProperty("symbol")]
        public string Symbol
        {
            get { return this._sym; }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    this._sym = value.Trim().ToUpper();
                }
            }
        }

        /// <summary></summary>
        [JsonProperty("totalShares")]
        public double TotalShares
        {
            get
            {
                if (this.Purchases == null)
                    return 0.0;

                return this.Purchases.Sum(s => s.Shares);
            }
        }

        /// <summary></summary>
        [JsonProperty("realizedValue")]
        public double RealizedValue
        {
            get
            {
                if (this.Purchases == null)
                    return 0.0;

                double shares = 0;
                double total = 0;
                foreach (StockPurchase p in this.Purchases)
                {
                    shares += p.Shares;
                    total += (p.Price * p.Shares);
                }

                return total / shares;
            }
        }

        /// <summary></summary>
        [BsonElement("target")]
        [JsonProperty("target")]
        public double SellPrice { get; set; }

        /// <summary></summary>
        [BsonElement("purchases")]
        public List<StockPurchase> Purchases { get; set; }
    }

    /// <summary></summary>
    [BsonIgnoreExtraElements()]
    public class StockPurchase
    {
        /// <summary></summary>
        [BsonElement("shares")]
        [JsonProperty("shares")]
        public double Shares { get; set; }

        /// <summary></summary>
        [BsonElement("price")]
        [JsonProperty("price")]
        public double Price { get; set; }

        /// <summary></summary>
        [BsonElement("date")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [JsonProperty("date")]
        public DateTime PurchaseDate { get; set; }
    }

    /// <summary></summary>
    [BsonIgnoreExtraElements()]
    public class StockWatch
    {
        private string _sym = String.Empty;

        public StockWatch() : this(String.Empty, 0.0)
        {
        }

        public StockWatch( string symbol ) : this(symbol, 0.0)
        {
        }

        public StockWatch( string symbol, double alert )
        {
            this.Symbol = symbol;
            this.BuyAlert = (alert < 0) ? 0.0 : alert;
            this.Added = DateTime.UtcNow;
        }

        /// <summary></summary>
        [BsonElement("symbol")]
        [JsonProperty("symbol")]
        public string Symbol
        {
            get { return this._sym; }
            set
            {
                if ( !String.IsNullOrWhiteSpace(value) )
                {
                    this._sym = value.Trim().ToUpper();
                }
            }
        }

        /// <summary></summary>
        [BsonElement("added")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [JsonProperty("added")]
        public DateTime Added { get; set; }

        /// <summary></summary>
        [BsonElement("buyAlert")]
        [JsonProperty("buyAlert")]
        public double BuyAlert { get; set; }
    }

}


