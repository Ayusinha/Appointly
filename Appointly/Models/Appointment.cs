using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Appointly.Models
{
    public class Appointment
    {
        public short Id { get; set; }

        public short VisitorId { get; set; }

        public short FacultyId { get; set; }

        [Display(Name = "From")]
        public DateTime From { get; set; }

        [Display(Name = "To")]
        public DateTime To { get; set; }

        public DateTime Entry { get; set; }

        public DateTime Exit { get; set; }

        [Display(Name = "Purpose")]
        public string Purpose { get; set; }

        public string Response { get; set; }

        public Status Status { get; set; }

        public string Extra { get; set; }
    }
}
