using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DotNetOpenAuth.AspNet.Clients;
using DotNetOpenAuth.OAuth.ChannelElements;
using DotNetOpenAuth.OAuth.Messages;
using SimpleMembership.Models;

namespace SimpleMembership.Auth
{
    public class DbTokenManager : IConsumerTokenManager
    {
        public string ConsumerKey { get; private set; }
        public string ConsumerSecret { get; private set; }

        private Dictionary<string, string> requestTokens = new Dictionary<string, string>();

        private readonly IOAuthTokenManager tokenManager;

		/// <summary>
		/// Initializes a new instance of the <see cref="SimpleConsumerTokenManager"/> class.
		/// </summary>
		/// <param name="consumerKey">The consumer key.</param>
		/// <param name="consumerSecret">The consumer secret.</param>
		/// <param name="tokenManager">The OAuth token manager.</param>
        public DbTokenManager(string consumerKey, string consumerSecret, IOAuthTokenManager tokenManager)
        {
			this.ConsumerKey = consumerKey;
			this.ConsumerSecret = consumerSecret;
			this.tokenManager = tokenManager;
		}

    
        public string GetTokenSecret(string token)
        {
            Debug.WriteLine("GetTokenSecret: " + token);
            using (var db = new UsersContext())
            {
                return db.OAuthTokens.Single(t => t.Token == token).Secret;
            }
        }

        public void StoreNewRequestToken(UnauthorizedTokenRequest request, ITokenSecretContainingMessage response)
        {
            Debug.WriteLine("StoreNewRequestToken: " + response.Token);
            this.requestTokens[response.Token] = response.TokenSecret;

        }

        public void ExpireRequestTokenAndStoreNewAccessToken(string consumerKey, string requestToken, string accessToken,
                                                             string accessTokenSecret)
        {
            Debug.WriteLine("ExpireRequestTokenAndStoreNewAccessToken: " + accessToken);
            this.requestTokens.Remove(requestToken);
            using (var db = new UsersContext())
            {
                db.OAuthTokens.Add(new OAuthToken { Token = accessToken, Secret = accessTokenSecret });
                db.SaveChanges();
            }
        }

        public TokenType GetTokenType(string token)
        {
            throw new NotImplementedException();
        }
    }
}
