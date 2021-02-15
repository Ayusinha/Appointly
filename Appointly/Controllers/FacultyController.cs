using Appointly.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Appointly.Controllers
{
    public class FacultyController : Controller
    {
        private readonly string connectionString;
        public FacultyController(IConfiguration iConfig) : base()
        {
            connectionString = iConfig.GetSection("ConnectionStrings").GetSection("Myconnection").Value;
        }

        [HttpGet]
        public IActionResult Index()
        {
            string userid = HttpContext.Session.GetString("User_Id");
            if (string.IsNullOrWhiteSpace(userid))
            {
                return NotFound();
            }
            List<Appointment> appoint = null;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_getAllAppointment", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", userid);
                    cmd.Parameters.AddWithValue("@Status", 1);
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            appoint = new List<Appointment>();
                            while (reader.Read())
                            {
                                var app = new Appointment();
                                app.Id = (short)Convert.ToInt32(reader["Id"].ToString());
                                app._From = (DateTime)reader["_From"];
                                app._To = (DateTime)reader["_To"];
                                app.Purpose = reader["purpose"].ToString();
                                app.Visitor_Id = (short)reader["Visitor_Id"];
                                appoint.Add(app);
                            }
                        }
                    }
                }
            }
            return View(appoint);
        }

        [HttpPost]
        public IActionResult Index(int id, string Response, string submit)
        {
            byte val=0;
            if (submit == "Accept")
            {
                val = 2;
            }else if (submit == "Decline")
            {
                val = 3;
            }
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_updateAppointmentstatus", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@status", val);
                    cmd.Parameters.AddWithValue("@Id", (short)id);
                    if (!string.IsNullOrEmpty(Response))
                    {
                        cmd.Parameters.AddWithValue("@Response", Response);
                    }
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            ViewBag.message = "Done Successfully";
            return RedirectToAction("index");
        }

        public IActionResult Profile()
        {
            string id = HttpContext.Session.GetString("User_Id");
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }
            short userId = Convert.ToInt16(id);
            User uc = new User();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_getuserbyid", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", userId);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    uc.Id = (short)Convert.ToInt32(dr["Id"].ToString());
                    uc.FirstName = dr["FirstName"].ToString();
                    uc.LastName = dr["LastName"].ToString();
                    uc.Date_of_birth = dr["Date_of_birth"].ToString();
                    uc.Gender = dr["Gender"].ToString();
                    uc.Phone = dr["Phone"].ToString();
                }
                con.Close();
            }
            if (uc == null)
            {
                return NotFound();
            }
            return View(uc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Profile([Bind] User userEmp)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_updateuser", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", userEmp.Id);
                cmd.Parameters.AddWithValue("@FirstName", userEmp.FirstName);
                cmd.Parameters.AddWithValue("@LastName", userEmp.LastName);
                cmd.Parameters.AddWithValue("@Phone", userEmp.Phone);
                cmd.Parameters.AddWithValue("@Date_of_birth", userEmp.Date_of_birth);
                cmd.Parameters.AddWithValue("@Gender", userEmp.Gender);
                con.Open();
                int num = cmd.ExecuteNonQuery();
                con.Close();

                if (num != -1)
                {
                    ViewBag.message = "User Profile Updated.";
                    return View();
                }
                else
                {
                    ViewBag.message = "Something went wrong please try again.";
                    return View();
                }
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            string User_Id = HttpContext.Session.GetString("User_Id");
            if (!string.IsNullOrWhiteSpace(User_Id))
            {
                HttpContext.Session.Clear();
                HttpContext.Session.SetString("User_Id", ""); ;
                return RedirectToAction("Login", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
    }
}
