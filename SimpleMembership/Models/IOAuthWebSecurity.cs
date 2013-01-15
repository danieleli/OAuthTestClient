using System.Collections.Generic;
using System.Security.Principal;
using System.Web;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;

namespace SimpleMembership.Models
{
    public interface IOAuthWebSecurity
    {
        string GetUserName(string providerName, string providerUserId);
        bool HasLocalAccount(int userId);
        ICollection<OAuthAccount> GetAccountsFromUserName(string userName);
        bool DeleteAccount(string providerName, string providerUserId);
        AuthenticationResult VerifyAuthentication(string returnUrl);
        bool Login(string providerName, string providerUserId, bool createPersistentCookie);
        void CreateOrUpdateAccount(string providerName, string providerUserId, string userName);
        string SerializeProviderUserId(string providerName, string providerUserId);
        AuthenticationClientData GetOAuthClientData(string providerName);
        bool TryDeserializeProviderUserId(string data, out string providerName, out string providerUserId);
        ICollection<AuthenticationClientData> RegisteredClientData { get; }
        void RequestAuthentication(string provider, string returnUrl);
    }


    public interface IWebSecurity
    {
        bool Login(string userName, string password, bool persistCookie = false);
        void Logout();
        string CreateUserAndAccount(string userName, string password, object propertyValues = null, bool requireConfirmationToken = false);
        int GetUserId(string userName);
        bool ChangePassword(string userName, string currentPassword, string newPassword);
        string CreateAccount(string userName, string password, bool requireConfirmationToken = false);

        IPrincipal CurrentUser { get; }
    }



    public class OAuthWebSecurityWrapper : IOAuthWebSecurity
    {
        public string GetUserName(string providerName, string providerUserId)
        {
            return OAuthWebSecurity.GetUserName(providerName, providerUserId);
        }

        public bool HasLocalAccount(int userId)
        {
            return OAuthWebSecurity.HasLocalAccount(userId);
        }

        public ICollection<OAuthAccount> GetAccountsFromUserName(string userName)
        {
            return OAuthWebSecurity.GetAccountsFromUserName(userName);
        }

        public bool DeleteAccount(string providerName, string providerUserId)
        {
            return OAuthWebSecurity.DeleteAccount(providerName, providerUserId);
        }

        public AuthenticationResult VerifyAuthentication(string returnUrl)
        {
            return OAuthWebSecurity.VerifyAuthentication(returnUrl);
        }

        public bool Login(string providerName, string providerUserId, bool createPersistentCookie)
        {
            return OAuthWebSecurity.Login(providerName, providerUserId, createPersistentCookie);
        }

        public void CreateOrUpdateAccount(string providerName, string providerUserId, string userName)
        {
            OAuthWebSecurity.CreateOrUpdateAccount(providerName, providerUserId, userName);
        }

        public string SerializeProviderUserId(string providerName, string providerUserId)
        {
            return OAuthWebSecurity.SerializeProviderUserId(providerName, providerUserId);
        }

        public AuthenticationClientData GetOAuthClientData(string providerName)
        {
            return OAuthWebSecurity.GetOAuthClientData(providerName);
        }

        public bool TryDeserializeProviderUserId(string data, out string providerName, out string providerUserId)
        {
            return OAuthWebSecurity.TryDeserializeProviderUserId(data, out providerName, out providerUserId);
        }

        public ICollection<AuthenticationClientData> RegisteredClientData { get { return OAuthWebSecurity.RegisteredClientData; } }

        public void RequestAuthentication(string provider, string returnUrl)
        {
            OAuthWebSecurity.RequestAuthentication(provider, returnUrl);
        }
    }

    public class WebSecurityWrapper : IWebSecurity
    {
        public bool Login(string userName, string password, bool persistCookie = false)
        {
            return WebSecurity.Login(userName, password, persistCookie);
        }

        public void Logout()
        {
            WebSecurity.Logout();
        }

        public string CreateUserAndAccount(string userName, string password, object propertyValues = null, bool requireConfirmationToken = false)
        {
            return WebSecurity.CreateUserAndAccount(userName, password, propertyValues);
        }

        public int GetUserId(string userName)
        {
            return WebSecurity.GetUserId(userName);
        }

        public bool ChangePassword(string userName, string currentPassword, string newPassword)
        {
            return WebSecurity.ChangePassword(userName, currentPassword, newPassword);
        }

        public string CreateAccount(string userName, string password, bool requireConfirmationToken = false)
        {
            return WebSecurity.CreateAccount(userName, password, requireConfirmationToken);
        }

        public IPrincipal CurrentUser
        {
            get { return HttpContext.Current.User; }
        }
    }
}
