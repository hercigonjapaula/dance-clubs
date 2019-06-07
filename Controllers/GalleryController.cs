using DanceClubs.Data;
using DanceClubs.Data.Models;
using DanceClubs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace DanceClubs.Controllers
{
    //[Authorize(Roles = "User")]
    public class GalleryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public GalleryController(ApplicationDbContext context, IRepository repository, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _repository = repository;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);

            var groupUsers = _repository.GetGroupUsersByUserId(userId);
            var images = groupUsers.SelectMany(i => _repository.GetImagesByGroupId(i.GroupId))
                .OrderByDescending(i => i.Published).ToList();
            var model = new ImageIndexModel
            {
                Images = images.Select(i => new ImageListingModel
                {
                    Author = i.Author.UserName,
                    GroupName = i.Group.Name,
                    Published = i.Published,
                    Url = i.ImageUrl
                }).ToList()
            };
            return View(model);
        }
    }
}
