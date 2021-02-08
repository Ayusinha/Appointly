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
        [Key]
        public short Id { get; set; }

        public virtual Users Visitor_Id { get; set; }

        public short Faculty_Id { get; set; }

        [Display(Name = "From")]
        public DateTime _From { get; set; }

        [Display(Name = "To")]
        public DateTime _To { get; set; }

        public DateTime _Entry { get; set; }


        public DateTime _Exit { get; set; }

        [Display(Name = "Purpose")]
        public string Purpose { get; set; }

        public string Response { get; set; }

        public virtual short _Status { get; set; }

        [ForeignKey("Visitor_Id")]
        public virtual Users User { get; set; }
        [ForeignKey("_Status")]
        public virtual Status Statuses { get; set; }
    }
}
