using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DanceClubs.Data;
using DanceClubs.Data.Models;

namespace DanceClubs.Controllers
{
    public class ClubUsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClubUsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ClubUsers
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ClubUsers.Include(c => c.ApplicationUser).Include(c => c.Club);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ClubUsers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clubUser = await _context.ClubUsers
                .Include(c => c.ApplicationUser)
                .Include(c => c.Club)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (clubUser == null)
            {
                return NotFound();
            }

            return View(clubUser);
        }

        // GET: ClubUsers/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "UserName");
            ViewData["ClubId"] = new SelectList(_context.Clubs, "Id", "Name");
            return View();
        }

        // POST: ClubUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ApplicationUserId,ClubId,MemberTo")] ClubUser clubUser)
        {
            if (ModelState.IsValid)
            {                
                clubUser.MemberFrom = DateTime.Now;
                _context.Add(clubUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", clubUser.ApplicationUserId);
            ViewData["ClubId"] = new SelectList(_context.Clubs, "Id", "Id", clubUser.ClubId);
            return View(clubUser);
        }

        // GET: ClubUsers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clubUser = await _context.ClubUsers.FindAsync(id);
            if (clubUser == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", clubUser.ApplicationUserId);
            ViewData["ClubId"] = new SelectList(_context.Clubs, "Id", "Id", clubUser.ClubId);
            return View(clubUser);
        }

        // POST: ClubUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ApplicationUserId,ClubId,MemberFrom,MemberTo")] ClubUser clubUser)
        {
            if (id != clubUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(clubUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClubUserExists(clubUser.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", clubUser.ApplicationUserId);
            ViewData["ClubId"] = new SelectList(_context.Clubs, "Id", "Id", clubUser.ClubId);
            return View(clubUser);
        }

        // GET: ClubUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clubUser = await _context.ClubUsers
                .Include(c => c.ApplicationUser)
                .Include(c => c.Club)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (clubUser == null)
            {
                return NotFound();
            }

            return View(clubUser);
        }

        // POST: ClubUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var clubUser = await _context.ClubUsers.FindAsync(id);
            _context.ClubUsers.Remove(clubUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClubUserExists(int id)
        {
            return _context.ClubUsers.Any(e => e.Id == id);
        }
    }
}
