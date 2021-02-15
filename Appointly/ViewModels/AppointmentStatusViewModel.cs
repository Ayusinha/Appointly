using Appointly.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appointly.ViewModel
{
    public class AppointmentStatusViewModel
    {
        public int Pending { get; set; }

        public int Approved { get; set; }

        public int Rejected { get; set; }

        public int Completed { get; set; }

        public int Total { get; set; }

        //List<Customer>
        public List<Appointment> appointments;


    }
}