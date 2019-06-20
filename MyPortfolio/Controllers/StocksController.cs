using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChartJS.Helpers.MVC;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;

        public StocksController(ApplicationDbContext ctx,
                          UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = ctx;
        }
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

       
        // GET: Stocks
        [Authorize]
        public async Task<IActionResult> Index(string _orderBy, string _sortDirection)
        {
            List<Stock> applicationDbContext = await _context.Stocks
                                                .Include(s => s.Country)
                                                .Include(s => s.Sector)
                                                .ToListAsync();
        if (_orderBy != null)
            {
                switch (_orderBy)
                {
                    case "Name":
                        if (_sortDirection == null || _sortDirection == "asc")
                        {
                            applicationDbContext  = applicationDbContext.OrderBy(s => s.Name).ToList();
                            ViewData["NameDirection"] = "desc";
                        }
                        else
                        {
                            applicationDbContext = applicationDbContext.OrderByDescending(s => s.Name).ToList();
                            ViewData["NameDirection"] = "asc";
                        }
                        break;
                        case "Country":
                        if (_sortDirection == null || _sortDirection == "asc")
                        {
                            applicationDbContext = applicationDbContext.OrderBy(s => s.Country.Name).ToList();
                            ViewData["CountryDirection"] = "desc";
                        }
                        else
                        {
                            applicationDbContext = applicationDbContext.OrderByDescending(s => s.Country.Name).ToList();
                            ViewData["CountryDirection"] = "asc";
                        }
                        break;
                        case "Sector":
                        if (_sortDirection == null || _sortDirection == "asc")
                        {
                            applicationDbContext = applicationDbContext.OrderBy(s => s.Sector.Name).ToList();
                            ViewData["SectorDirection"] = "desc";
                        }
                        else
                        {
                            applicationDbContext = applicationDbContext.OrderByDescending(s => s.Sector.Name).ToList();
                            ViewData["SectorDirection"] = "asc";
                        }
                        break;
                }
            }
            return View(applicationDbContext);
        }


        // GET: Stocks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await GetCurrentUserAsync();
            var stock = await _context.Stocks
                .Include(s => s.Country)
                .Include(s => s.Sector)
                .Include(s => s.Transactions)
                    .ThenInclude(t => t.UserAgency)
                .FirstOrDefaultAsync(m => m.StockId == id);

            stock.Transactions = stock.Transactions.Where(t => t.UserAgency.UserId == user.Id).ToList();
            //calculate stocks avg rate and total qty
            stock = CaluculateTotals(stock);

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
                stock.Name = char.ToUpper(stock.Name[0]) + stock.Name.Substring(1);
                stock.Ticker = stock.Ticker.ToUpper();
                Stock st = _context.Stocks
                                    .Where(s => (s.CountryId == stock.CountryId) &&
                                    (s.Name == stock.Name || s.Ticker == stock.Ticker))
                                    .FirstOrDefault();
                if(st == null)
                {
                    _context.Add(stock);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Message = "This Stock Already entered!!";
                }
            }
            else
            {
                if(stock.CountryId == 0 )
                    ViewBag.Message = "Select a Country from dropdown list";
                else
                if (stock.SectorId == 0)
                    ViewBag.Message = "Select a Sector from dropdown list";

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