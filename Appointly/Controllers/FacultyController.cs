using Appointly.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Appointly.Controllers
{
    public class FacultyController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly SqlConnection con;

        public FacultyController(IConfiguration iConfig) : base()
        {
            _configuration = iConfig;
            _connectionString = _configuration.GetSection("ConnectionStrings").GetSection("Myconnection").Value;
            con = new SqlConnection(_connectionString);

        }
        [HttpGet]
        public IActionResult Index()
        {
            List<Appointment> apo = new List<Appointment>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_getallapprovedappointment", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var a = new Appointment();
                    a.Id = (short)Convert.ToInt32(dr["Id"].ToString());
                    a._From = (DateTime)dr["_From"];
                    a._To = (DateTime)dr["_To"];
                    a.Purpose = dr["purpose"].ToString();
                    //a.Visitor = (Users)dr["Visitor"];
                    apo.Add(a);
                }
                con.Close();
            }

            return View(apo);
        }
        [HttpPost]
        public IActionResult Index(Appointment ap, string value)
        {

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_updateappointmentstatus", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                con.Open();
                cmd.Parameters.AddWithValue("@status", value);
                cmd.Parameters.AddWithValue("@Id", ap.Id);
                cmd.ExecuteNonQuery();

                con.Open();

                con.Close();
                con.Close();
            }

            return RedirectToAction("index");
        }
    }
}
