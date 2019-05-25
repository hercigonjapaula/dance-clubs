using DanceClubs.Data;
using DanceClubs.Data.Models;
using DanceClubs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace DanceClubs.Controllers
{
    //[Authorize(Roles = "User")]
    public class CalendarController : Controller
    {
        private readonly IRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CalendarController(IRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;       
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddNewActivity()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddNewActivity(ActivityInput activityInput)
        {
            var activity = new Activity
            {
                
            };

            await _repository.AddActivity(activity);

            return RedirectToAction("Index");
        }
    }
}
