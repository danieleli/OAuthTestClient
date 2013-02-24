using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using DotNetOpenAuth.AspNet;
using DotNetOpenAuth.AspNet.Clients;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.ChannelElements;
using DotNetOpenAuth.OAuth.Messages;
using log4net;

namespace SimpleMembership.Auth.OAuth1a
{
    public class MxClient2 : IAuthenticationClient
    {
        public void RequestAuthentication(HttpContextBase context, Uri returnUrl)
        {
            throw new NotImplementedException();
        }

        public AuthenticationResult VerifyAuthentication(HttpContextBase context)
        {
            throw new NotImplementedException();
        }

        public string ProviderName { get { return "MxClient"; } }
    }

    public class MxClient : OAuthClient
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(MxClient));

        public static readonly ServiceProviderDescription MxServiceDescription = new ServiceProviderDescription
        {
            RequestTokenEndpoint = new MessageReceivingEndpoint("https://test.api.mxmerchant.com/v1/OAuth/1A/RequestToken", HttpDeliveryMethods.AuthorizationHeaderRequest | HttpDeliveryMethods.GetRequest),
            UserAuthorizationEndpoint = new MessageReceivingEndpoint("http://test.mxmerchant.com/oauth/authorize", HttpDeliveryMethods.AuthorizationHeaderRequest | HttpDeliveryMethods.GetRequest),
            AccessTokenEndpoint = new MessageReceivingEndpoint("https://test.api.mxmerchant.com/v1/OAuth/1A/AccessToken", HttpDeliveryMethods.AuthorizationHeaderRequest | HttpDeliveryMethods.GetRequest),
            TamperProtectionElements = new ITamperProtectionChannelBindingElement[] { new HmacSha1SigningBindingElement() },
            ProtocolVersion = ProtocolVersion.V10a
        };

        public const string NAME = "MxClient";

        public IConsumerTokenManager TokenManager { get; private set; }

        #region -- Constructors --
        
        public MxClient(string consumerKey, string consumerSecret)
            : this(new InMemoryOAuthTokenManager(consumerKey, consumerSecret))
        {
        }

        public MxClient(IConsumerTokenManager tokenManager)
            : base(NAME, MxServiceDescription, tokenManager)
        {
            this.TokenManager = tokenManager;
        }

        #endregion  -- Constructors --



        public override AuthenticationResult VerifyAuthentication (HttpContextBase context)
        {
            AuthorizedTokenResponse response = this.WebWorker.ProcessUserAuthorization();
            if (response == null)
                return AuthenticationResult.Failed;
            AuthenticationResult authenticationResult = this.VerifyAuthenticationCore(response);
            if (authenticationResult.IsSuccessful && authenticationResult.ExtraData != null)
            {
                IDictionary<string, string> extraData = authenticationResult.ExtraData.IsReadOnly ? (IDictionary<string, string>)new Dictionary<string, string>(authenticationResult.ExtraData) : authenticationResult.ExtraData;
                extraData["accesstoken"] = response.AccessToken;
                authenticationResult = new AuthenticationResult(authenticationResult.IsSuccessful, authenticationResult.Provider, authenticationResult.ProviderUserId, authenticationResult.UserName, extraData);
            }
            return authenticationResult;
        }

        protected override AuthenticationResult VerifyAuthenticationCore (AuthorizedTokenResponse response)
        {
            LOG.Debug("vERITY");
            return Helper.CreateAuthenticationResult(response, this.ProviderName);
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

                return null;
            }

            public static AuthenticationResult CreateAuthenticationResult(AuthorizedTokenResponse response, string providerName)
            {
                var accessToken = response.AccessToken;
                var accessTokenSecret = (response as ITokenSecretContainingMessage).TokenSecret;

                var extraData = response.ExtraData;

                string userId = null;
                string userName = null;

                extraData.TryGetValue("userid", out userId);
                extraData.TryGetValue("username", out userName);


                var randomString = Guid.NewGuid().ToString().Substring(0, 5);
                userId = userId ?? "userIdNotFound" + randomString;
                userName = userName ?? "userNameNotFound" + randomString;


                // todo: fetch user profile (insert into extra data) to pre-populate user registration fields.
                // var profile = Helper.GetUserProfile(this.TokenManager, null, accessToken);

                const bool isSuccessful = true;
                return new AuthenticationResult(isSuccessful, providerName, userId, userName, extraData);   
            }
        }

    }

}
