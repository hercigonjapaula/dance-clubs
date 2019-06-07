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

namespace DanceClubs.Controllers
{
    public class ActivitiesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository _repository;
        private readonly IEmailService _EmailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAppEmailService _appEmailService;

        public ActivitiesController(ApplicationDbContext context, IRepository repository, IEmailService emailService, UserManager<ApplicationUser> userManager, IAppEmailService appEmailService)
        {
            _repository = repository;
            _context = context;
            _EmailService = emailService;
            _userManager = userManager;
            _appEmailService = appEmailService;
        }

        // GET: Activities
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Activities.Include(m => m.ActivityType).Include(g => g.Group); 
            return View(await _context.Activities.ToListAsync());
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
                .FirstOrDefaultAsync(m => m.Id == id);
            if (activity == null)
            {
                return NotFound();
            }

            return View(activity);
        }

        // GET: Activities/Create
        public IActionResult Create()

        {
            ViewData["ActivityTypeId"] = new SelectList(_context.ActivityTypes, "Id", "Name");
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "Name");       
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
                        Body = "DanceClubsApp - stvorena nova aktivnost",
                        Attachment = ms,
                        FileName = "calendar.ics"
                    };
                    await _appEmailService.SendAsync(emailRequest);                    
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
    }
}
