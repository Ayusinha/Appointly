using Appointly.Models;
using Appointly.ViewModel;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Appointly.DAL
{
    public class VisitorRepository : IVisitorRepository
    {
        private readonly string connectionString;
      

        public VisitorRepository(IConfiguration iConfig)
        {
            connectionString = iConfig.GetSection("ConnectionStrings").GetSection("Myconnection").Value;
        }

        public void AddAppointment(Appointment appointment, int id,short Visitor_Id)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_scheduleappointment", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Visitor_Id", Visitor_Id);
                    cmd.Parameters.AddWithValue("@Faculty_Id", id);
                    cmd.Parameters.AddWithValue("@From", appointment.From);
                    cmd.Parameters.AddWithValue("@To", appointment.To);
                    cmd.Parameters.AddWithValue("@Purpose", appointment.Purpose);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }


        public IEnumerable<Appointment> GetFacultyAppointment(int id)
        {
            List<Appointment> appointments = null;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
               
                SqlCommand cmd1 = new SqlCommand("sp_getmeetingdetail", con);
                cmd1.Parameters.AddWithValue("@FacultyId", id);
                cmd1.CommandType = CommandType.StoredProcedure;

                con.Open();
                using (SqlDataReader dr = cmd1.ExecuteReader())
                {
                    appointments = new List<Appointment>();
                    while (dr.Read())
                    {
                        Appointment ap = new Appointment();
                        ap.From = (DateTime)dr["From"];
                        ap.To = (DateTime)dr["To"];
                        ap.FacultyId = (short)Convert.ToInt32(dr["FacultyId"]);
                        ap.Extra = (string)dr["FirstName"];
                        ap.Extra += " ";
                        ap.Extra += dr["LastName"];
                        appointments.Add(ap);
                    }
                }
            }
            return appointments;
        }

        public IEnumerable<User> GetFaculty()
        {
            // throw new NotImplementedException();
            List<User> users  = null;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_getallfaculty", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        users= new List<User>(); ;
                        while (dr.Read())
                        {
                            User uc = new User();
                            uc.Id = (short)Convert.ToInt32(dr["Id"].ToString());
                            uc.FirstName = dr["FirstName"].ToString();
                            uc.LastName = dr["LastName"].ToString();
                            uc.Email = dr["Email"].ToString();
                            uc.Phone = dr["Phone"].ToString();
                            users.Add(uc);
                        }
                    }
                }
            }
            return users;
        }


        public AppointmentStatusViewModel GetVisitorAppointment(int? id,short userid)
        {
            if (id == 0) id = 1;
            List<Appointment> appointments = new List<Appointment>();
            AppointmentStatusViewModel appointmentStatusViewModel = new AppointmentStatusViewModel();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_getmeetingfromstatus", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@visitorId", userid);
                    cmd.Parameters.AddWithValue("@status", id);
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        for (int i = 1; i <=5;  i++)
                        {
                            while (dr.Read())
                            {
                                switch (i)
                                {
                                    case 1:
                                       appointmentStatusViewModel.Pending = Convert.ToInt16(dr["meeting_count"]);
                                       
                                        break;
                                    case 2:
                                       appointmentStatusViewModel.Approved = Convert.ToInt16(dr["meeting_count"]);
                                       
                                        break;
                                    case 3:
                                       appointmentStatusViewModel.Rejected= Convert.ToInt16(dr["meeting_count"]);
                                        
                                        break;
                                    case 4:
                                        appointmentStatusViewModel.Completed = Convert.ToInt16(dr["meeting_count"]);
                                        
                                        break;
                                    case 5:
                                        appointmentStatusViewModel.Total = Convert.ToInt16(dr["meeting_count"]);
                                        break;
                                }

                            }
                            dr.NextResult();
                        }
                       
                        while (dr.Read())
                        {
                            Appointment app = new Appointment();
                            app.Extra = Convert.ToString(dr["FirstName"]);
                            app.Extra += ' ';
                            app.Extra += (string)dr["LastName"];
                            app.From = (DateTime)dr["_From"];
                            app.To = (DateTime)dr["_To"];
                            app.Purpose = Convert.ToString(dr["Purpose"]);
                            appointments.Add(app);
                        }
                        appointmentStatusViewModel.appointments = appointments;
                    }
                }
            }
            return appointmentStatusViewModel;
          
        }
    }
}