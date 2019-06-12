using DanceClubs.Data;
using DanceClubs.Data.Models;
using DanceClubs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace DanceClubs.Controllers
{
    [Authorize]
    public class ClubController: Controller
    {
        private readonly IRepository _repository;
        private static UserManager<ApplicationUser> _userManager;

        public ClubController(IRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var clubs = _repository.GetClubsByUserId(_userManager.GetUserId(User))                  
                .Select(c => new ClubListingModel
                 {
                     Id = c.Id,
                     Name = c.Name
                 });

            var model = new ClubIndexModel
            {
                ClubList = clubs.ToList()
            };

            return View(model);
        }
    }
}
