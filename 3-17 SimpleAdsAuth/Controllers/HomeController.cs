using _3_17_SimpleAdsAuth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace _3_17_SimpleAdsAuth.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=Ads;Integrated Security=true;";
        public IActionResult Index()
        {
            var db = new AdDb(_connectionString);          
            var vm = new HomeViewModel();
            vm.SimpleAds = db.GetSimpleAds();

            vm.IsAuthenticated = User.Identity.IsAuthenticated;
            if (User.Identity.IsAuthenticated)
            {
                var email = User.Identity.Name; 
                vm.CurrentUser = db.GetByEmail(email);
            }
            return View(vm);
        }
        [Authorize]
        public IActionResult NewAd()
        {
            var db = new AdDb(_connectionString);
            var vm = new HomeViewModel();
                vm.IsAuthenticated = User.Identity.IsAuthenticated;
                var email = User.Identity.Name;
                vm.CurrentUser = db.GetByEmail(email);
            
            return View(vm);
        }
        [Authorize]
        [HttpPost]
        public IActionResult NewAd(SimpleAd simpleAd)
        {
            var db = new AdDb(_connectionString);
            var vm = new HomeViewModel();
            var email = User.Identity.Name;
            vm.CurrentUser = db.GetByEmail(email);

            db.AddAd(simpleAd);

            return Redirect("/home/index");

        }
        [Authorize]
        [HttpPost]
        public ActionResult DeleteAd(int id)
        {
            var db = new AdDb(_connectionString);
            db.DeleteAdd(id);
            return Redirect("/home/index");
        }

    }
}
