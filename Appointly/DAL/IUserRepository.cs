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
        public int Create(User uc);

        public User Login(User uc);

        public List<string> ExistingFaculties();
    }
}
