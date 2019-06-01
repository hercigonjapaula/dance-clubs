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
    public class GroupUsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GroupUsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: GroupUsers
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.GroupUsers.Include(g => g.ApplicationUser).Include(g => g.Group).Include(g => g.GroupRole);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: GroupUsers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groupUser = await _context.GroupUsers
                .Include(g => g.ApplicationUser)
                .Include(g => g.Group)
                .Include(g => g.GroupRole)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (groupUser == null)
            {
                return NotFound();
            }

            return View(groupUser);
        }

        // GET: GroupUsers/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "UserName");
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "Name");
            ViewData["GroupRoleId"] = new SelectList(_context.GroupRoles, "Id", "Name");
            return View();
        }

        // POST: GroupUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ApplicationUserId,GroupId,GroupRoleId,MemberTo")] GroupUser groupUser)
        {
            if (ModelState.IsValid)
            {
                groupUser.MemberFrom = DateTime.Now;
                _context.Add(groupUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", groupUser.ApplicationUserId);
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "Id", groupUser.GroupId);
            ViewData["GroupRoleId"] = new SelectList(_context.GroupRoles, "Id", "Id", groupUser.GroupRoleId);
            return View(groupUser);
        }

        // GET: GroupUsers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groupUser = await _context.GroupUsers.FindAsync(id);
            if (groupUser == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", groupUser.ApplicationUserId);
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "Id", groupUser.GroupId);
            ViewData["GroupRoleId"] = new SelectList(_context.GroupRoles, "Id", "Id", groupUser.GroupRoleId);
            return View(groupUser);
        }

        // POST: GroupUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ApplicationUserId,GroupId,GroupRoleId,MemberFrom,MemberTo")] GroupUser groupUser)
        {
            if (id != groupUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(groupUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupUserExists(groupUser.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", groupUser.ApplicationUserId);
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "Id", groupUser.GroupId);
            ViewData["GroupRoleId"] = new SelectList(_context.GroupRoles, "Id", "Id", groupUser.GroupRoleId);
            return View(groupUser);
        }

        // GET: GroupUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groupUser = await _context.GroupUsers
                .Include(g => g.ApplicationUser)
                .Include(g => g.Group)
                .Include(g => g.GroupRole)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (groupUser == null)
            {
                return NotFound();
            }

            return View(groupUser);
        }

        // POST: GroupUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var groupUser = await _context.GroupUsers.FindAsync(id);
            _context.GroupUsers.Remove(groupUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GroupUserExists(int id)
        {
            return _context.GroupUsers.Any(e => e.Id == id);
        }
    }
}
