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

        public async Task<ActionResult> Index()
        {
            var model = await GetFullAndPartialViewModel(_repository.GetOneClubByUserId(_userManager.GetUserId(User)), DateTime.Now.Year);
            return View(model);
        }

        public async Task<ActionResult> RefreshChart(int clubId, int year)
        {
            var model = await GetFullAndPartialViewModel(clubId, year);
            return PartialView("Chart", model);
        }

        private async Task<ReportModel> GetFullAndPartialViewModel(int clubId, int year)
        {
            Dictionary<int, string> monthNames = new Dictionary<int, string>
            {
                { 1, "Siječanj" },
                { 2, "Veljača" },
                { 3, "Ožujak" },
                { 4, "Travanj" },
                { 5, "Svibanj" },
                { 6, "Lipanj" },
                { 7, "Srpanj" },
                { 8, "Kolovoz" },
                { 9, "Rujan" },
                { 10, "Listopad" },
                { 11, "Studeni" },
                { 12, "Prosinac" }
            };
            var userId = _userManager.GetUserId(User);
            var clubUsers = _repository.GetClubUsersByUserId(userId);
            var clubList = new List<ClubReportModel>();
            var club = _repository.GetClubById(clubId);
            var report = new Dictionary<string, List<ReportListingModel>>();
            var activityTypes = _repository.GetAllActivityTypes();
            int[] months = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var years = new List<YearModel>();
            for (var i = 0; i < 30; i++)
            {
                years.Add(new YearModel{
                    Year = DateTime.Now.AddYears(-i).Year
                });
            }
            foreach (var clubUser in clubUsers)
            {
                clubList.Add(new ClubReportModel
                {
                    ClubId = clubUser.ClubId,
                    ClubName = clubUser.Club.Name
                });
            }   
            foreach(var activityType in activityTypes)
            {
                report[activityType.Name] = new List<ReportListingModel>();
                foreach(var month in months)
                {
                    var activities = _repository.GetActivitiesByClubIdActivityTypeIdMonthYear(clubId, activityType.Id, month, year);
                    report[activityType.Name].Add(new ReportListingModel
                    {
                        Month = monthNames[month],
                        Quantity = activities.Count
                    });
                }                    
            }            
            var model = new ReportModel
            {
                ClubId = clubId,
                Year = year,
                ClubList = clubList,
                Report = report,
                YearList = years                
            };
            return model;
        }
    }
}
