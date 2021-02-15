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


        public void AddAppointment(int id)
        {
            throw new NotImplementedException();
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
                    cmd.Parameters.AddWithValue("@From", appointment._From);
                    cmd.Parameters.AddWithValue("@To", appointment._To);
                    cmd.Parameters.AddWithValue("@Purpose", appointment.Purpose);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
           // throw new NotImplementedException();
        }


        public IEnumerable<Appointment> GetFacultyAppointment(int id)
        {
            List<Appointment> appointments = null;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
               
                SqlCommand cmd1 = new SqlCommand("sp_getmeetingdetail", con);
                cmd1.Parameters.AddWithValue("@fid", id);
                cmd1.CommandType = CommandType.StoredProcedure;

                con.Open();
                using (SqlDataReader dr = cmd1.ExecuteReader())
                {
                    appointments = new List<Appointment>();
                    while (dr.Read())
                    {
                        Appointment ap = new Appointment();
                        ap._From = (DateTime)dr["_From"];
                        ap._To = (DateTime)dr["_To"];
                        ap.Faculty_Id = (short)Convert.ToInt32(dr["faculty_Id"].ToString());
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
                         //   users = 
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
                                       appointmentStatusViewModel.Pending = (short)Convert.ToInt32(dr["meeting_count"].ToString());
                                       
                                        break;
                                    case 2:
                                       appointmentStatusViewModel.Approved = (short)Convert.ToInt32(dr["meeting_count"].ToString());
                                       
                                        break;
                                    case 3:
                                       appointmentStatusViewModel.Rejected= (short)Convert.ToInt32(dr["meeting_count"].ToString());
                                        
                                        break;
                                    case 4:
                                        appointmentStatusViewModel.Completed = (short)Convert.ToInt32(dr["meeting_count"].ToString());
                                        
                                        break;
                                    case 5:
                                        appointmentStatusViewModel.Total = (short)Convert.ToInt32(dr["meeting_count"].ToString());
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
                            app._From = (DateTime)dr["_From"];
                            app._To = (DateTime)dr["_To"];
                            app.Purpose = Convert.ToString(dr["Purpose"]);
                            appointments.Add(app);
                        }
                        appointmentStatusViewModel.appointments = appointments;
                    }
                }
            }
            return appointmentStatusViewModel;
          
        }


        public User Update(short id)
        {
            short userId = Convert.ToInt16(id);
            User user = null;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_getuserbyid", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", userId);
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows) user = new User();
                        while (dr.Read())
                        {
                            user.Id = (short)Convert.ToInt32(dr["Id"].ToString());
                            user.FirstName = dr["FirstName"].ToString();
                            user.LastName = dr["LastName"].ToString();
                            user.Date_of_birth = dr["Date_of_birth"].ToString();
                            user.Gender = dr["Gender"].ToString();
                            user.Phone = dr["Phone"].ToString();
                        }
                    }
                }
            }
            return user;
        }

        public int Update(User user)
        {
            int num = 0;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_updateuser", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", user.Id);
                    cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", user.LastName);
                    cmd.Parameters.AddWithValue("@Phone", user.Phone);
                    cmd.Parameters.AddWithValue("@Date_of_birth", user.Date_of_birth);
                    cmd.Parameters.AddWithValue("@Gender", user.Gender);
                    con.Open();
                    num = cmd.ExecuteNonQuery();
                   
                }
            }
            return num;
            
        }
    }
}