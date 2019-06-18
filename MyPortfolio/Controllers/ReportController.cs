﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyPortfolio.Data;
using MyPortfolio.Models;
using MyPortfolio.Models.ChartModel;
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
        public async Task<IActionResult> AllStockReport()
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
                        st.TotalQty -= t.Qty;
                    }
                }
            }
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

        public IActionResult ChartReport2()
        {
            return View();
        }
        public async Task<IActionResult> ChartReport()
        {
            List<double> ValueArray = new List<double>();
            List<string> NameArray = new List<string>();
            List<List<object>> Arr = new List<List<object>>();
            List<object> array1 = new List<object>();
            array1.Add("ticker");
            array1.Add("Qty");
            Arr.Add(array1);

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
                List<object> array = new List<object>();
               array.Add(st.Ticker);
                array.Add(st.TotalQty);
                Arr.Add(array);
                //Arr[i] = [{v = st.Ticker;
                // Arr[i] = [new { v= (st.AvarageRate * st.TotalQty)}];
            }
            ViewBag.ValueArray = ValueArray;
            ViewBag.NameArray = NameArray;
            Arr = Arr.ToList();
            

            string json = JsonConvert.SerializeObject(Arr);
            
            ViewBag.Arr = json;
             return View();
            //return Json(Arr);
        }

        public IActionResult UseDataFromServer()
        {
            return View();
        }

        public JsonResult JsonData()
        {
            var data = ModelHelper.MultiLineData();
            return Json(data);
        }

        //public async JsonResult jsonData ()
        //{
        //    List<double> ValueArray = new List<double>();
        //    List<string> NameArray = new List<string>();
        //    List<List<object>> Arr = new List<List<object>>();

        //    ListOfStocks listOfStock = new ListOfStocks();
        //    listOfStock.Stocks = await _context.Stocks.Include(s => s.Country)
        //                                .Include(s => s.Sector)
        //                                .Include(s => s.Transactions)
        //                                .Distinct().ToListAsync();
        //    int i = 0;
        //    foreach (Stock st in listOfStock.Stocks)
        //    {
        //        foreach (Transaction t in st.Transactions)
        //        {
        //            if (t.BuyOrSell)
        //            {
        //                double total = st.TotalQty * st.AvarageRate;
        //                total = total + (t.Qty * t.Rate);
        //                st.TotalQty += t.Qty;
        //                st.AvarageRate = Math.Round((total / st.TotalQty) * 100) / 100;
        //            }
        //            else
        //            {
        //                st.TotalQty -= t.Qty;
        //            }
        //        }
        //        ValueArray.Add(st.AvarageRate * st.TotalQty);
        //        NameArray.Add(st.Ticker);
        //        List<object> array = new List<object>();
        //        array.Add(st.Ticker);
        //        array.Add(st.TotalQty);
        //        Arr.Add(array);
        //        //Arr[i] = [{v = st.Ticker;
        //        // Arr[i] = [new { v= (st.AvarageRate * st.TotalQty)}];
        //    }
        //    ViewBag.ValueArray = ValueArray;
        //    ViewBag.NameArray = NameArray;
        //    Arr = Arr.ToList();


        //    string json = JsonConvert.SerializeObject(Arr);
        //    return(json);
        //}
    }
}
