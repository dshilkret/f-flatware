using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.IdentityModel.Clients.ActiveDirectory;
//using Microsoft.Azure.KeyVault;
using System.Net.Cache;
using System.Net;
using System.IO;
//using Microsoft.Azure.Services.AppAuthentication;
//using Microsoft.Azure.KeyVault.Models;

namespace Raydreams.Common.Services.Security
{
    /// <summary>Manager for acessing Azure KeyVault secrets</summary>
    public class KeyVaultRepository
    {
        //		#region [ Fields ]

        //		/// <summary>the location of Incommons keyvault store</summary>
        //		public static readonly string DefaultVaultAddress = "https://";

        //		#endregion [ Fields]

        //		#region [Constructor]

        //		/// <summary>Constructor using the Client ID and PW which come from the env config settings</summary>
        //		public KeyVaultRepository( string clientID, string clientPW )
        //		{
        //			this.ClientID = clientID;
        //			this.ClientPW = clientPW;

        //			this.VaultAddress = DefaultVaultAddress;
        //		}

        //		#endregion [Constructor]

        //		#region [Properties]

        //		/// <summary>The key vault URL</summary>
        //		public string VaultAddress { get; set; }

        //		/// <summary>The Client ID GUID in Azure AD</summary>
        //		public string ClientID { get; set; }

        //		/// <summary>The Client PW in Azure AD</summary>
        //		public string ClientPW { get; set; }

        //		#endregion [Properties]

        //		#region [Methods]

        //		/// <summary>Gets the symmetrical AES key to encrypt and decrypt the app session tokens</summary>
        //        /// <param name="secretName">Pass in the name of the secret in the Azure KV that you want its secret key</param>
        //		public string GetSecret(string secretName)
        //		{
        //			if ( String.IsNullOrWhiteSpace( secretName ) )
        //				return null;

        //			string secret = null;

        //			try
        //			{
        //				// new provider
        //				AzureServiceTokenProvider provider = new AzureServiceTokenProvider();

        //				// create a KV client
        //				KeyVaultClient client = GetClient();

        //				// get the secret key
        //				SecretBundle results = client.GetSecretAsync( this.VaultAddress, secretName.Trim() ).GetAwaiter().GetResult();
        //				secret = results.Value;
        //			}
        //			catch ( KeyVaultErrorException exp )
        //			{
        //				throw exp;
        //			}

        //			return secret;
        //		}

        //		/// <summary>Create a new KeyVault client with GetToken callback</summary>
        //		public KeyVaultClient GetClient()
        //		{
        //			return new KeyVaultClient( new KeyVaultClient.AuthenticationCallback( GetToken ) );
        //		}

        //		/// <summary>Callback to use to autheticate based on the client ID and PW</summary>
        //		/// <param name="authority"></param>
        //		/// <param name="resource"></param>
        //		/// <returns></returns>
        //		protected async Task<string> GetToken( string authority, string resource, string scope )
        //		{
        //			AuthenticationContext authContext = new AuthenticationContext( authority );

        //			ClientCredential clientCred = new ClientCredential( this.ClientID, this.ClientPW );

        //			AuthenticationResult result = await authContext.AcquireTokenAsync( resource, clientCred );

        //			if ( result == null )
        //				throw new InvalidOperationException( "Failed to obtain a token." );

        //			return result.AccessToken;
        //		}

        //		#endregion [Methods]

    }
}
