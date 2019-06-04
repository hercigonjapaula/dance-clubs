using DanceClubs.Data;
using DanceClubs.Data.Models;
using DanceClubs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DanceClubs.Controllers
{
    //[Authorize(Roles = "User,Vlasnik,Developer")]
    public class MembershipFeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository _repository;
        private static UserManager<ApplicationUser> _userManager;

        public MembershipFeeController(ApplicationDbContext context, IRepository repository,UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _repository = repository;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);
            var clubUsers = _repository.GetClubUsersByUserId(userId);
            var membershipFees = clubUsers.SelectMany(u => _repository.GetMembershipFeesByClubUserId(u.Id))
                .OrderBy(m => m.ClubUser.Club.Name).ThenByDescending(m => m.Year)
                .ThenByDescending(m => m.Month).ToList();
            var model = new MembershipFeeIndexModel
            {
                MembershipFees = new List<MembershipFeeListingModel>()
            };
            foreach(var membershipFee in membershipFees)
            {
                model.MembershipFees.Add(new MembershipFeeListingModel
                {
                    Club = membershipFee.ClubUser.Club.Name,
                    User = membershipFee.ClubUser.ApplicationUser.UserName,
                    Amount = membershipFee.Amount.ToString(),
                    Month = membershipFee.Month,
                    Year = membershipFee.Year,
                    PaymentTime = membershipFee.PaymentTime.Date
                });
            }
            return View(model);
        }

        public IActionResult MyClubsFees()
        {
            var userId = _userManager.GetUserId(User);
            var clubOwners = _repository.GetClubOwnersByUserId(userId);
            var clubUsers = clubOwners.SelectMany(o => _repository.GetClubUsersByClubId(o.ClubId))
                .OrderBy(u => u.Club.Name).ThenBy(u => u.ApplicationUser.UserName).ToList();
            var membershipFees = clubUsers.SelectMany(u => _repository.GetMembershipFeesByClubUserId(u.Id))
                .OrderBy(m => m.ClubUser.Club.Name).ThenBy(m => m.ClubUser.ApplicationUser.UserName)
                .ThenByDescending(m => m.Year).ThenByDescending(m => m.Month).ToList();
            var model = new MembershipFeeIndexModel
            {
                MembershipFees = new List<MembershipFeeListingModel>()
            };
            foreach (var membershipFee in membershipFees)
            {
                model.MembershipFees.Add(new MembershipFeeListingModel
                {
                    Club = membershipFee.ClubUser.Club.Name,
                    User = membershipFee.ClubUser.ApplicationUser.UserName,
                    Amount = membershipFee.Amount.ToString(),
                    Month = membershipFee.Month,
                    Year = membershipFee.Year,
                    PaymentTime = membershipFee.PaymentTime.Date
                });
            }
            return View(model);
        }

        // GET: MembershipFee/Create
        public IActionResult Create()

        {
            var userId = _userManager.GetUserId(User);
            var clubOwners = _repository.GetClubOwnersByUserId(userId);            
            var clubUsers = clubOwners.SelectMany(o => _repository.GetClubUsersByClubId(o.ClubId))
                .OrderBy(u => u.Club.Name).ThenBy(u => u.ApplicationUser.UserName).ToList();
            //var users = clubUsers.Select(u => _repository.GetApplicationUserById(u.ApplicationUserId)).ToList();
            List<object> users = new List<object>();
            foreach(var user in clubUsers)
            {
                users.Add(new
                {
                    ClubUserId = user.Id,
                    Name = user.ApplicationUser.UserName
                });
            }            
            ViewData["ClubUserId"] = new SelectList(users, "ClubUserId", "Name");            
            return View();
        }

        // POST: Activities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClubUserId,Month,Year,PaymentTime,Amount")] MembershipFee membershipFee)
        {
            membershipFee.ClubUser = _repository.GetClubUserById(membershipFee.ClubUserId);
            if (ModelState.IsValid)
            {                
                _context.Add(membershipFee);
                await _context.SaveChangesAsync();               
                return RedirectToAction(nameof(MyClubsFees));
            }
            var userId = _userManager.GetUserId(User);
            var clubOwners = _repository.GetClubOwnersByUserId(userId);
            var clubUsers = clubOwners.SelectMany(o => _repository.GetClubUsersByClubId(o.ClubId))
                .OrderBy(u => u.Club.Name).ThenBy(u => u.ApplicationUser.UserName).ToList();
            //var users = clubUsers.Select(u => _repository.GetApplicationUserById(u.ApplicationUserId)).ToList();
            List<object> users = new List<object>();
            foreach (var user in clubUsers)
            {
                users.Add(new
                {
                    ClubUserId = user.Id,
                    Name = user.ApplicationUser.UserName
                });
            }
            ViewData["ClubUserId"] = new SelectList(users, "ClubUserId", "Name");            
            return View(membershipFee);
        }

    }
}
