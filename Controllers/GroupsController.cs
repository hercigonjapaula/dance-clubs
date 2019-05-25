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
    public class GroupsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GroupsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Groups
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Groups.Include(m => m.AgeGroup).Include(g => g.Club).Include(g => g.DanceStyle);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Groups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ggroup = await _context.Groups
                .Include(g => g.AgeGroup)
                .Include(g => g.Club)
                .Include(g => g.DanceStyle)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ggroup == null)
            {
                return NotFound();
            }

            return View(ggroup);
        }

        // GET: Groups/Create
        public IActionResult Create()
        {
            ViewData["AgeGroupId"] = new SelectList(_context.AgeGroups, "Id", "Name");
            ViewData["ClubId"] = new SelectList(_context.Clubs, "Id", "Name");
            ViewData["DanceStyleId"] = new SelectList(_context.DanceStyles, "Id", "Name");
            return View();
        }

        // POST: Groups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ClubId,DanceStyleId,AgeGroupId")] Group ggroup)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ggroup);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AgeGroupId"] = new SelectList(_context.AgeGroups, "Id", "Id", ggroup.AgeGroupId);
            ViewData["ClubId"] = new SelectList(_context.Clubs, "Id", "Id", ggroup.ClubId);
            ViewData["DanceStyleId"] = new SelectList(_context.DanceStyles, "Id", "Id", ggroup.DanceStyleId);
            return View(ggroup);
        }

        // GET: Groups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ggroup = await _context.Groups.FindAsync(id);
            if (ggroup == null)
            {
                return NotFound();
            }
            ViewData["AgeGroupId"] = new SelectList(_context.AgeGroups, "Id", "Id", ggroup.AgeGroupId);
            ViewData["ClubId"] = new SelectList(_context.Clubs, "Id", "Id", ggroup.ClubId);
            ViewData["DanceStyleId"] = new SelectList(_context.DanceStyles, "Id", "Id", ggroup.DanceStyleId);
            return View(ggroup);
        }

        // POST: Groups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ClubId,DanceStyleId,AgeGroupId")] Group ggroup)
        {
            if (id != ggroup.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ggroup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupExists(ggroup.Id))
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
            ViewData["AgeGroupId"] = new SelectList(_context.AgeGroups, "Id", "Id", ggroup.AgeGroupId);
            ViewData["ClubId"] = new SelectList(_context.Clubs, "Id", "Id", ggroup.ClubId);
            ViewData["DanceStyleId"] = new SelectList(_context.DanceStyles, "Id", "Id", ggroup.DanceStyleId);
            return View(ggroup);
        }

        // GET: Groups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ggroup = await _context.Groups
                .Include(g => g.AgeGroup)
                .Include(g => g.Club)
                .Include(g => g.DanceStyle)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ggroup == null)
            {
                return NotFound();
            }

            return View(ggroup);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ggroup = await _context.Groups.FindAsync(id);
            _context.Groups.Remove(ggroup);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GroupExists(int id)
        {
            return _context.Groups.Any(e => e.Id == id);
        }
    }
}
