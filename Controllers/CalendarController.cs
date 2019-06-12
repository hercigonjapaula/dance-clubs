using DanceClubs.Data;
using DanceClubs.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using DanceClubs.Models;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace DanceClubs.Controllers
{
    [Authorize]
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
            return View();
        }

        public JsonResult GetEvents()
        {
            var userId = _userManager.GetUserId(User);

            var groupUsers = _repository.GetGroupUsersByUserId(userId);
            var activities = groupUsers.SelectMany(i => _repository.GetActivitiesByGroupId(i.GroupId)).ToList();
            var model = new CalendarIndexModel
            {
                Activities = activities.Select(a => new ActivityListingModel
                {
                    Description = a.Group.Name + "\n" + "- " + a.ActivityType.Name + "\n" + "[" + a.Location + "]",
                    ActivityType = a.ActivityType.Name,
                    Author = a.Author.UserName,
                    Start = a.Start,
                    End = a.End,
                    Group = a.Group.Name,
                    Location = a.Location
                }).ToList()
            };

            var events = model.Activities.Select(e => new
            {
                title = e.Description.Replace("\n", Environment.NewLine),                
                start = e.Start.Subtract(new DateTime(1970, 1, 1, 2, 0, 0, DateTimeKind.Utc)).TotalMilliseconds,
                end = e.End.Subtract(new DateTime(1970, 1, 1, 2, 0, 0, DateTimeKind.Utc)).TotalMilliseconds
            });

            return Json(events.ToArray());            
        }
        
        public ActionResult DownloadiCal()
        {
            var userId = _userManager.GetUserId(User);

            var groupUsers = _repository.GetGroupUsersByUserId(userId);
            var activities = groupUsers.SelectMany(i => _repository.GetActivitiesByGroupId(i.GroupId)).ToList();
            StringBuilder sb = new StringBuilder();
            string DateFormat = "yyyyMMddTHHmmssZ";
            string now = DateTime.Now.ToUniversalTime().ToString(DateFormat);
            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("PRODID:");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("METHOD:PUBLISH");
            foreach (var activity in activities)
            {                
                DateTime dtStart = Convert.ToDateTime(activity.Start);
                DateTime dtEnd = Convert.ToDateTime(activity.End);
                sb.AppendLine("BEGIN:VEVENT");
                sb.AppendLine("DTSTART:" + dtStart.ToUniversalTime().ToString(DateFormat));
                sb.AppendLine("DTEND:" + dtEnd.ToUniversalTime().ToString(DateFormat));
                sb.AppendLine("DTSTAMP:" + now);
                sb.AppendLine("UID:" + Guid.NewGuid());
                sb.AppendLine("CREATED:" + now);
                sb.AppendLine("DESCRIPTION:" + activity.Group.Name + " - " + activity.ActivityType.Name);
                sb.AppendLine("LAST-MODIFIED:" + now);
                sb.AppendLine("LOCATION:" + activity.Location);
                sb.AppendLine("SEQUENCE:0");
                sb.AppendLine("STATUS:CONFIRMED");
                sb.AppendLine("SUMMARY:" + activity.Group.Name + " - " + activity.ActivityType.Name);
                sb.AppendLine("TRANSP:OPAQUE");
                sb.AppendLine("END:VEVENT");                
            }
            sb.AppendLine("END:VCALENDAR");
            var calendarBytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(calendarBytes, "text/calendar", "calendar.ics");
        }

    }
}
