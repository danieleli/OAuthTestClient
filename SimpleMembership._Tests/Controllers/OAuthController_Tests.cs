using DotNetOpenAuth.AspNet;
using DotNetOpenAuth.AspNet.Clients;
using NUnit.Framework;
using SimpleMembership.Auth.OAuth1a;
using SimpleMembership.Models;
using SimpleMembership.Models.Login;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Moq;
using SimpleMembership.Controllers;
using System.Security.Principal;
using System.Collections.Generic;
using Microsoft.Web.WebPages.OAuth;
using log4net;

namespace SimpleMembership._Tests.Controllers
{
    [TestFixture]
    public class OAuthController_Tests
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(OAuthController_Tests));
        public class OAuthMocks
        {
            public AccountController Controller { get; set; }
            public RouteCollection Routes { get; set; }
            public Mock<IWebSecurity> WebSecurity { get; set; }
            public Mock<IOAuthWebSecurity> OAuthWebSecurity { get; set; }
            public Mock<HttpResponseBase> Response { get; set; }
            public Mock<HttpRequestBase> Request { get; set; }
            public Mock<HttpContextBase> Context { get; set; }
            public Mock<ControllerContext> ControllerContext { get; set; }
            public Mock<IPrincipal> User { get; set; }
            public Mock<IIdentity> Identity { get; set; }

            public OAuthMocks()
            {
                WebSecurity = new Mock<IWebSecurity>(MockBehavior.Strict);
                OAuthWebSecurity = new Mock<IOAuthWebSecurity>(MockBehavior.Strict);

                Identity = new Mock<IIdentity>(MockBehavior.Strict);
                User = new Mock<IPrincipal>(MockBehavior.Strict);
                User.SetupGet(u => u.Identity).Returns(Identity.Object);
                WebSecurity.SetupGet(w => w.CurrentUser).Returns(User.Object);

                Routes = new RouteCollection();
                RouteConfig.RegisterRoutes(Routes);

                Request = new Mock<HttpRequestBase>(MockBehavior.Strict);
                Request.SetupGet(x => x.ApplicationPath).Returns("/");
                Request.SetupGet(x => x.Url).Returns(new Uri("http://localhost/a", UriKind.Absolute));
                Request.SetupGet(x => x.ServerVariables).Returns(new System.Collections.Specialized.NameValueCollection());

                Response = new Mock<HttpResponseBase>(MockBehavior.Strict);

                Context = new Mock<HttpContextBase>(MockBehavior.Strict);
                Context.SetupGet(x => x.Request).Returns(Request.Object);
                Context.SetupGet(x => x.Response).Returns(Response.Object);
            }
        }

        public OAuthController GetController(OAuthMocks mocks)
        {

            var controller = new OAuthController(mocks.WebSecurity.Object, mocks.OAuthWebSecurity.Object);
            controller.ControllerContext = new ControllerContext(mocks.Context.Object, new RouteData(), controller);
            controller.Url = new UrlHelper(new RequestContext(mocks.Context.Object, new RouteData()), mocks.Routes);

            return controller;
        }


        const string RETURN_URL = "/Home/Index";
        const string PROVIDER = "MxMerchant";
        const string PROVIDER_DISPLAY_NAME = "Mx Merchant";
        const string PROVIDER_USERID = "pUser";
        const string USERNAME = "username";

        [Test]
        public void ExternalLogin_Returns_ExternalLoginResult()
        {
            // Arrange
            var mocks = new OAuthMocks();
            var controller = GetController(mocks);


            mocks.Response.Setup(r => r.ApplyAppPathModifier(It.IsAny<string>())).Returns(RETURN_URL);

            // Act
            var result = controller.ExternalLogin(PROVIDER, RETURN_URL) as OAuthController.ExternalLoginResult;

            // Assert
            Assert.NotNull(result, "result");
            Assert.AreEqual(result.Provider, PROVIDER);
            Assert.AreEqual(result.ReturnUrl, RETURN_URL);
        }


        [Test]
        public void ExternalLogin_RedirectsTo_ExternalLoginFailure_OnFailure()
        {
            // Arrange
            var mocks = new OAuthMocks();
            var controller = GetController(mocks);

            mocks.Response.Setup(r => r.ApplyAppPathModifier(It.IsAny<string>())).Returns(RETURN_URL);
            mocks.OAuthWebSecurity.Setup(ows => ows.VerifyAuthentication(It.IsAny<string>())).Returns(new AuthenticationResult(false));

            // Act
            var result = controller.ExternalLoginCallback(RETURN_URL) as RedirectToRouteResult;

            // Assert
            Assert.NotNull(result, "result");
            Assert.NotNull(result.RouteValues["action"], "ExternalLoginFailure");
        }



        [Test]
        public void UnauthenticatedUser_And_KnownProviderUserId_Should_LoginUser_And_RedirectToReturnUrl()
        {
            // Arrange
            var mocks = new OAuthMocks();
            var controller = GetController(mocks);
            var authResult = new AuthenticationResult(true, PROVIDER, PROVIDER_USERID, USERNAME, null);

            mocks.Response.Setup(r => r.ApplyAppPathModifier(It.IsAny<string>())).Returns(RETURN_URL);
            mocks.OAuthWebSecurity.Setup(ows => ows.VerifyAuthentication(It.IsAny<string>())).Returns(authResult);
            mocks.OAuthWebSecurity.Setup(ows => ows.Login(PROVIDER, PROVIDER_USERID, false)).Returns(true);

            // Act
            var result = controller.ExternalLoginCallback(RETURN_URL) as RedirectResult;

            // Assert
            Assert.NotNull(result, "result");
            Assert.NotNull(result.Url, RETURN_URL);
            mocks.OAuthWebSecurity.Verify(ows => ows.Login(PROVIDER, PROVIDER_USERID, false),Times.Exactly(1));
        }

        [Test]
        public void AuthenticatedUser_And_NewProviderUserId_Should_UpdateAccount()
        {
            // Arrange
            var mocks = new OAuthMocks();
            var controller = GetController(mocks);
            var authResult = new AuthenticationResult(true, PROVIDER, PROVIDER_USERID, USERNAME, null);

            mocks.Response.Setup(r => r.ApplyAppPathModifier(It.IsAny<string>())).Returns(RETURN_URL);
            mocks.OAuthWebSecurity.Setup(ows => ows.VerifyAuthentication(It.IsAny<string>())).Returns(authResult);
            mocks.OAuthWebSecurity.Setup(ows => ows.Login(PROVIDER, PROVIDER_USERID, false)).Returns(false);
            mocks.Identity.Setup(i => i.IsAuthenticated).Returns(true);
            mocks.Identity.Setup(i => i.Name).Returns(USERNAME);
            mocks.OAuthWebSecurity.Setup(ows => ows.CreateOrUpdateAccount(PROVIDER, PROVIDER_USERID, USERNAME));


            // Act
            var result = controller.ExternalLoginCallback(RETURN_URL) as RedirectResult;

            // Assert
            Assert.NotNull(result, "result");
            Assert.NotNull(result.Url, RETURN_URL);
            mocks.OAuthWebSecurity.Verify(ows => ows.CreateOrUpdateAccount(PROVIDER, PROVIDER_USERID, USERNAME), Times.Exactly(1));
        }

        [Test]
        public void ExternalLoginCallback_RedirectsTo_ExternalLoginConfirm_WhenUserIsNew()
        {
            // Arrange
            var mocks = new OAuthMocks();
            var controller = GetController(mocks);
            var extraData = new Dictionary<string, string> {{"accesstoken", "providerAccessToken"}};
            var authResult = new AuthenticationResult(true, PROVIDER, PROVIDER_USERID, USERNAME, extraData);
            var mxClient = new MxClient(new InMemoryOAuthTokenManager("a", "b"));

            mocks.Response.Setup(r => r.ApplyAppPathModifier(It.IsAny<string>())).Returns(RETURN_URL);
            mocks.OAuthWebSecurity.Setup(ows => ows.VerifyAuthentication(It.IsAny<string>())).Returns(authResult);
            mocks.OAuthWebSecurity.Setup(ows => ows.Login(PROVIDER, PROVIDER_USERID, false)).Returns(false);
            mocks.Identity.Setup(i => i.IsAuthenticated).Returns(false);

            mocks.OAuthWebSecurity.Setup(ows => ows.SerializeProviderUserId(PROVIDER, PROVIDER_USERID)).Returns(PROVIDER_USERID);
            mocks.OAuthWebSecurity.Setup(ows => ows.GetOAuthClientData(PROVIDER)).Returns(new AuthenticationClientData(mxClient, PROVIDER_DISPLAY_NAME, null));

            // Act
            var result = controller.ExternalLoginCallback(RETURN_URL) as ViewResult;

            // Assert
            Assert.NotNull(result, "result");
            Assert.AreEqual(result.ViewBag.ProviderDisplayName, PROVIDER_DISPLAY_NAME);

            var model = result.Model as RegisterExternalLoginModel;
            Assert.NotNull(model, "result");
            Assert.AreEqual(model.UserName, authResult.UserName);
            Assert.AreEqual(model.ExternalLoginData, authResult.ProviderUserId);
            Assert.AreEqual(model.AccessToken, "providerAccessToken");
        }


          [Test]
          public void ExternalLoginConfirmation()
          {
              Assert.Ignore("Not Implemented.");
          }

    }
}