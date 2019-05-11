using DanceClubs.Data;
using DanceClubs.Data.Models;
using DanceClubs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace DanceClubs.Controllers
{
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
            var clubs = _repository.GetAllClubs()
                .Select(club => new ClubListingModel {
                    Id = club.Id,
                    Name = club.Name
            });

            var model = new ClubIndexModel
            {
                ClubList = clubs
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult AddNewClub()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddNewClub(ClubInput clubInput)
        {
            var club = new Club
            {
                Name = clubInput.Name,
                Address = clubInput.Address,
            };

            await _repository.AddClub(club);

            foreach (string email in clubInput.OwnersEmails)
            {
                var owner = await _userManager.FindByEmailAsync(email);
                await _repository.AddClubOwner(new ClubOwner
                {
                    Club = club,
                    Owner = owner,
                    ClubId = club.Id,
                    OwnerId = owner.Id
                });
            }

            return RedirectToAction("Index");
        }
    }
}
