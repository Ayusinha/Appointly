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
            Existing_staff = iConfig.GetSection("StaffRecords");
        }

        public int Create(User uc)
        {
            int doesUserAlreadyExist = 0;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("CreateUserIfNotExist", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FirstName", uc.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", uc.LastName);
                    cmd.Parameters.AddWithValue("@Email", uc.Email);
                    cmd.Parameters.AddWithValue("@Phone", uc.Phone);
                    cmd.Parameters.AddWithValue("@Pwd", uc.Pwd);
                    cmd.Parameters.AddWithValue("@UserRole", uc.UserRole);
                    cmd.Parameters.AddWithValue("@RegistrationId", uc.RegistrationId);
                    con.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.HasRows)
                        {
                            doesUserAlreadyExist = 1;
                        }
                        rdr.NextResult();
                    }
                }
            }
            return doesUserAlreadyExist;
        }

        public User Get(short userId)
        {
            User user = null;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetUserById", con))
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
                            user.DateOfBirth = dr["DateOfBirth"].ToString();
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
            int isUserUpdated = 0;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateUserById", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", user.Id);
                    cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", user.LastName);
                    cmd.Parameters.AddWithValue("@Phone", user.Phone);
                    cmd.Parameters.AddWithValue("@DateOfBirth", user.DateOfBirth);
                    cmd.Parameters.AddWithValue("@Gender", user.Gender);
                    con.Open();
                    isUserUpdated = cmd.ExecuteNonQuery();
                    
                }
            }
            return isUserUpdated;
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
                using (SqlCommand cmd = new SqlCommand("ValidateUser", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", uc.Email);
                    cmd.Parameters.AddWithValue("@Pwd", uc.Pwd);

                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        user = new User();
                        while (reader.Read())
                        {
                            user.Id = Convert.ToInt16(reader["Id"]);
                            user.UserRole = (Role)Convert.ToInt32(reader["UserRole"]);
                        }
                    }
                }
            }
            return user;
        }
    }
}
