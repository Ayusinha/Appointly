using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Appointly.Models
{
    public class Users
    {
        [Key]
        public short Id { get; set; }

        [Required(ErrorMessage = "Please Select a Role..")]
        [Display(Name = "User_Role")]
        public string User_Role { get; set; }
        
        [Required(ErrorMessage = "Please Enter Firstname..")]
        [Display(Name = "FirstName")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please Enter Lastname..")]
        [Display(Name = "LastName")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please Enter Email..")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please Enter Phone Number..")]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Please Enter Password..")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Minimum 6 characters are required")]
        [Display(Name = "Password")]
        public string Pwd { get; set; }

        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Display(Name = "Date of birth")]
        public string Date_of_birth { get; set; }

        [Display(Name = "Registration Id")]
        public string Registration_Id { get; set; }
    }
}