using Appointly.Models;
using Appointly.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appointly.DAL
{
    public interface IVisitorRepository
    {
        public AppointmentStatusViewModel GetVisitorAppointment(int? id, short uid); //index

        public IEnumerable<User> GetFaculty(); //facultyname schedule

        public IEnumerable<Appointment> GetFacultyAppointment(int id); //details

        public User Update(short id); //profile http get

        public int Update(User user); //profile http post 

        public void AddAppointment(Appointment appointment, int id, short Vid); //schedule appointment http post 

    }
}