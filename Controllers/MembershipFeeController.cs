using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DanceClubs.Controllers
{
    [Authorize(Roles = "User")]
    public class MembershipFeeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
