using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
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

        public static readonly ServiceProviderDescription MxServiceDescription = new ServiceProviderDescription
        {
            RequestTokenEndpoint = new MessageReceivingEndpoint("https://test.api.mxmerchant.com/v1/OAuth/1A/RequestToken", HttpDeliveryMethods.AuthorizationHeaderRequest | HttpDeliveryMethods.GetRequest),
            UserAuthorizationEndpoint = new MessageReceivingEndpoint("http://test.mxmerchant.com/oauth/authorize", HttpDeliveryMethods.AuthorizationHeaderRequest | HttpDeliveryMethods.GetRequest),
            AccessTokenEndpoint = new MessageReceivingEndpoint("https://test.api.mxmerchant.com/v1/OAuth/1A/AccessToken", HttpDeliveryMethods.AuthorizationHeaderRequest | HttpDeliveryMethods.GetRequest),
            TamperProtectionElements = new ITamperProtectionChannelBindingElement[] { new HmacSha1SigningBindingElement() },
            ProtocolVersion = ProtocolVersion.V10a
        };

        public IConsumerTokenManager TokenManager { get; private set; }


        public MxClient(string consumerKey, string consumerSecret)
            : this(new InMemoryOAuthTokenManager(consumerKey, consumerSecret))
        {
        }

        public MxClient(IConsumerTokenManager tokenManager)
            : base("MxClient", MxServiceDescription, tokenManager)
        {
            this.TokenManager = tokenManager;
        }

        /// Check if authentication succeeded after user is redirected back from the service provider.
        /// The response token returned from service provider authentication result. 
        protected override AuthenticationResult VerifyAuthenticationCore(AuthorizedTokenResponse response)
        {
            string accessToken = response.AccessToken;
            var accessTokenSecret = (response as ITokenSecretContainingMessage).TokenSecret;

            var extraData = response.ExtraData;

            // todo: this is good place to fetch user profile (insert into extra data) to pre-populate user registration fields.
            // var profile = Helper.GetUserProfile(this.TokenManager, null, accessToken);

            return Helper.VerifyAuthenticationCore(extraData, accessToken, accessTokenSecret, this.ProviderName);
        }



        public static class Helper
        {
            public static object GetUserProfile(IConsumerTokenManager tokenManager, MessageReceivingEndpoint profileEndpoint, string accessToken)
            {
                var w = new WebConsumer(MxServiceDescription, tokenManager);
                var request = w.PrepareAuthorizedRequest(profileEndpoint, accessToken);

                try
                {
                    using (WebResponse profileResponse = request.GetResponse())
                    {
                        using (Stream responseStream = profileResponse.GetResponseStream())
                        {

                        }
                    }
                }
                catch (Exception exception)
                {
                    return new AuthenticationResult(exception);
                }
            }

            public static AuthenticationResult VerifyAuthenticationCore(IDictionary<string, string> extraData, string accessToken, string accessTokenSecret, string provider)
            {
                string userId = null;
                string userName = null;

                extraData.TryGetValue("userid", out userId);
                extraData.TryGetValue("username", out userName);

                var randomString = Guid.NewGuid().ToString().Substring(0, 5);
                userId = userId ?? "simulatedUserId" + randomString;
                userName = userName ?? "simulatedUserName" + randomString;

                const bool isSuccessful = true;
                return new AuthenticationResult(isSuccessful, provider, userId, userName, extraData);

            }

        }

    }



}
