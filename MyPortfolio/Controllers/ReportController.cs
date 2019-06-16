using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyPortfolio.Data;
using MyPortfolio.Models;
using MyPortfolio.Models.StocksViewModel;

namespace MyPortfolio.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReportController(ApplicationDbContext ctx,
                          UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = ctx;
        }
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> AllStockReport()
        {
            ListOfStocks listOfStock = new ListOfStocks();
            listOfStock.Stocks = await _context.Stocks.Include(s => s.Country)
                                        .Include(s => s.Sector)
                                        .Include(s => s.Transactions)
                                        .Distinct().ToListAsync();

            foreach (Stock st in listOfStock.Stocks)
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
            }
            return View(listOfStock);
        }

        [Authorize]
        public async Task<IActionResult> ProfitReport()
        {
            ListOfStocks listOfStock = new ListOfStocks();
            listOfStock.Stocks = await _context.Stocks.Include(s => s.Country)
                                        .Include(s => s.Sector)
                                        .Include(s => s.Transactions)
                                        .Distinct().ToListAsync();

            foreach (Stock st in listOfStock.Stocks)
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
                        st.Profit = st.Profit + (t.Qty * st.AvarageRate);
                        st.TotalQty -= t.Qty;
                    }
                }
            }
            return View(listOfStock);
        }

        [Authorize]
        public async Task<IActionResult> SectorReport()
        {
            ListOfStocks listOfStock = new ListOfStocks();
            listOfStock.Stocks = await _context.Stocks.Include(s => s.Country)
                                        .Include(s => s.Sector)
                                        .Include(s => s.Transactions)
                                        .Distinct().ToListAsync();

            foreach (Stock st in listOfStock.Stocks)
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
                        st.Profit = st.Profit + (t.Qty * st.AvarageRate);
                        st.TotalQty -= t.Qty;
                    }
                }
            }
            return View(listOfStock);
        }

        public async Task<IActionResult> ChartReport()
        {
            List<double> ValueArray = new List<double>();
            List<string> NameArray = new List<string>();
            List<List<object>> Arr = new List<List<object>>();

            ListOfStocks listOfStock = new ListOfStocks();
            listOfStock.Stocks = await _context.Stocks.Include(s => s.Country)
                                        .Include(s => s.Sector)
                                        .Include(s => s.Transactions)
                                        .Distinct().ToListAsync();
            int i = 0;
            foreach (Stock st in listOfStock.Stocks)
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
                ValueArray.Add(st.AvarageRate * st.TotalQty);
                NameArray.Add(st.Ticker);
                //Arr[i] = [{v = st.Ticker;
                // Arr[i] = [new { v= (st.AvarageRate * st.TotalQty)}];
            }
            ViewBag.ValueArray = ValueArray;
            ViewBag.NameArray = NameArray;
            ViewBag.arr = Arr;
            return View();
        }

    }
}
