using System;
using DotNetOpenAuth.AspNet;
using DotNetOpenAuth.AspNet.Clients;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.ChannelElements;
using DotNetOpenAuth.OAuth.Messages;

namespace SimpleMembership.Auth.OAuth1a
{

    public class MxClient : OAuthClient
    {

        public static readonly ServiceProviderDescription ServiceDescription = new ServiceProviderDescription
        {
            RequestTokenEndpoint = new MessageReceivingEndpoint("https://test.api.mxmerchant.com/v1/OAuth/1A/RequestToken", HttpDeliveryMethods.AuthorizationHeaderRequest | HttpDeliveryMethods.GetRequest),
            UserAuthorizationEndpoint = new MessageReceivingEndpoint("http://test.mxmerchant.com/oauth/authorize", HttpDeliveryMethods.AuthorizationHeaderRequest | HttpDeliveryMethods.GetRequest),
            AccessTokenEndpoint = new MessageReceivingEndpoint("https://test.api.mxmerchant.com/v1/OAuth/1A/AccessToken", HttpDeliveryMethods.AuthorizationHeaderRequest | HttpDeliveryMethods.GetRequest),
            TamperProtectionElements = new ITamperProtectionChannelBindingElement[] { new HmacSha1SigningBindingElement() },
            ProtocolVersion = ProtocolVersion.V10a
        };

        private string ConsumerKey { get; set; }
        private string ConsumerSecret { get; set; }
        public IConsumerTokenManager TokenManager { get; set; }

        public MxClient(string consumerKey, string consumerSecret)
            : this(consumerKey, consumerSecret, new InMemoryOAuthTokenManager(consumerKey, consumerSecret)) { }

        public MxClient(string consumerKey, string consumerSecret, IOAuthTokenManager tokenManager)
            : this(consumerKey, consumerSecret, new SimpleConsumerTokenManager(consumerKey, consumerSecret, tokenManager)) { }

         public MxClient(string consumerKey, string consumerSecret, IConsumerTokenManager tokenManager)
             :base("MxClient", ServiceDescription, tokenManager)
         {
             ConsumerKey = consumerKey;
             ConsumerSecret = consumerSecret;
             TokenManager = tokenManager; 
         }

        /// Check if authentication succeeded after user is redirected back from the service provider.
        /// The response token returned from service provider authentication result. 
        protected override AuthenticationResult VerifyAuthenticationCore(AuthorizedTokenResponse response)
        {
            string accessToken = response.AccessToken;
            var accessTokenSecret = (response as ITokenSecretContainingMessage).TokenSecret;
 
            var userId = "";
            var userName = "";

            response.ExtraData.TryGetValue("userid", out userId);
            response.ExtraData.TryGetValue("username",out userName);
              
            var extraData = response.ExtraData;

            var randomString = Guid.NewGuid().ToString().Substring(0, 5);
            userId = userId ?? "simulatedUserId" + randomString;
            userName = userName ?? "simulatedUserName" + randomString;

            
            return new AuthenticationResult(
                isSuccessful: true, provider: this.ProviderName, providerUserId: userId, userName: userName, extraData: extraData);

            //WebConsumer w = new WebConsumer(MxServiceDescription, imoatm);

            //HttpWebRequest request = w.PrepareAuthorizedRequest(profileEndpoint, accessToken);

            //using (WebResponse profileResponse = request.GetResponse())
            //{
            //    using (Stream responseStream = profileResponse.GetResponseStream())
            //    {            
            //        
            //    }
            //}
            //catch (Exception exception)
            //{
            //    return new AuthenticationResult(exception);
            //}
        }
    }
}
