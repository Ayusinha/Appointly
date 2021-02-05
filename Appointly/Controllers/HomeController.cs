using Appointly.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Appointly.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationUser _auc;
        private IConfiguration _configuration;
        public HomeController(ILogger<HomeController> logger, ApplicationUser auc, IConfiguration iConfig)
        {
            _logger = logger;
            _auc = auc;
            _configuration = iConfig;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Login", "Home");
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(Users uc)
        {
            
            string role=HttpContext.Request.Form["User_Role"].ToString();
            string reg_no= HttpContext.Request.Form["Registration_Id"].ToString();
            string pwd= HttpContext.Request.Form["Pwd"].ToString();
            string cpwd = HttpContext.Request.Form["confirm_Pwd"].ToString();
            var arr = _configuration.GetSection("Staff_List");
            var list = arr.Get<string[]>();

            if (pwd != cpwd)
            {
                ViewBag.message = "Password and Confirm password field doesn't match enter your details again.";
                return View();
            }
            else
            {
                if (role == "faculty" || role == "admin")
                {
                    if (!list.Contains(reg_no) || reg_no == "")
                    {
                        ViewBag.message = "Enter a Valid staff ID for registering as a Faculty or Admin.";
                        return View();
                    }
                }
            }
            _auc.Add(uc);
            _auc.SaveChanges();
            return RedirectToAction("Login", "Home");
        }

        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
