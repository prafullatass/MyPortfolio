using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyPortfolio.Data;
using MyPortfolio.Models;

namespace MyPortfolio.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransactionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Transactions
                                        .Include(t => t.Stock)
                                        .Include(t => t.UserAgency)
                                            .ThenInclude(ua => ua.Agency);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.Stock)
                .Include(t => t.UserAgency)
                .FirstOrDefaultAsync(m => m.TransactionId == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transactions/Create
        public IActionResult Create()
        {
            //make a list of Agency Name and Account no.
            var agencyDetails = _context.UserAgencies.Include(ua => ua.Agency)
                                    .Select(ua => new
                                    {
                                        userAgencyId = ua.UserAgencyId,
                                        desc = string.Format("{0}-- {1}", ua.Agency.Name, ua.AccountNo)
                                    }).ToList();
            //ViewData["StockId"] = new SelectList(_context.Stocks, "StockId", "Name");
            ViewData["UserAgencyId"] = new SelectList(agencyDetails, "userAgencyId", "desc");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TransactionId,StockId,Date,Qty,Rate,Value,BuyOrSell,UserAgencyId")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                _context.Add(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StockId"] = new SelectList(_context.Stocks, "StockId", "Name", transaction.StockId);
            ViewData["UserAgencyId"] = new SelectList(_context.UserAgencies, "UserAgencyId", "UserAgencyId", transaction.UserAgencyId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            ViewData["StockId"] = new SelectList(_context.Stocks, "StockId", "Name", transaction.StockId);
            ViewData["UserAgencyId"] = new SelectList(_context.UserAgencies, "UserAgencyId", "UserAgencyId", transaction.UserAgencyId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TransactionId,StockId,Date,Qty,Rate,Value,BuyOrSell,UserAgencyId")] Transaction transaction)
        {
            if (id != transaction.TransactionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.TransactionId))
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
            ViewData["StockId"] = new SelectList(_context.Stocks, "StockId", "Name", transaction.StockId);
            ViewData["UserAgencyId"] = new SelectList(_context.UserAgencies, "UserAgencyId", "UserAgencyId", transaction.UserAgencyId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.Stock)
                .Include(t => t.UserAgency)
                .FirstOrDefaultAsync(m => m.TransactionId == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.TransactionId == id);
        }
        public List<Stock> GetStocksFromUserAgencyId(int _id)
        {
            UserAgency ua = _context.UserAgencies.Include(s => s.Agency).First(s => s.UserAgencyId == _id);
            //Stock stock = _context.Stocks.Find(ua.Agency.CountryId);
            List<Stock> stocks = _context.Stocks.Where(s => s.CountryId == ua.Agency.CountryId).ToList();
            return stocks;
        }
    }
}

