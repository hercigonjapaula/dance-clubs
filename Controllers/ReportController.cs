using DanceClubs.Data;
using DanceClubs.Data.Models;
using DanceClubs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DanceClubs.Controllers
{
    //[Authorize(Roles = "User")]
    public class ReportController : Controller
    {
        private readonly IRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReportController(IRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var modelList = new List<ReportListingModel>();
            string[] months = new string[] { "Rujan", "Listopad", "Studeni", "Prosinac" };
            foreach (var month in months)
            {
                modelList.Add(new ReportListingModel
                {
                    Month = month,
                    Quantity = 100
                });
            }
            return View(modelList);
        }

        /*public IActionResult GetClasses()
        {
            
        }*/

        public JsonResult GetRehearsals()
        {
            int[] classes = new int[] { 130, 140, 150, 160, 170, 180, 190, 100, 110, 120, 130, 140 };
            return Json(classes);            
        }

        public JsonResult GetPerformances()
        {
            int[] classes = new int[] { 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140 };
            return Json(classes);           
        }

        public JsonResult GetMembershipFees()
        {
            int[] classes = new int[] { 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140 };
            return Json(classes);
        }
    }
}
