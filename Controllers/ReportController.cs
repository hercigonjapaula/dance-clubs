using DanceClubs.Data;
using DanceClubs.Data.Models;
using DanceClubs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

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
            var userId = _userManager.GetUserId(User);
            var clubs = _repository.GetClubsByOwnerId(userId);
            Array categoryValues = Enum.GetValues(typeof(Category));
            Array categoryNames = Enum.GetNames(typeof(Category));
            ViewData["ClubId"] = new SelectList(clubs, "Id", "Name");
            ViewData["ReportCategory"] = new SelectList(categoryValues);            
            return View();
        }
    }
}
