using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using NUnit.Framework;
using Rhino.Mocks;
using SimpleMembership.Auth.OAuth1a;
using SimpleMembership.Controllers;

namespace SimpleMembership._Tests.OAuth.OAuth1a
{
    [TestFixture]
    public class MxClient_Tests
    {
        //[Test]
        //public void RequestLoginToMxMerchant_Redirects_ToMxMerchant()
        //{

        //    var repo = new MockRepository();
        //    var baseContext = repo.DynamicMock<HttpContextBase>();
        //    var requestContext = repo.DynamicMock<RequestContext>();
        //    requestContext.HttpContext = baseContext;
        //    var ctrlContext = repo.DynamicMock<ControllerContext>();
        //    ctrlContext.HttpContext = baseContext;

        //    var urlHelper = repo.Stub<UrlHelper>(requestContext);
            
        //    const string returnUrl = "testing";

        //    var ctrl = new AccountController
        //        {
        //            Url = urlHelper,
        //            ControllerContext = ctrlContext
        //        };


        //    repo.Record();
        //    requestContext.Expect(c => c.RouteData).PropertyBehavior().Return(null);
        //    urlHelper.Expect(a => a.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl })).Return("test");

        //    repo.Playback();
        //    var result = ctrl.ExternalLogin(MxClient.NAME, returnUrl);
            
        //    Assert.IsNotNull(result);
        //    Assert.That(result, Is.TypeOf<AccountController.ExternalLoginResult>(),"type");
        //    var strongResult = (AccountController.ExternalLoginResult) result;
        //    Assert.That(strongResult.ReturnUrl, Is.EqualTo(returnUrl), "returnUrl");
        //    Assert.That(strongResult.Provider, Is.EqualTo(MxClient.NAME), "Provider");
            
        //}
    }
}
