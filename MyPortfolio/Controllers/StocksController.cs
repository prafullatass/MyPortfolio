using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChartJS.Helpers.MVC;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyPortfolio.Data;
using MyPortfolio.Models;
using MyPortfolio.Models.StocksViewModel;

namespace MyPortfolio.Controllers
{
    public class StocksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StocksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Stocks
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Stocks
                                                .Include(s => s.Country)
                                                .Include(s => s.Sector);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Stocks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stock = await _context.Stocks
                .Include(s => s.Country)
                .Include(s => s.Sector)
                .Include(s => s.Transactions)
                .FirstOrDefaultAsync(m => m.StockId == id);

            if (stock == null)
            {
                return NotFound();
            }

            return View(stock);
        }

        // GET: Stocks/Create
        public IActionResult Create()
        {
            ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "Name");
            ViewData["SectorId"] = new SelectList(_context.Sectors, "SectorId", "Name");
            return View();
        }

        // POST: Stocks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StockId,CountryId,Name,Ticker,SectorId")] Stock stock)
        {
            if (ModelState.IsValid)
            {
                _context.Add(stock);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "Name", stock.CountryId);
            ViewData["SectorId"] = new SelectList(_context.Sectors, "SectorId", "Name", stock.SectorId);
            return View(stock);
        }

        // GET: Stocks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stock = await _context.Stocks.FindAsync(id);
            if (stock == null)
            {
                return NotFound();
            }
            ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "Name", stock.CountryId);
            ViewData["SectorId"] = new SelectList(_context.Sectors, "SectorId", "Name", stock.SectorId);
            return View(stock);
        }

        // POST: Stocks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StockId,CountryId,Name,Ticker,SectorId")] Stock stock)
        {
            if (id != stock.StockId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(stock);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StockExists(stock.StockId))
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
            ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "Name", stock.CountryId);
            ViewData["SectorId"] = new SelectList(_context.Sectors, "SectorId", "Name", stock.SectorId);
            return View(stock);
        }

        // GET: Stocks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stock = await _context.Stocks
                .Include(s => s.Country)
                .Include(s => s.Sector)
                .FirstOrDefaultAsync(m => m.StockId == id);
            
            if (stock == null)
            {
                return NotFound();
            }

            return View(stock);
        }

        // POST: Stocks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stock = await _context.Stocks.FindAsync(id);
            _context.Stocks.Remove(stock);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StockExists(int id)
        {
            return _context.Stocks.Any(e => e.StockId == id);
        }

        //calculate avarateRate & total Quantity for a stock
        public Stock CaluculateTotals(Stock st)
        {
            foreach (Transaction t in st.Transactions)
            {
                if (t.BuyOrSell)
                {
                    double total = st.TotalQty * st.AvarageRate;
                    total = total + (t.Qty * t.Rate);
                    st.TotalQty += t.Qty;
                    st.AvarageRate = Math.Round((total / st.TotalQty) * 100) / 100;
                }
                else
                {
                    st.TotalQty -= t.Qty;
                }
            }
            return st;
        }
    }
}