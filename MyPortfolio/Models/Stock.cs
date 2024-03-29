﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyPortfolio.Models
{
    public class Stock
    {
        [Key]
        public int StockId { get; set; }
        [Required]
        public int CountryId { get; set; }
        [Required]
        [StringLength(30)]
        [Display(Name = "Stock Name")]
        public string Name { get; set; }
        [Required]
        [StringLength(6)]
        public string Ticker { get; set; }
        [Required]
        public int SectorId { get; set; }
        

        public virtual ICollection<Transaction> Transactions { get; set; }
        public Country Country { get; set; }
        public Sector Sector { get; set; }

        [NotMapped]
        [Display(Name = "Avarage Rate")]
        public double AvarageRate { get; set; }
        [NotMapped]
        [Display(Name = "Quantity")]
        public int TotalQty { get; set; }
        [NotMapped]
        [Display(Name = "Profit / Loss")]
        public double Profit { get; set; }
        [NotMapped]
        [Display(Name = "Quantity")]
        public double ProfitQty { get; set; }
    }
}
