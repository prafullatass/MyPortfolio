using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyPortfolio.Data;
using MyPortfolio.Models;
using MyPortfolio.Models.AgenciesViewModel;

namespace MyPortfolio.Controllers
{
    public class UserAgenciesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserAgenciesController(ApplicationDbContext ctx,
                          UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = ctx;
        }
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        [Authorize]
        // GET: UserAgencies
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            var applicationDbContext = _context.UserAgencies
                                        .Include(u => u.Agency)
                                        .Include(u => u.User)
                                        .Where(u => u.UserId == user.Id);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: UserAgencies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAgency = await _context.UserAgencies
                .Include(u => u.Agency)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.UserAgencyId == id);
            if (userAgency == null)
            {
                return NotFound();
            }

            return View(userAgency);
        }

        // GET: UserAgencies/Create
        public IActionResult Create()
        {
            ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "Name");
            return View();
        }

        // POST: UserAgencies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewAgency newAgency)
        {
            UserAgency userAgency = newAgency.UserAgency;
            if (newAgency.Agency.AgencyId == 0)
            {
                ViewBag.Message = string.Format("Please select an Agency !");

                ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "Name");
                return View(newAgency);
            }
            var user = await GetCurrentUserAsync();
            userAgency.AgencyId = newAgency.Agency.AgencyId;
            userAgency.UserId = user.Id;
            ModelState.Remove("UserAgency.UserId");
            ModelState.Remove("Agency.Name");
            if (ModelState.IsValid)
            {
                _context.Add(userAgency);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "Name");
            return View(newAgency);
        }

        // GET: UserAgencies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAgency = await _context.UserAgencies.FindAsync(id);
            if (userAgency == null)
            {
                return NotFound();
            }
            ViewData["AgencyId"] = new SelectList(_context.Agencies, "AgencyId", "Name", userAgency.AgencyId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", userAgency.UserId);
            return View(userAgency);
        }

        // POST: UserAgencies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserAgencyId,AgencyId,UserId,AccountNo")] UserAgency userAgency)
        {
            if (id != userAgency.UserAgencyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userAgency);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserAgencyExists(userAgency.UserAgencyId))
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
            ViewData["AgencyId"] = new SelectList(_context.Agencies, "AgencyId", "Name", userAgency.AgencyId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", userAgency.UserId);
            return View(userAgency);
        }

        // GET: UserAgencies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAgency = await _context.UserAgencies
                .Include(u => u.Agency)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.UserAgencyId == id);
            if (userAgency == null)
            {
                return NotFound();
            }

            return View(userAgency);
        }

        // POST: UserAgencies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userAgency = await _context.UserAgencies.FindAsync(id);
            _context.UserAgencies.Remove(userAgency);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserAgencyExists(int id)
        {
            return _context.UserAgencies.Any(e => e.UserAgencyId == id);
        }

        public List<Agency> GetCountrysAgencies (int _id)
        {
            List<Agency> agencies = _context.Agencies.Where(s => s.CountryId == _id).ToList();
            return agencies;
        }
    }
}
