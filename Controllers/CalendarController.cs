using DanceClubs.Data;
using DanceClubs.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ical.Net;
using Ical.Net.CalendarComponents;
using System.Linq;
using Ical.Net.DataTypes;
using System;
using Microsoft.AspNetCore.Authorization;
using Ical.Net.Serialization;
using DanceClubs.Models;

namespace DanceClubs.Controllers
{
    //[Authorize(Roles = "User")]
    public class CalendarController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CalendarController(ApplicationDbContext context, IRepository repository, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _repository = repository;
            _userManager = userManager;       
        }

        public IActionResult Index()
        {

            var userId = _userManager.GetUserId(User);

            var groupUsers = _repository.GetGroupUsersByUserId(userId);
            var activities = groupUsers.SelectMany(i => _repository.GetActivitiesByGroupId(i.GroupId)).ToList();
            var model = new CalendarIndexModel();
            model.Activities = activities.Select(a => new ActivityListingModel
            {
                ActivityType=a.ActivityType.Name,
                Author=a.Author.UserName,
                Start=a.Start,
                End=a.End,
                Group=a.Group.Name,
                Location=a.Location
            }).ToList();
            return View(model);
        }

        public IActionResult ICal()
        {
            var userId = _userManager.GetUserId(User);

            var groupUsers = _repository.GetGroupUsersByUserId(userId);
            var activities = groupUsers.SelectMany(i => _repository.GetActivitiesByGroupId(i.GroupId)).ToList();

            var calendar = new Calendar();

            foreach(var activity in activities)
            {
                calendar.Events.Add(new CalendarEvent
                {
                    Class = "PUBLIC",
                    Created = new CalDateTime(DateTime.Now),
                    Start = new CalDateTime(activity.Start),
                    End = new CalDateTime(activity.End),
                    Sequence = 0,
                    Uid = Guid.NewGuid().ToString(),
                    Location = activity.Location
                });
            }

            var calendarSerializer = new CalendarSerializer();
            var ics = calendarSerializer.SerializeToString(calendar);
            

            return View();
        }

    }
}
