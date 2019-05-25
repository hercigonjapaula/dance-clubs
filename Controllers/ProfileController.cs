using DanceClubs.Data;
using DanceClubs.Data.Models;
using DanceClubs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DanceClubs.Controllers
{
    [Authorize(Roles = "User")]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository _repository;

        public ProfileController(UserManager<ApplicationUser> userManager, 
            IRepository repository)
        {
            _userManager = userManager;
            _repository = repository;
        }

        public IActionResult Index(string id)
        {
            var userId = _userManager.GetUserId(User);
            var user = _repository.GetApplicationUserById(userId);
            var model = new ProfileModel
            {
                UserId = userId,
                Email = user.Email,
                UserName = user.UserName,
                ProfileImageUrl = user.ProfileImageUrl
            };

            return View(model);
        }        
    }
}
