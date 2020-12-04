using System;
using System.Collections.Generic;
using MongoDB.Bson;
using System.Linq;
using Raydreams.Common.Services.Model;

namespace Raydreams.Common.Services.Data
{
    /// <summary>Mock data for demo accounts</summary>
    public static class MockData
    {
        static MockData()
        {
            Guid id = LoadUsers();
            LoadSessions(id);
        }

        /// <summary>All Mock Accounts</summary>
        public static List<User> Users = new List<User>();

        /// <summary>All Mock Account PWs</summary>
        public static Dictionary<Guid, string> PWs = new Dictionary<Guid, string>();

        public static List<Session> Sessions = new List<Session>();

        /// <summary>Returns all the Mock User IDs</summary>
        public static List<string> MockUserIDs
        {
            get
            {
                return Users.Select( a => a.UserID ).ToList();
            }
        }

        /// <summary>Test if the User ID is a mock account</summary>
        public static bool IsMockUser( string id )
        {
            if ( String.IsNullOrWhiteSpace( id ) )
                return false;

            id = id.Trim();

            User acct = Users.Where( a => a.UserID.Equals( id, StringComparison.InvariantCultureIgnoreCase ) ).FirstOrDefault();

            return ( acct != null );
        }

        /// <summary>Loads all the Mock accounts</summary>
        private static Guid LoadUsers()
        {
            //Users = new List<User>();

            //User acct1 = new User()
            //{
            //    ID = new Guid( "e5cfae73-d823-b749-9280-05e008fa2e65" ),
            //    UserID = "jbob",
            //    Name = "Joe Bob",
            //    Email = "tguillory@bouncingpixel.com",
            //    Created = DateTime.Parse( "2019-09-16T19:00:34.706Z" ),
            //    LastLogin = DateTime.Parse( "2020-05-15T21:17:04.408Z" ),
            //    Updated = DateTime.Parse( "2020-05-15T21:17:04.408Z" ),
            //    ResetPW = false,
            //    Enabled = true,
            //    Preferences = null,
            //    Role = "user",
            //    FailedLogins = new List<DateTime>(),
            //};
            //Users.Add( acct1 );

            //// Password1
            //PWs.Add( acct1.ID, "$2b$10$hNvGHXtoVyYLiEcTnGpcxeMM8X7M.k.4sbZFJfP8G2ZnmZ3CRF01." );

            //return acct1.ID;

            return Guid.Empty;
        }

        /// <summary></summary>
        private static void LoadSessions(Guid userID)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            Sessions = new List<Session>
            {
                new Session()
                {
                    ID = Guid.NewGuid(),
                    UserID = userID,
                    Domain = "raydreams.com",
                    Created = now.AddDays( -1 ).UtcDateTime,
                    LastModified = now.AddHours( -1 ).UtcDateTime,
                    IPAddress = "192.168.1.100",
                    Salt = "ABCD1234"
                }
            };
        }

        /// <summary>Some fake companies to choose from</summary>
		private static readonly string[] _Companies = new string[]
        {
            "United Parcel Service",
            "Gucci",
            "Microsoft",
            "Cisco Systems Inc",
            "Morgan Stanley",
            "Pizza Hut",
            "Porsche",
            "Intel Corporation",
            "Zara",
            "Netflix",
            "Hewlett-Packard",
            "Khols",
            "Credit Suisse",
            "Citigroup",
            "Allianz",
            "Ralph Lauren Corporation",
            "Volkswagen Group",
            "Johnson - Johnson",
            "Verizon Communications",
            "Corona",
            "Facebook Inc",
            "Sony",
            "SAP",
            "3M",
            "Kia Motors",
            "Mercedes-Benz",
            "Moet et Chandon",
            "Kleenex",
            "Budweiser Stag Brewing Company",
            "Panasonic Corporation",
            "Avon",
            "L-Oreal",
            "Kellogg Company",
            "Siemens AG",
            "Shell Oil Company",
            "Burberry",
            "Lowes",
            "Honda Motor Company Ltd",
            "Smirnoff",
            "Bank of America",
            "General Electric",
            "McDonalds",
            "American Express",
            "Chase",
            "Nescafe",
            "Apple Inc",
            "Tesco Corporation",
            "IKEA",
            "Wells Fargo",
            "Beko",
            "Harley-Davidson Motor Company",
            "MasterCard",
            "BMW",
            "Vodafone",
            "Prada",
            "Amazon.com",
            "Nike Inc",
            "Xerox",
            "Starbucks",
            "Audi",
            "Tiffany - Co",
            "AT&T",
            "Sprite",
            "Hyundai",
            "Samsung Group",
            "Oracle Corporation",
            "Fedex",
            "Adobe Systems",
            "KFC",
            "Gap Inc",
            "St Arnolds Brewery",
            "Toyota Motor Corporation",
            "Wal-Mart",
            "Global Gillette",
            "Louis Vuitton",
            "HSBC",
            "Yahoo",
            "Coca-Cola",
            "H-M",
            "IBM",
            "Deere - Company",
            "Specs",
            "Ferrari SpA",
            "Google",
            "Nintendo",
            "eBay",
            "PepsiCo",
            "Canon",
            "USPS",
            "Jack Daniels",
            "Adidas",
            "The Walt Disney Company",
            "Home Depot",
            "Nissan Motor Co Ltd",
            "Hermes",
            "BlackBerry",
            "Caterpillar Inc",
            "Mitsubishi",
            "Cartier SA",
            "Pappas",
            "H-E-B",
            "Ford",
            "Baylor University",
            "McLennan Community College",
            "Subway",
            "Cracker Barrel",
            "Sonic",
            "Cameron Park",
            "Dr Pepper",
            "Trujillos Comedor Y Cantina",
            "Buffalo Bayou Brewery",
            "Minute Maid Park",
            "Kroger",
            "Pizza Hut",
            "Target",
            "Williams Sonoma",
            "Crate Barrel",
            "Pottery Barn",
            "Exxon",
            "Whole Foods",
            "United Plumbing",
            "Kwick Save",
            "AAA",
            "Brewsters Bar",
            "Gexa Energy",
            "Raising Cane",
            "ACE Hardware",
            "Taco Bell",
            "Barton Hills Food Mart",
            "Tractor Supply"
        };
    }
}
