﻿using Appointly.Models;
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

namespace Appointly.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        //private readonly ApplicationUser _auser;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly SqlConnection con,con1;

        public object Labelinfo { get; private set; }

        public HomeController(ILogger<HomeController> logger, IConfiguration iConfig)
        {
            _logger = logger;
            //_auser = auser;
            _configuration = iConfig;
            _connectionString = _configuration.GetSection("ConnectionStrings").GetSection("Myconnection").Value;
            con = new SqlConnection(_connectionString);
            con1 = new SqlConnection(_connectionString);
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

        public bool CheckUser(Users uc) 
        {
            bool ans = false;
            SqlCommand cmd = new SqlCommand("sp_signin", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Email", uc.Email);
            cmd.Parameters.AddWithValue("@Pwd", uc.Pwd);

            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                ans = true;
            }
            con.Close();
            return ans;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(Users uc)
        {
            if (ModelState.IsValid)
            {
                string role = HttpContext.Request.Form["User_Role"].ToString();
                string reg_no = HttpContext.Request.Form["Registration_Id"].ToString();
                string pwd = HttpContext.Request.Form["Pwd"].ToString();
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
                if (CheckUser(uc) == true)
                {
                    ViewBag.success = "Entered Email is already registered Please Login..";
                    return View();
                }
                else
                {
                    SqlCommand cmd = new SqlCommand("sp_createuser", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FirstName", uc.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", uc.LastName);
                    cmd.Parameters.AddWithValue("@Email", uc.Email);
                    cmd.Parameters.AddWithValue("@Phone", uc.Phone);
                    cmd.Parameters.AddWithValue("@Pwd", uc.Pwd);
                    cmd.Parameters.AddWithValue("@User_Role", uc.User_Role);
                    con.Open();
                    cmd.Parameters.AddWithValue("@Registration_Id", uc.Registration_Id);
                    cmd.ExecuteNonQuery();
                    con.Close();
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
        public IActionResult Login(Users uc)
        {
           SqlCommand cmd = new SqlCommand("sp_signin", con);
            
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Email", uc.Email);
                cmd.Parameters.AddWithValue("@Pwd", uc.Pwd);

                string User_Id = "",role="";
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        User_Id = reader["Id"].ToString();
                        role = reader["User_Role"].ToString();
                    }
                    //setting value into a session key
                    HttpContext.Session.SetString("User_Id", User_Id);
                    con.Close();
                    if(role == "visitor")
                    {
                        return RedirectToAction("MyMeeting","Visitor");
                    }
                    else if(role == "faculty")
                    {
                        return RedirectToAction("Index","Faculty");
                    }
                    else if(role == "admin")
                    {
                        return RedirectToAction("Index","Admin");
                    }
                    else
                    {
                        ViewBag.message = "You entered wrong email or password, Please try again.";
                        return View();
                    }
                }
                else
                {
                    con.Close();
                    ViewBag.message = "You entered wrong email or password, Please try again.";
                    return View();
                }
            
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("User_Id");
            return RedirectToAction("Login","Home");
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
