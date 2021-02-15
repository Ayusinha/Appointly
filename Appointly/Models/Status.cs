using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Appointly.Models
{
    public class Status
    {
        [Key]
        public byte Id { get; set; }

        [Display(Name = "Status")]
        public string Status_value { get; set; }
    }
}
