using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DanceClubs.Models;
using DanceClubs.Data;
using Microsoft.AspNetCore.Identity;
using DanceClubs.Data.Models;

namespace DanceClubs.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository _repository;
        private static UserManager<ApplicationUser> _userManager;

        public HomeController(IRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddNotification()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddNotification(NotificationInput notificationInput)
        {
            var authorId = _userManager.GetUserId(User);
            var author = _repository.GetApplicationUserById(authorId);

            await _repository.AddNotification(new Notification
            {
                Content = notificationInput.Content,
                Published = DateTime.Now,
                Author = author
            });

            foreach (string group in notificationInput.GroupNames)
            {
                
                await _repository.AddNotificationGroup(new NotificationGroup
                {
                    
                });
            }

            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
