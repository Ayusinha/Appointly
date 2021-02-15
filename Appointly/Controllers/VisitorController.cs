using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Appointly.Models;
using System.Data;
using Appointly.DAL;
using Appointly.ViewModel;
namespace Appointly.Controllers
{
    public class VisitorController : Controller
    {
        private readonly IVisitorRepository visitorRepository;
        public VisitorController(IVisitorRepository visitorRepository)
        {
            this.visitorRepository = visitorRepository;
        }
        public IActionResult Index(int? id)
        {
            if (id == 0) id = 0;
            ViewBag.Status = id;
            string userid = HttpContext.Session.GetString("User_Id");
            if (string.IsNullOrWhiteSpace(userid))
            {
                return NotFound();
            }
            short userId=Convert.ToInt16(userid);
            AppointmentStatusViewModel appointmentStatusViewModel = new AppointmentStatusViewModel();
            appointmentStatusViewModel = visitorRepository.GetVisitorAppointment(id, userId);
            return View(appointmentStatusViewModel); 
        }

        public IActionResult FacultyInfo()
        {
            string User_Id = HttpContext.Session.GetString("User_Id");
            if (!string.IsNullOrWhiteSpace(User_Id))
            { 
                List<User> users = (List<User>)visitorRepository.GetFaculty();
                return View(users);
            }
            else
            {
                return NotFound();
            }
        }
        public IActionResult Profile()
        {
            string id = HttpContext.Session.GetString("User_Id");
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }
            short userId = Convert.ToInt16(id);
            User user = visitorRepository.Update(userId);
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Profile(User user)
        {
            string id = HttpContext.Session.GetString("User_Id");
            
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }
            int num = visitorRepository.Update(user);
            if (num != -1)
            {
                ViewBag.message = "User Profile Updated.";
                return View();
            }
            else
            {
                ViewBag.message = "Something went wrong please try again.";
                return View();
            }
        }
        public IActionResult Details(int id)
        {
            string User_Id = HttpContext.Session.GetString("User_Id");
            if (User_Id != "")
            {
                List<Appointment> appointments = (List<Appointment>)visitorRepository.GetFacultyAppointment(id);
                return View(appointments);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        public IActionResult ScheduleAppointment(int id)
        {
            string User_Id = HttpContext.Session.GetString("User_Id");
            if (!string.IsNullOrWhiteSpace(User_Id))
            {
                ViewBag.Fid = id;
                return View();
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ScheduleAppointment(Appointment ap,int id)
        {
            short Id = Convert.ToInt16(id);
            string user_id = HttpContext.Session.GetString("User_Id");
            if (!string.IsNullOrWhiteSpace(user_id))
            {
                short Visitor_Id = Convert.ToInt16(user_id);
                if (ModelState.IsValid)
                {
                    visitorRepository.AddAppointment(ap, Id, Visitor_Id);
                    return RedirectToAction("Index", "Visitor");
                }
                else
                {
                    ViewBag.message = "Something went wrong please try again.";
                    return View();
                }
            }
            else
            {
                return NotFound();
            }
        }
        [HttpGet]
        public IActionResult Logout()
        {
            string User_Id = HttpContext.Session.GetString("User_Id");
            if (!string.IsNullOrWhiteSpace(User_Id))
            {
                HttpContext.Session.Clear();
                HttpContext.Session.SetString("User_Id", ""); ;
                return RedirectToAction("Login", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
    }
}

