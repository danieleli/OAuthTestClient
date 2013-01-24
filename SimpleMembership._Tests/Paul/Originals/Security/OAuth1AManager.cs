using System.Net.Http;
using System.Web;
using System.Web.Http;
using PPS.API.Common.Extensions;
using PPS.API.Common.Helpers;
using System;
using System.Linq;

namespace PPS.API.Common.Security
{
    internal class OAuth1AManager : ISecurityManager
    {
        public IAuthRepository AuthRepository
        {
            get
            {
                return GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IAuthRepository)) as IAuthRepository;
            }
        }

        public SecurityContext GetSecurityContext(HttpRequestMessage request)
        {
            var queryString = HttpUtility.ParseQueryString(request.RequestUri.Query);
            var authHdr = request.Headers.Authorization.ParseAuthParam();

            var consumer = authHdr[Constants.OAuth.V1A.ConsumerKeyKey] ?? queryString[Constants.OAuth.V1A.ConsumerKeyKey];
            var token = authHdr[Constants.OAuth.V1A.TokenKey] ?? queryString[Constants.OAuth.V1A.TokenKey];

            if ((token == null && consumer == null) || (token == null && !request.IsAuth()))
                return null;

			var securityContext = new SecurityContext() { Protocol = AuthorizationProtocol.OAuth1A };

            if (token != null)
            {
                var accessToken = AuthRepository.GetAccessToken(token);

                if (accessToken != null)
                {
                    securityContext.TokenInfo = accessToken;
                    securityContext.ConsumerInfo = AuthRepository.GetConsumerByToken(accessToken.Token);
					securityContext.Grants = AuthRepository.GetAccessGrantsByToken(accessToken.Token);
                }
                else
                { 
                    var requestToken = AuthRepository.GetRequestToken(token);

                    if (requestToken != null)
                    {
                        securityContext.TokenInfo = requestToken;
						securityContext.ConsumerInfo = AuthRepository.GetConsumerByToken(requestToken.Token);
                    }
                }

                if (securityContext.TokenInfo == null || (securityContext.TokenInfo is PPS.API.Common.Security.Model.RequestToken && !request.IsAuth()))
                {
                    Logger.Debug(this, "Token Information is null for token: " + (token == null ? "<<NULL>>" : token));
                    throw new HttpResponseException(new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized));
                }
                else
                    Logger.Debug(this, "Token user id: " + securityContext.TokenInfo.ConsumerId);
            }
            else if (consumer != null)
            {
                securityContext.ConsumerInfo = AuthRepository.GetConsumer(consumer);
				
				if (securityContext.ConsumerInfo != null)
				{
					Logger.Debug(this, "Consumer Type: " + securityContext.ConsumerInfo.ConsumerType.ToString());
					Logger.Debug(this, "Consumer Id: " + securityContext.ConsumerInfo.Id.ToString());
				}
            }

            if (securityContext == null || securityContext.ConsumerInfo == null)
                return null;

            securityContext.Parameters.Add(Constants.OAuth.V1A.NonceKey, authHdr[Constants.OAuth.V1A.NonceKey] ?? queryString[Constants.OAuth.V1A.NonceKey]);
            securityContext.Parameters.Add(Constants.OAuth.V1A.TimestampKey, authHdr[Constants.OAuth.V1A.TimestampKey] ?? queryString[Constants.OAuth.V1A.TimestampKey]);
            securityContext.Parameters.Add(Constants.OAuth.V1A.TokenKey, authHdr[Constants.OAuth.V1A.TokenKey] ?? queryString[Constants.OAuth.V1A.TokenKey]);
            securityContext.Parameters.Add(Constants.OAuth.V1A.SignatureKey, authHdr[Constants.OAuth.V1A.SignatureKey] ?? queryString[Constants.OAuth.V1A.SignatureKey]);
            securityContext.Parameters.Add(Constants.OAuth.V1A.ConsumerKeyKey, authHdr[Constants.OAuth.V1A.ConsumerKeyKey] ?? queryString[Constants.OAuth.V1A.ConsumerKeyKey]);
            securityContext.Parameters.Add(Constants.OAuth.V1A.CallbackKey, authHdr[Constants.OAuth.V1A.CallbackKey] ?? queryString[Constants.OAuth.V1A.CallbackKey]);
            securityContext.Parameters.Add(Constants.OAuth.V1A.VersionKey, authHdr[Constants.OAuth.V1A.VersionKey] ?? queryString[Constants.OAuth.V1A.VersionKey]);
            securityContext.Parameters.Add(Constants.OAuth.V1A.SignatureMethodKey, authHdr[Constants.OAuth.V1A.SignatureMethodKey] ?? queryString[Constants.OAuth.V1A.SignatureMethodKey]);
            securityContext.Parameters.Add(Constants.OAuth.V1A.TokenSecretKey, authHdr[Constants.OAuth.V1A.TokenSecretKey] ?? queryString[Constants.OAuth.V1A.TokenSecretKey]);
            securityContext.Parameters.Add(Constants.OAuth.V1A.VerifierKey, authHdr[Constants.OAuth.V1A.VerifierKey] ?? queryString[Constants.OAuth.V1A.VerifierKey]);

            securityContext.Timestamp = securityContext.Parameters[Constants.OAuth.V1A.TimestampKey];
            securityContext.Signature = securityContext.Parameters[Constants.OAuth.V1A.SignatureKey];

            return securityContext;
        }

        public bool IsValidSignature(SecurityContext securityContext, HttpRequestMessage request)
        {
            string originalUri = request.RequestUri.ToString();

            if (ComputeSignatureAndValidate(securityContext, request))
            {
                return true;
            }
            else
            {
                // Try the alternate URI
                if (!String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[Constants.Settings.ApiUrlAlternate]))
                {
                    var apiUrl = System.Configuration.ConfigurationManager.AppSettings[Constants.Settings.ApiUrlAlternate];
                    var reroutedUrl = System.Configuration.ConfigurationManager.AppSettings[Constants.Settings.ReroutedUrl];

                    Logger.Debug(typeof(PPS.API.Common.Extensions.UriExtensions), apiUrl);
                    Logger.Debug(typeof(PPS.API.Common.Extensions.UriExtensions), reroutedUrl);
                    Logger.Debug(typeof(PPS.API.Common.Extensions.UriExtensions), "Comparing URLs - {0} - {1}", originalUri, reroutedUrl);

                    request.RequestUri = new Uri(originalUri.Replace(reroutedUrl, apiUrl));

                    if (ComputeSignatureAndValidate(securityContext, request))
                    {
                        return true;
                    }
                }
			}

           return false;
		}

        private bool ComputeSignatureAndValidate(SecurityContext securityContext, HttpRequestMessage request)
        {
            // we only support HMAC-SHA1 signatures!!!
            if (securityContext.Parameters[Constants.OAuth.V1A.SignatureMethodKey] != null &&
                securityContext.Parameters[Constants.OAuth.V1A.SignatureMethodKey] != Constants.OAuth.V1A.HMACSHA1SignatureType)
                return false;

            // let's get all the significant inputs loaded
            string requestToken = securityContext.Parameters[Constants.OAuth.V1A.TokenKey];
            string requestConsumerKey = securityContext.Parameters[Constants.OAuth.V1A.ConsumerKeyKey];
            string requestTimestamp = securityContext.Parameters[Constants.OAuth.V1A.TimestampKey];
            string requestNonce = securityContext.Parameters[Constants.OAuth.V1A.NonceKey];
            string requestSignature = securityContext.Parameters[Constants.OAuth.V1A.SignatureKey];
            string requestCallback = securityContext.Parameters[Constants.OAuth.V1A.CallbackKey];
            string requestVerifier = securityContext.Parameters[Constants.OAuth.V1A.VerifierKey];

            string consumerSecret_base64 = securityContext.ConsumerInfo.Secret;
            string consumerSecret_hex = BitConverter.ToString(Convert.FromBase64String(securityContext.ConsumerInfo.Secret)).Replace("-", string.Empty).ToLower();

            string tokenSecret = null;
			
			if (securityContext.TokenInfo != null && securityContext.TokenInfo is PPS.API.Common.Security.Model.AccessToken)
				tokenSecret = (securityContext.TokenInfo as PPS.API.Common.Security.Model.AccessToken).Secret;
			else if (securityContext.TokenInfo != null && securityContext.TokenInfo is PPS.API.Common.Security.Model.RequestToken)
				tokenSecret = (securityContext.TokenInfo as PPS.API.Common.Security.Model.RequestToken).Secret;

            Logger.Debug(this, "get the signature using HEX SECRET");
            if (ComputeSignatureAndCompare(requestSignature, request, requestConsumerKey, consumerSecret_hex, requestToken, tokenSecret, requestTimestamp, requestNonce, requestCallback, requestVerifier))
                return true;

            Logger.Debug(this, "get the signature using BASE64 SECRET");
            if (ComputeSignatureAndCompare(requestSignature, request, requestConsumerKey, consumerSecret_base64, requestToken, tokenSecret, requestTimestamp, requestNonce, requestCallback, requestVerifier))
                return true;

            if (request.RequestUri.ChangeToExternalIfRerouted().Scheme.Equals(System.Uri.UriSchemeHttp))
                request.RequestUri = new Uri(request.RequestUri.ChangeToExternalIfRerouted().ToString().Replace(System.Uri.UriSchemeHttp, System.Uri.UriSchemeHttps));
            else if (request.RequestUri.ChangeToExternalIfRerouted().Scheme.Equals(System.Uri.UriSchemeHttps))
                request.RequestUri = new Uri(request.RequestUri.ChangeToExternalIfRerouted().ToString().Replace(System.Uri.UriSchemeHttps, System.Uri.UriSchemeHttp));
            else
                return false;

            Logger.Debug(this, "get the signature using HEX SECRET AND REROUTED URI");
            if (ComputeSignatureAndCompare(requestSignature, request, requestConsumerKey, consumerSecret_hex, requestToken, tokenSecret, requestTimestamp, requestNonce, requestCallback, requestVerifier))
                return true;

            Logger.Debug(this, "get the signature using BASE64 SECRET AND REROUTED URI");
            if (ComputeSignatureAndCompare(requestSignature, request, requestConsumerKey, consumerSecret_base64, requestToken, tokenSecret, requestTimestamp, requestNonce, requestCallback, requestVerifier))
                return true;

            return false;
        }

        private bool ComputeSignatureAndCompare(string requestSignature, HttpRequestMessage request, string requestConsumerKey, string consumerSecret, string requestToken, string tokenSecret, string requestTimestamp, string requestNonce, string requestCallback, string requestVerifier)
            {
                // get the signature using hex secret
            var computedSignature = Signature.GetOAuth1ASignature(request, requestConsumerKey, consumerSecret, requestToken, tokenSecret, requestTimestamp, requestNonce, requestCallback, requestVerifier);

            Logger.Debug(this, "OAUTH.V1A Computed a signature using the secret - " + consumerSecret);
                Logger.Debug(this, "OAUTH.V1A Request signature is - " + requestSignature);
                Logger.Debug(this, "OAUTH.V1A Derived signature is - " + computedSignature);
                Logger.Debug(this, "OAUTH.V1A     and url encoded uppercase  - " + UrlEncodingHelper.UpperCaseUrlEncode(computedSignature));
                Logger.Debug(this, "OAUTH.V1A     and url encoded lowercase  - " + UrlEncodingHelper.LowerCaseUrlEncode(computedSignature));

                // some OAuth libraries URL encode these values and some do not so lets check both ways just in case
                if (requestSignature == computedSignature || requestSignature == UrlEncodingHelper.UpperCaseUrlEncode(computedSignature) ||
                    requestSignature == UrlEncodingHelper.LowerCaseUrlEncode(computedSignature))
                    return true;
                else
                    return false;
        }

        public bool IsRepeatedRequest(SecurityContext securityContext, HttpRequestMessage request)
        {
            var result = AuthRepository.LogRequest(securityContext.ConsumerInfo,
                securityContext.Parameters[Constants.OAuth.V1A.NonceKey],
                securityContext.Parameters[Constants.OAuth.V1A.TimestampKey],
                securityContext.Parameters[Constants.OAuth.V1A.TokenKey]);

            return result;
        }

		public bool IsExpiredToken(SecurityContext securityContext, HttpRequestMessage request)
		{
			if (securityContext.TokenInfo == null)
			{
				Logger.Debug(this, "Token info: <<NULL>>");
				return false;
			}
			else if (securityContext.TokenInfo.UtcDateExpires.HasValue && securityContext.TokenInfo.UtcDateExpires < DateTime.UtcNow)
			{
				Logger.Debug(this, "Token expired: " + securityContext.TokenInfo.UtcDateExpires.Value.ToString());
				return true;
			}
			else if (securityContext.TokenInfo.UseLimit > 0 && securityContext.TokenInfo.UseCount >= securityContext.TokenInfo.UseLimit)
			{
				Logger.Debug(this, "Token use limit exceeded. Limit: " + securityContext.TokenInfo.UseLimit.ToString() + ", Use: " + securityContext.TokenInfo.UseCount.ToString());
				return true;
			}

			var tokenLifetime = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings[Constants.Settings.TokenLifetime]);

			if (tokenLifetime > 0 && securityContext.TokenInfo.UtcDateCreated.AddSeconds(tokenLifetime) < DateTime.UtcNow)
			{
				Logger.Debug(this, "Token expired: " + securityContext.TokenInfo.UtcDateCreated.AddSeconds(tokenLifetime).ToString());
				return true;
			}

			return false; 
		}
    }
}
