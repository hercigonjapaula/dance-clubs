using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DanceClubs.Data;
using DanceClubs.Data.Models;
using NETCore.MailKit.Core;
using Microsoft.AspNetCore.Identity;
using System.Text;
using System.IO;
using DanceClubs.Services;
using DanceClubs.Models;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace DanceClubs.Controllers
{
    [Authorize]
    public class ActivitiesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository _repository;
        private readonly IEmailService _EmailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAppEmailService _appEmailService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ActivitiesController(ApplicationDbContext context, IRepository repository, IEmailService emailService, 
            UserManager<ApplicationUser> userManager, IAppEmailService appEmailService, SignInManager<ApplicationUser> signInManager)
        {
            _repository = repository;
            _context = context;
            _EmailService = emailService;
            _userManager = userManager;
            _appEmailService = appEmailService;
            _signInManager = signInManager;
        }

        // GET: Activities
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Activities.Include(m => m.ActivityType).Include(g => g.Group).Include(g => g.Author); 
            return View(await _context.Activities.Include(m => m.ActivityType).Include(g => g.Group)
                .Include(g => g.Author).OrderByDescending(g => g.Start).OrderByDescending(g => g.End).ToListAsync());
        }

        // GET: Activities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await _context.Activities
                .Include(g => g.ActivityType)
                .Include(g => g.Group)
                .Include(g => g.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (activity == null)
            {
                return NotFound();
            }

            var groupUsers = _repository.GetGroupUsersByGroupId(activity.GroupId);
            var users = groupUsers.Select(gu => _repository.GetApplicationUserById(gu.ApplicationUserId));

            var activityUsers = groupUsers.Select(u => _repository.GetUserActivityByGroupUserIdActivityId(u.Id, activity.Id));

            var members = new List<UserActivityDetail>();
            foreach(var member in groupUsers)
            {
                var attendance = "";
                var activityUser = activityUsers.Where(au => au.GroupUserId == member.Id).First();
                if (activityUser.AttendanceOnHold)
                {
                    attendance = "Na čekanju";
                }
                else
                {
                    if(activityUser.Attendance)
                    {
                        attendance = "Potvrđen";
                    }
                    else
                    {
                        attendance = "Odbijen";
                    }                    
                }
                members.Add(new UserActivityDetail
                {
                    UserName = member.ApplicationUser.UserName,
                    Name = member.ApplicationUser.FirstName,
                    Surname = member.ApplicationUser.LastName,
                    Attendance = attendance
                });
            }

            var model = new ActivityDetailModel
            {
                ActivityType = activity.ActivityType.Name,
                Group = activity.Group.Name,
                Author = activity.Author.UserName,
                Start = activity.Start,
                End = activity.End,
                Location = activity.Location,
                Members =members
            };

            return View(model);
        }

        // GET: Activities/Create
        public IActionResult Create()

        {
            var groups = _repository.GetGroupsByDanceTeacherId(_userManager.GetUserId(User));
            ViewData["ActivityTypeId"] = new SelectList(_context.ActivityTypes, "Id", "Name");
            ViewData["GroupId"] = new SelectList(groups, "Id", "Name");       
            return View();           
        }

        // POST: Activities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Start,End,Location,ActivityTypeId,GroupId")] Activity activity)
        {
            if (ModelState.IsValid)
            {
                activity.Group = _repository.GetGroupById(activity.GroupId);
                activity.ActivityType = _repository.GetActivityTypeById(activity.ActivityTypeId);
                activity.AuthorId = _userManager.GetUserId(User);
                _context.Add(activity);
                await _context.SaveChangesAsync();
                
                var mailAddresses = _repository.GetGroupUsersByGroupId(activity.GroupId)
                    .Select(u => u.ApplicationUser.Email).ToList();

                StringBuilder sb = new StringBuilder();
                string DateFormat = "yyyyMMddTHHmmssZ";
                string now = DateTime.Now.ToUniversalTime().ToString(DateFormat);
                sb.AppendLine("BEGIN:VCALENDAR");
                sb.AppendLine("PRODID:");
                sb.AppendLine("VERSION:2.0");
                sb.AppendLine("METHOD:PUBLISH");                
                DateTime dtStart = Convert.ToDateTime(activity.Start);
                DateTime dtEnd = Convert.ToDateTime(activity.End);
                sb.AppendLine("BEGIN:VEVENT");
                sb.AppendLine("DTSTART:" + dtStart.ToUniversalTime().ToString(DateFormat));
                sb.AppendLine("DTEND:" + dtEnd.ToUniversalTime().ToString(DateFormat));
                sb.AppendLine("DTSTAMP:" + now);
                sb.AppendLine("UID:" + Guid.NewGuid());
                sb.AppendLine("CREATED:" + now);
                sb.AppendLine("DESCRIPTION:" + activity.Group.Name+ " - " + activity.ActivityType.Name);                
                sb.AppendLine("LAST-MODIFIED:" + now);
                sb.AppendLine("LOCATION:" + activity.Location);
                sb.AppendLine("SEQUENCE:0");
                sb.AppendLine("STATUS:CONFIRMED");
                sb.AppendLine("SUMMARY:" + activity.Group.Name + " - " + activity.ActivityType.Name);
                sb.AppendLine("TRANSP:OPAQUE");
                sb.AppendLine("END:VEVENT");                
                sb.AppendLine("END:VCALENDAR");
                var calendarBytes = Encoding.UTF8.GetBytes(sb.ToString());
                MemoryStream ms = new MemoryStream(calendarBytes);

                foreach(var mail in mailAddresses)
                {
                    var emailRequest = new EmailRequest{
                        ToAddress = mail,
                        Subject = "Nova aktivnost",
                        Body = "Potvrdite dolazak na aktivnost klikom na link " + "https://localhost:44379/Activities/Confirm/" + activity.Id 
                        + " ili javite nedolazak klikom na link " + "https://localhost:44379/Activities/Decline/" + activity.Id,
                        Attachment = ms,
                        FileName = "calendar.ics"
                    };
                    await _appEmailService.SendAsync(emailRequest);                    
                }

                var groupUsers = _repository.GetGroupUsersByGroupId(activity.GroupId);

                foreach(var groupUser in groupUsers)
                {
                    await _repository.AddUserActivity(new UserActivity
                        {
                            GroupUserId = groupUser.Id,
                            ActivityId = activity.Id,
                            Activity = activity,
                            GroupUser = groupUser,
                            AttendanceOnHold = true
                        });
                }
                

                return Redirect("/Calendar");
            }
            ViewData["ActivityTypeId"] = new SelectList(_context.ActivityTypes, "Id", "Id", activity.ActivityTypeId);
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "Id", activity.GroupId);    
            return View(activity);
        }

        // GET: Activities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await _context.Activities.FindAsync(id);
            if (activity == null)
            {
                return NotFound();
            }
            ViewData["ActivityTypeId"] = new SelectList(_context.ActivityTypes, "Id", "Id", activity.ActivityTypeId);
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "Id", activity.GroupId);
            return View(activity);
        }

        // POST: Activities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Start,End,Location")] Activity activity)
        {
            if (id != activity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(activity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActivityExists(activity.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ActivityTypeId"] = new SelectList(_context.ActivityTypes, "Id", "Id", activity.ActivityTypeId);
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "Id", activity.GroupId);
            return View(activity);
        }

        // GET: Activities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await _context.Activities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (activity == null)
            {
                return NotFound();
            }

            return View(activity);
        }

        // POST: Activities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var activity = await _context.Activities.FindAsync(id);
            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActivityExists(int id)
        {
            return _context.Activities.Any(e => e.Id == id);
        }

        [Route("/Activities/Confirm/{activityid}")]
        public IActionResult Confirm(int activityid)
        {            
            var userId = _userManager.GetUserId(User);
            var activity = _repository.GetActivityById(activityid);
            var groupId = activity.GroupId;
            var groupUser = _repository.GetGroupUserByUserIdGroupId(userId, groupId);
            var userActivity = _repository.GetUserActivityByGroupUserIdActivityId(groupUser.Id, activityid);

            _repository.SetAttendanceToTrue(userActivity);           

            var model = new ActivityListingModel
            {
                Description = "Potvrdili ste dolazak na aktivnost!",
                Group = activity.Group.Name,
                ActivityType = activity.ActivityType.Name,
                Author = activity.Author.UserName,
                Start = activity.Start,
                End = activity.End,
                Location = activity.Location
            };
            return View(model);           
        }

        [Route("/Activities/Decline/{activityid}")]
        public IActionResult Decline(int activityid)
        {
            var userId = _userManager.GetUserId(User);
            var activity = _repository.GetActivityById(activityid);
            var groupId = activity.GroupId;
            var groupUser = _repository.GetGroupUserByUserIdGroupId(userId, groupId);
            var userActivity = _repository.GetUserActivityByGroupUserIdActivityId(groupUser.Id, activityid);

            _repository.SetAttendanceToFalse(userActivity);          

            var model = new ActivityListingModel
            {
                Description = "Javili ste nedolazak na aktivnost!",
                Group = activity.Group.Name,
                ActivityType = activity.ActivityType.Name,
                Author = activity.Author.UserName,
                Start = activity.Start,
                End = activity.End,
                Location = activity.Location
            };
            return View(model);
        }
    }
}
