using Appointly.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Appointly.DAL
{
    public interface IUserRepository
    {
        public int Create(User user);

        public User Login(User user);

        public User Get(short userId);

        public int Update(User user);

        public List<string> ExistingFaculties();
    }
}
