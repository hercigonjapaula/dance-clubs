using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DanceClubs.Models;
using DanceClubs.Data;
using Microsoft.AspNetCore.Identity;
using DanceClubs.Data.Models;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace DanceClubs.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository _repository;
        private static UserManager<ApplicationUser> _userManager;
        private readonly IServiceProvider serviceProvider;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public HomeController(IServiceProvider serviceProvider, IRepository repository, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _repository = repository;
            _userManager = userManager;
            this.serviceProvider = serviceProvider;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            if (_signInManager.IsSignedIn(User))
            {
                var userId = _userManager.GetUserId(base.User);
                var groupUsers = _repository.GetGroupUsersByUserId(userId);
                var notifications = groupUsers.SelectMany(i => _repository.GetNotificationsByGroupId(i.GroupId)).OrderByDescending(i => i.Published).ToList();
                var model = new NotificationIndexModel
                {
                    NotificationListingModels = new List<NotificationListingModel>()
                };
                foreach (var notif in notifications)
                {
                    var comments = _repository.GetCommentsByNotificationId(notif.Id);
                    model.NotificationListingModels.Add(new NotificationListingModel
                    {
                        Id = notif.Id,
                        Author = notif.Author.UserName,
                        AuthorImage = notif.Author.ProfileImageUrl,
                        Content = notif.Content,
                        ImageUrl = notif.ImageUrl,
                        Published = notif.Published,
                        ClubName = notif.Group.Club.Name,
                        GroupName = notif.Group.Name,
                        CommentListingModels = (comments != null) ? comments.Select(i => new CommentListingModel
                        {
                            Author = i.Author.UserName,
                            AuthorImage = i.Author.ProfileImageUrl,
                            Content = i.Content,
                            Published = i.Published

                        }).ToList() : new List<CommentListingModel>()
                    });
                }
                return View(model);
            }
            else
            {
                return Redirect("/Identity/Account/Login");
            }
                   
        }      

        [HttpPost]
        public async Task<IActionResult> CreateComment(string content, int idNotif)
        {
            var userId = _userManager.GetUserId(User);
            await _repository.AddComment(new Comment
            {
                AuthorId = userId,
                Content = content,
                NotificationId = idNotif,
                Published = DateTime.Now
            });
            return Redirect("/");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
