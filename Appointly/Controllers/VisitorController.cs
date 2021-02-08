using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Appointly.Models;
using System.Data;

namespace Appointly.Controllers
{
    public class VisitorController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly SqlConnection con;

        public object Labelinfo { get; private set; }

        public VisitorController(IConfiguration iConfig)
        {
            _configuration = iConfig;
            _connectionString = _configuration.GetSection("ConnectionStrings").GetSection("Myconnection").Value;
            con = new SqlConnection(_connectionString);
        }
        public IActionResult Index()
        {
            List<Users> user = new List<Users>();
            string User_Id = HttpContext.Session.GetString("User_Id");
            if(User_Id != null)
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_getallfaculty", con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        Users uc = new Users();
                        uc.Id = (short)Convert.ToInt32(dr["Id"].ToString());
                        uc.FirstName = dr["FirstName"].ToString();
                        uc.LastName = dr["LastName"].ToString();
                        uc.Email = dr["Email"].ToString();
                        uc.Phone = dr["Phone"].ToString();
                        user.Add(uc);
                    }
                    con.Close();
                }
                List<Users> userList = new List<Users>();
                userList = user.ToList();
                return View(userList);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        public IActionResult MyMeeting()
        {
            string str = HttpContext.Session.GetString("User_Id");
            return View();
        }
        public IActionResult Profile()
        {
            string id = HttpContext.Session.GetString("User_Id");
            if (id == null)
            {
                return RedirectToAction("Login", "Home");
            }
            string userid = HttpContext.Session.GetString("User_Id");
            short userId = Convert.ToInt16(userid);
            //return Json(userId + 10); 
            if (userid == null)
            {
                return NotFound();
            }
            Users uc = new Users();
            using (SqlConnection con = new SqlConnection(_connectionString))
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
        public IActionResult Profile([Bind] Users userEmp)
        {
            //return Json(userEmp);
            string id = HttpContext.Session.GetString("User_Id");
            if (id == null)
            {
                return RedirectToAction("Login", "Home");
            }
            using (SqlConnection con = new SqlConnection(_connectionString))
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
                    ViewBag.message = "User Profile Updated...";
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
            HttpContext.Session.Remove("User_Id");
            return RedirectToAction("Login", "Home");
        }

        public IActionResult Details(int? id)
        {
            List<Appointment> ap = new List<Appointment>();
            string User_Id = HttpContext.Session.GetString("User_Id");
            if (User_Id != null)
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    string faculty_name = "";
                    SqlCommand cmd = new SqlCommand("sp_getnamebyid", con);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        faculty_name = (string)dr["FirstName"];
                        faculty_name += " ";
                        faculty_name += dr["LastName"];
                    }
                    con.Close();
                    ViewBag.faculty_name = faculty_name;
                    ViewBag.FId = id;

                    SqlCommand cmd1 = new SqlCommand("sp_getmeetingdetail", con);
                    cmd1.Parameters.AddWithValue("@fid", id);
                    cmd1.CommandType = CommandType.StoredProcedure;

                    con.Open();
                    dr = cmd1.ExecuteReader();
                    while (dr.Read())
                    {
                        Appointment uc = new Appointment();

                        uc._From = (DateTime)dr["_From"];
                        uc._To = (DateTime)dr["_To"];
                        uc.Faculty_Id = (short)Convert.ToInt32(dr["faculty_Id"].ToString());

                        ap.Add(uc);
                    }
                    con.Close();
                }
                List<Appointment> apList = new List<Appointment>();
                apList = ap.ToList();
                return View(apList);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        public IActionResult ScheduleAppointment(int id)
        {
            ViewBag.Fid = id;
            ViewBag.Vid = HttpContext.Session.GetString("User_Id");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ScheduleAppointment(Appointment ap)
        {
            //return Json(ap);
            if (ModelState.IsValid)
            {
                SqlCommand cmd = new SqlCommand("sp_scheduleappointment", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Visitor_Id", ap.Visitor_Id);
                cmd.Parameters.AddWithValue("@Faculty_Id", ap.Faculty_Id);
                cmd.Parameters.AddWithValue("@From", ap._From);
                cmd.Parameters.AddWithValue("@To", ap._To);
                cmd.Parameters.AddWithValue("@Purpose", ap.Purpose);

                con.Open();
                //if(user.Registration_Id!=null)

                cmd.ExecuteNonQuery();
                con.Close();

                return RedirectToAction("Index", "Visitor");
            }
            else
            {
                ViewBag.message = "Something went wrong please try again.";
                return View();
            }

        }
    }
}