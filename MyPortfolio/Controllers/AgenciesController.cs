using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyPortfolio.Data;
using MyPortfolio.Models;
using MyPortfolio.Models.AgenciesViewModel;

namespace MyPortfolio.Controllers
{
    public class AgenciesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AgenciesController(ApplicationDbContext ctx,
                          UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = ctx;
        }
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);


        // GET: Agencies
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Agencies.Include(a => a.Country);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Agencies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var agency = await _context.Agencies
                .Include(a => a.Country)
                .FirstOrDefaultAsync(m => m.AgencyId == id);
            if (agency == null)
            {
                return NotFound();
            }

            return View(agency);
        }

        // GET: Agencies/Create
        public IActionResult Create()
        {
            var model = new NewAgency();
            ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "Name");
            return View(model);
        }

        // POST: Agencies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewAgency newAgency)
        {
            //if (ModelState.IsValid)
            {
                _context.Add(newAgency.Agency);
                await _context.SaveChangesAsync();
                // add data to userAgency table
                UserAgency userAgency = new UserAgency();
                var user = await GetCurrentUserAsync();

                userAgency.AgencyId = newAgency.Agency.AgencyId;
                userAgency.UserId = user.Id;
                userAgency.AccountNo = newAgency.UserAgency.AccountNo;
                _context.Add(userAgency);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "Name", newAgency.Agency.CountryId);
            return View(newAgency);
        }

        // GET: Agencies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var agency = await _context.Agencies.FindAsync(id);
            if (agency == null)
            {
                return NotFound();
            }
            ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "Name", agency.CountryId);
            return View(agency);
        }

        // POST: Agencies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AgencyId,CountryId,Name")] Agency agency)
        {
            if (id != agency.AgencyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(agency);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AgencyExists(agency.AgencyId))
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
            ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "Name", agency.CountryId);
            return View(agency);
        }

        // GET: Agencies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var agency = await _context.Agencies
                .Include(a => a.Country)
                .FirstOrDefaultAsync(m => m.AgencyId == id);
            if (agency == null)
            {
                return NotFound();
            }

            return View(agency);
        }

        // POST: Agencies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var agency = await _context.Agencies.FindAsync(id);
            _context.Agencies.Remove(agency);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AgencyExists(int id)
        {
            return _context.Agencies.Any(e => e.AgencyId == id);
        }
    }
}
