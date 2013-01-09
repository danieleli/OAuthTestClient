using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;
using System.Web.Mvc;
using SimpleMembership.Models;

namespace SimpleMembership.Controllers
{
    // touch
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        public ActionResult Membership()
        {
            ViewBag.Message = "Membership Page";

            UserProfile[] users;
            using (var db = new UsersContext())
            {
                users = db.UserProfiles.Include(u => u.Tokens).ToArray();
            }
            return View(users);
        }

        public ActionResult DeleteAll()
        {
            using (var db = new UsersContext())
            {
            
                db.Database.ExecuteSqlCommand(DELETE_ALL);
                db.SaveChanges();
            }
            return this.RedirectToActionPermanent("Membership");
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private const string DELETE_ALL = @"delete from webpages_OAuthMembership 
                                            delete from UserProfile
                                            delete from OAuthTokens
                                            delete from webpages_Membership;";

    }
}
