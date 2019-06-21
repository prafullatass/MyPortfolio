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
using MyPortfolio.Models.StocksViewModel;
using Newtonsoft.Json;

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
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> AllStockReport(string _orderBy, string _sortDirection)
        {
            var user = await GetCurrentUserAsync();
            ListOfStocks listOfStock = new ListOfStocks();
            listOfStock.Stocks = await _context.Stocks.Include(s => s.Country)
                                        .Include(s => s.Sector)
                                        .Include(s => s.Transactions)
                                            .ThenInclude(t => t.UserAgency)
                                        .ToListAsync();

            if (_orderBy != null)
            {
                switch (_orderBy)
                {
                    case "Name":
                        if (_sortDirection == null || _sortDirection == "asc")
                        {
                            listOfStock.Stocks = listOfStock.Stocks.OrderBy(s => s.Name).ToList();
                            ViewData["NameDirection"] = "desc";
                        }
                        else
                        {
                            listOfStock.Stocks = listOfStock.Stocks.OrderByDescending(s => s.Name).ToList();
                            ViewData["NameDirection"] = "asc";
                        }
                        break;
                    case "Country":
                        if (_sortDirection == null || _sortDirection == "asc")
                        {
                            listOfStock.Stocks = listOfStock.Stocks.OrderBy(s => s.Country.Name).ToList();
                            ViewData["CountryDirection"] = "desc";
                        }
                        else
                        {
                            listOfStock.Stocks = listOfStock.Stocks.OrderByDescending(s => s.Country.Name).ToList();
                            ViewData["CountryDirection"] = "asc";
                        }
                        break;
                    case "Sector":
                        if (_sortDirection == null || _sortDirection == "asc")
                        {
                            listOfStock.Stocks = listOfStock.Stocks.OrderBy(s => s.Sector.Name).ToList();
                            ViewData["SectorDirection"] = "desc";
                        }
                        else
                        {
                            listOfStock.Stocks = listOfStock.Stocks.OrderByDescending(s => s.Sector.Name).ToList();
                            ViewData["SectorDirection"] = "asc";
                        }
                        break;
                }
            }

            //select transactions of the logged in user only
            foreach (Stock st in listOfStock.Stocks)
            {
                st.Transactions = st.Transactions.Where(t => t.UserAgency.UserId == user.Id).ToList();
            }
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
            listOfStock.TotalValues = listOfStock.Stocks
                                      .GroupBy(s => s.Country.Name)
                                      .Select(group => new TotalValue
                                      {
                                          CountryName = group.Key,
                                          Currnecy = group.Select(item => item.Country.Currency).First().ToString(),
                                          Total = group.Sum(item => item.TotalQty * item.AvarageRate)
                                      }).ToList();
                                      
            return View(listOfStock);
        }

        [Authorize]
        public async Task<IActionResult> ProfitReport()
        {
            var user = await GetCurrentUserAsync();
            ListOfStocks listOfStock = new ListOfStocks();
            listOfStock.Stocks = await _context.Stocks.Include(s => s.Country)
                                        .Include(s => s.Sector)
                                        .Include(s => s.Transactions)
                                            .ThenInclude(t => t.UserAgency)
                                        .Distinct().ToListAsync();
            //select transactions of the logged in user only
            foreach (Stock st in listOfStock.Stocks)
            {
                st.Transactions = st.Transactions.Where(t => t.UserAgency.UserId == user.Id).ToList();
            }

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
                        st.Profit = st.Profit + ((t.Qty * t.Rate) - (t.Qty * st.AvarageRate));
                        st.TotalQty -= t.Qty;
                        st.ProfitQty += t.Qty;
                    }
                }
            }
            listOfStock.TotalValues = listOfStock.Stocks
                                      .GroupBy(s => s.Country.Name)
                                      .Select(group => new TotalValue
                                      {
                                          CountryName = group.Key,
                                          Currnecy = group.Select(item => item.Country.Currency).First().ToString(),
                                          Total = group.Sum(item => item.Profit)
                                      }).ToList();
            return View(listOfStock);
        }

        [Authorize]
        public async Task<IActionResult> SectorReport()
        {
            ////convert list of Igrouping to list
            //ListOfStocks listOfStock = new ListOfStocks();
            //IEnumerable<IGrouping<int, Stock>> groups = await _context.Stocks.Include(s => s.Country)
            //                            .Include(s => s.Sector)
            //                            .Include(s => s.Transactions)
            //                            .GroupBy(s => s.SectorId)
            //                            .ToListAsync();
            //IEnumerable<Stock> Stocks = groups.SelectMany(group => group);
            //listOfStock.Stocks = Stocks.ToList();


            var user = await GetCurrentUserAsync();
            List<Sector> listOfSector = new List<Sector>();
            listOfSector = await _context.Sectors.Include(s => s.Stocks)
                                .ThenInclude(s => s.Transactions)
                                    .ThenInclude(t => t.UserAgency)
                                .ToListAsync();

            //select transactions of the logged in user only
            foreach (Sector s in listOfSector)
            {
                foreach (Stock st in s.Stocks)
                {
                    st.Transactions = st.Transactions.Where(t => t.UserAgency.UserId == user.Id).ToList();
                }
            }


            foreach (Sector s in listOfSector)
            {
                foreach (Stock st in s.Stocks)
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
                    s.TotalValue += st.TotalQty * st.AvarageRate;
                }
            }
            return View(listOfSector);
        }

        [Authorize]
        public async Task<IActionResult> ChartReport(int? _id)
        {
            var user = await GetCurrentUserAsync();
            ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "Name");

            if (_id == null)
            {
                List<Country> list = _context.Countries.ToList();
                _id = list[0].CountryId;
            }
            List<List<object>> Arr = new List<List<object>>();
            List<object> array1 = new List<object>();
            array1.Add("ticker");
            array1.Add("Qty");
            Arr.Add(array1);

            ListOfStocks listOfStock = new ListOfStocks();
            listOfStock.Stocks = await _context.Stocks.Include(s => s.Country)
                                        .Include(s => s.Sector)
                                        .Include(s => s.Transactions)
                                            .ThenInclude(t=>t.UserAgency)
                                        .Where(s => s.CountryId == _id)
                                        .Distinct().ToListAsync();

            foreach (Stock st in listOfStock.Stocks)
            {
                st.Transactions = st.Transactions.Where(t => t.UserAgency.UserId == user.Id).ToList();
            }

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
                List<object> array = new List<object>();
                array.Add(st.Ticker);
                array.Add(st.TotalQty*st.AvarageRate);
                Arr.Add(array);
            }
            Arr = Arr.ToList();
            
            string json = JsonConvert.SerializeObject(Arr);
            
            ViewBag.Arr = json;
             return View();
        }
    }
}
