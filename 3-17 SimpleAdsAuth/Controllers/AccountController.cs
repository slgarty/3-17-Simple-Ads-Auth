using _3_17_SimpleAdsAuth.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace _3_17_SimpleAdsAuth.Views
{
    public class AccountController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=Ads;Integrated Security=true;";

        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Signup(User user, string password)
        {
            var db = new AdDb(_connectionString);
            db.AddUser(user, password);
            return Redirect("/account/login");
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var db = new AdDb(_connectionString);
            var user = db.Login(email, password);
            if (user == null)
            {
                TempData["message"] = "Invalid email/password combination";
                return Redirect("/account/login");
            }

            var claims = new List<Claim>
            {
                new Claim("user", email) 
            };

          
            HttpContext.SignInAsync(new ClaimsPrincipal(
                new ClaimsIdentity(claims, "Cookies", "user", "role"))).Wait();

            return Redirect("/home/newAd");
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync().Wait();
            return Redirect("/");
        }
        [Authorize]
        public IActionResult MyAccount()
        {
            var db = new AdDb(_connectionString);
            var vm = new HomeViewModel();
            var email = User.Identity.Name;
            vm.CurrentUser = db.GetByEmail(email);
            vm.SimpleAds = db.GetSimpleAds().Where(s => s.UserId == vm.CurrentUser.Id).ToList();
            return View(vm);
        }
    }
}
