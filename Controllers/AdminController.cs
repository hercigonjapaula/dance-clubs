using DanceClubs.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DanceClubs.Controllers
{
    public class AdminController: Controller
    {
        private readonly IRepository _repository;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(IRepository repository, UserManager<IdentityUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public IActionResult Index()
        {


            return View();
        }
    }
}
