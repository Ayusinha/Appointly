using Appointly.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using System.Linq;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Data;
using Appointly.DAL;

namespace Appointly.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserRepository userRepository;

        public HomeController(ILogger<HomeController> logger,IUserRepository userRepository)
        {
            _logger = logger;
            this.userRepository = userRepository;
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
        public IActionResult Register(User uc)
        {
            if (ModelState.IsValid)
            {
                string role = Convert.ToString(HttpContext.Request.Form["User_Id"]);
                string reg_no = Convert.ToString(HttpContext.Request.Form["Registration_Id"]);
                string pwd = Convert.ToString(HttpContext.Request.Form["Pwd"]);
                string cpwd = Convert.ToString(HttpContext.Request.Form["confirm_Pwd"]);
                var list = userRepository.ExistingFaculties();
                int isUserAlreadyExist = userRepository.Create(uc);
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
                if(isUserAlreadyExist == 1)
                {
                    ViewBag.message = "Entered Email is already registered Please Enter Again";
                    return View();
                }
                else
                {
                    ViewBag.success = "User Registered Successfully";
                    return View();
                }
            }
            else
            {
                ViewBag.message = "Something went wrong please try again.";
                return View();
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(User user)
        {
            User uc = userRepository.Login(user);
            if (uc != null) { 
                //setting value into a session key
                HttpContext.Session.SetString("User_Id", Convert.ToString(uc.Id));
                if (uc.User_Role == "visitor")
                {
                    return RedirectToAction("Index", "Visitor");
                }
                else if (uc.User_Role == "faculty")
                {
                    return RedirectToAction("Index", "Faculty");
                }
                else if (uc.User_Role == "admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    ViewBag.message = "You entered wrong email or password, Please try again.";
                    return View();
                }
            }
            else
            {
                ViewBag.message = "You entered wrong email or password, Please try again.";
                return View();
            }
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
