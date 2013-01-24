using System.Net.Http;
using System.Web.Http;
using PPS.API.Common.Extensions;
using PPS.API.Common.Helpers;
using System;
using System.Web;

namespace PPS.API.Common.Security
{
	internal class OAuth2Manager : ISecurityManager
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
			var formData = request.Content.ReadAsFormDataAsync().Result;

			var clientId = queryString[Constants.OAuth.V2.ClientId] ?? formData[Constants.OAuth.V2.ClientId];
			var token = request.Headers.Authorization.Token();

			if ((String.IsNullOrWhiteSpace(clientId) && String.IsNullOrWhiteSpace(token)) || (String.IsNullOrWhiteSpace(token) && !request.IsAuth()))
				return null;

			// Bearer token will always be in the auth header for OAuth 2.0 except when during the auth
			var securityContext = new SecurityContext() { Protocol = AuthorizationProtocol.OAuth2 };

			if (!String.IsNullOrWhiteSpace(token))
			{
				var accessToken = AuthRepository.GetAccessToken(token);

				if (accessToken == null)
				{
					Logger.Debug(this, "Token Information is null for token: " + token);
					throw new HttpResponseException(new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized));
				}

				securityContext.TokenInfo = accessToken;
				securityContext.ConsumerInfo = AuthRepository.GetConsumerByToken(token);
				securityContext.Grants = AuthRepository.GetAccessGrantsByToken(token);

				if (securityContext.ConsumerInfo == null)
				{
					Logger.Debug(this, "Consumer Information is null for token: " + token);
					return null;
				}
				
				Logger.Debug(this, "Token consumer id: " + securityContext.TokenInfo.ConsumerId);
			}
			else
			{
				securityContext.ConsumerInfo = AuthRepository.GetConsumer(clientId);
			}

			if (securityContext.ConsumerInfo == null)
				return null;

			securityContext.Parameters.Add(Constants.OAuth.V2.ResponseType, queryString[Constants.OAuth.V2.ResponseType] ?? formData[Constants.OAuth.V2.ResponseType]);
			securityContext.Parameters.Add(Constants.OAuth.V2.ClientId, queryString[Constants.OAuth.V2.ClientId] ?? formData[Constants.OAuth.V2.ClientId]);
			securityContext.Parameters.Add(Constants.OAuth.V2.ClientSecret, queryString[Constants.OAuth.V2.ClientSecret] ?? formData[Constants.OAuth.V2.ClientSecret]);
			securityContext.Parameters.Add(Constants.OAuth.V2.RedirectUri, queryString[Constants.OAuth.V2.RedirectUri] ?? formData[Constants.OAuth.V2.RedirectUri]);
			securityContext.Parameters.Add(Constants.OAuth.V2.Scope, queryString[Constants.OAuth.V2.Scope] ?? formData[Constants.OAuth.V2.Scope]);
			securityContext.Parameters.Add(Constants.OAuth.V2.State, queryString[Constants.OAuth.V2.State] ?? formData[Constants.OAuth.V2.State]);
			securityContext.Parameters.Add(Constants.OAuth.V2.AccessType, queryString[Constants.OAuth.V2.AccessType] ?? formData[Constants.OAuth.V2.AccessType]);
			securityContext.Parameters.Add(Constants.OAuth.V2.ApprovalPrompt, queryString[Constants.OAuth.V2.ApprovalPrompt] ?? formData[Constants.OAuth.V2.ApprovalPrompt]);
			securityContext.Parameters.Add(Constants.OAuth.V2.Code, queryString[Constants.OAuth.V2.Code] ?? formData[Constants.OAuth.V2.Code]);
			securityContext.Parameters.Add(Constants.OAuth.V2.GrantType, queryString[Constants.OAuth.V2.GrantType] ?? formData[Constants.OAuth.V2.GrantType]);
			securityContext.Parameters.Add(Constants.OAuth.V2.RefreshToken, queryString[Constants.OAuth.V2.RefreshToken] ?? formData[Constants.OAuth.V2.RefreshToken]);

			return securityContext;
		}

		public bool IsValidSignature(SecurityContext securityContext, HttpRequestMessage request)
		{
			// If this is an auth request then we need to validate the secret
			if (request.IsAuth() && !String.Equals(securityContext.Parameters[Constants.OAuth.V2.ResponseType], "code", StringComparison.OrdinalIgnoreCase))
				return securityContext.ConsumerInfo.Secret == securityContext.Parameters[Constants.OAuth.V2.ClientSecret];

			return true;
		}

		public bool IsRepeatedRequest(SecurityContext securityContext, HttpRequestMessage request)
		{
			return false;
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
