using Appointly.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Appointly.DAL
{

    public class UserRepository : IUserRepository
    {
        private readonly string connectionString;
        private readonly IConfiguration Existing_staff;

        public UserRepository(IConfiguration iConfig)
        {
            connectionString = iConfig.GetSection("ConnectionStrings").GetSection("Myconnection").Value;
            Existing_staff = iConfig.GetSection("Staff_List");
        }
        public int Create(User uc)
        {
            int isUserAlreadyExist = 0;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_createuser", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FirstName", uc.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", uc.LastName);
                    cmd.Parameters.AddWithValue("@Email", uc.Email);
                    cmd.Parameters.AddWithValue("@Phone", uc.Phone);
                    cmd.Parameters.AddWithValue("@Pwd", uc.Pwd);
                    cmd.Parameters.AddWithValue("@User_Role", uc.User_Role);
                    cmd.Parameters.AddWithValue("@Registration_Id", uc.Registration_Id);
                    con.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        bool next_query = true;
                        while (next_query == true)
                        {
                            if (rdr.HasRows)
                            {
                                isUserAlreadyExist = 1;
                                break;
                            }
                            else
                            {
                                if (rdr.NextResult())
                                {
                                    next_query = true;
                                }
                                else
                                {
                                    next_query = false;
                                }
                            }
                        }
                    }
                }
            }
            return isUserAlreadyExist;
        }

        public List<string> ExistingFaculties()
        {
            return Existing_staff.Get<List<string>>();
        }

        public User Login(User uc)
        {
            User user = null;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_signin", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", uc.Email);
                    cmd.Parameters.AddWithValue("@Pwd", uc.Pwd);

                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows) user = new User();
                        while (reader.Read())
                        {
                            user.Id = (short)Convert.ToInt32(reader["Id"]);
                            user.User_Role = Convert.ToString(reader["User_Role"]);
                        }
                    }
                }
            }
            return user;
        }
    }
}
