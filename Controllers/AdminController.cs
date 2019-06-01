using DanceClubs.Data;
using DanceClubs.Data.Models;
using DanceClubs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DanceClubs.Controllers
{
    [Authorize(Roles = "ADMIN,Admin,admin")]
    public class AdminController: Controller
    {
        private readonly IRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(IRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var clubs = _repository.GetAllClubs()
                .Select(club => new ClubListingModel
                {
                    Id = club.Id,
                    Name = club.Name
                });

            var model = new ClubIndexModel
            {
                ClubList = clubs.ToList()
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

                await _repository.AddClubUser(new ClubUser
                {
                    ClubId = club.Id,
                    ApplicationUserId = owner.Id,
                    Club = club,
                    ApplicationUser = owner,
                    MemberFrom = DateTime.Now                    
                });
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult ClubInfo()
        {
            return View();
        }
    }
}
