using DanceClubs.Data;
using DanceClubs.Data.Models;
using DanceClubs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DanceClubs.Controllers
{
    [Authorize]
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
            return Redirect("/identity/Account/Manage");
            //var userId = _userManager.GetUserId(User);
            //var user = _repository.GetApplicationUserById(userId);
            //var model = new ProfileModel
            //{
            //    UserId = userId,
            //    Email = user.Email,
            //    UserName = user.UserName,
            //    ProfileImageUrl = user.ProfileImageUrl
            //};

            //return View(model);
        } 
        
        public async Task<IActionResult> Update(ProfileModel model)
        {
            var userId = _userManager.GetUserId(User);
            var user = _repository.GetApplicationUserById(userId);
            user.Email = model.Email;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetPasswordAsync(user, token, model.Password);

            return Redirect("Index");
        }
    }
}
