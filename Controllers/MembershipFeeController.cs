using DanceClubs.Data;
using DanceClubs.Data.Models;
using DanceClubs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DanceClubs.Controllers
{
    //[Authorize(Roles = "User,Vlasnik,Developer")]
    public class MembershipFeeController : Controller
    {
        private readonly IRepository _repository;
        private static UserManager<ApplicationUser> _userManager;

        public MembershipFeeController(IRepository repository,UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var model = new ClubIndexModel();
            model.ClubList = new List<ClubListingModel>();

            var userId = _userManager.GetUserId(User);
            var clubs = _repository.GetClubsByOwnerId(userId);
            foreach(var club in clubs)
            {
                model.ClubList.Add(new ClubListingModel
                {
                    Id = club.Id,
                    Name = club.Name,
                    Users = club.Members.Select(i => new UserListingModel
                    {
                        ClubId = i.ClubId,
                        FirstName = i.ApplicationUser.FirstName,
                        LastName = i.ApplicationUser.LastName
                    }).ToList()
                });
            }
            return View(model);
        }

    }
}
