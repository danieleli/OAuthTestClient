using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using SimpleMembership.Auth.OAuth1a;
using SimpleMembership.Filters;
using SimpleMembership.Models;
using SimpleMembership.Models.Login;

namespace SimpleMembership.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class OAuthController : Controller
    {
        private IWebSecurity WebSecurity { get; set; }
        private IOAuthWebSecurity OAuthWebSecurity { get; set; }

        public OAuthController()
            : this(new WebSecurityWrapper(), new OAuthWebSecurityWrapper())
        {
        }

        public OAuthController(IWebSecurity webSecurity, IOAuthWebSecurity oAuthWebSecurity)
        {
            WebSecurity = webSecurity;
            OAuthWebSecurity = oAuthWebSecurity;
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            var rtnUrl = Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl });
            return new ExternalLoginResult(provider, rtnUrl, this.OAuthWebSecurity);
        }



        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            var knownProviderUserId = OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false);
            if (knownProviderUserId)
            {
                return RedirectToLocal(returnUrl);
            }

            if (WebSecurity.CurrentUser.Identity.IsAuthenticated)
            {
                // New providerUserId
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, WebSecurity.CurrentUser.Identity.Name);
                return RedirectToLocal(returnUrl);
            }
            else           
            {
                // New user
                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;

                var accessToken = "";
                result.ExtraData.TryGetValue("accesstoken", out accessToken);

                return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, AccessToken = accessToken, ExternalLoginData = loginData });
            }
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider = null;
            string providerUserId = null;

            if (WebSecurity.CurrentUser.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Insert a new user into the database
                using (UsersContext db = new UsersContext())
                {
                    UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
                    // Check if user already exists
                    if (user == null)
                    {
                        // Insert name into the profile table
                        var userProfile = new UserProfile() { UserName = model.UserName };
                        db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
                        db.SaveChanges();

                        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
                        var isLoggedIn = OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                        if (!isLoggedIn)
                        {
                            throw new Exception("Login failed");
                        }

                        var secret = "";
                        if (provider == "MxClient")
                        {
                            secret = GetMxSecret(model);
                        }

                        var newToken = new OAuthToken()
                            {
                                Provider = provider,
                                ProviderUserId = providerUserId,
                                Token = model.AccessToken,
                                Secret = secret,
                                UserId = userProfile.UserId
                            };


                        db.OAuthTokens.Add(newToken);
                        var rows = db.SaveChanges();
                        Debug.WriteLine("Rows updated: " + rows);
                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
                    }
                }
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }


        private string GetMxSecret(RegisterExternalLoginModel model)
        {
            var clientData = OAuthWebSecurity.GetOAuthClientData("MxClient");
            var extraData = clientData.ExtraData;


            if (extraData != null)
            {
                foreach (var pair in extraData)
                {
                    Debug.WriteLine(pair.Key + " - " + pair.Value);
                }
            }

            var mxClient = (MxClient)clientData.AuthenticationClient;
            var secret = mxClient.TokenManager.GetTokenSecret(model.AccessToken);
            return secret;
        }

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(WebSecurity.CurrentUser.Identity.Name);
            List<ExternalLogin> externalLogins = new List<ExternalLogin>();
            foreach (OAuthAccount account in accounts)
            {
                AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

                externalLogins.Add(new ExternalLogin
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                });
            }

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(WebSecurity.CurrentUser.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        public class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl, IOAuthWebSecurity oAuthWebSecurity)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
                OAuthWebSecurity = oAuthWebSecurity;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }
            public IOAuthWebSecurity OAuthWebSecurity { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        #endregion
    }
}