using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyPortfolio.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }
        [Required]
        public int StockId { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [Required]
        [Display(Name = "Quantity")]
        public int Qty { get; set; }
        [Required]
        public Double Rate { get; set; }
        
        [Required]
        [Display(Name = "Action")]
        public Boolean BuyOrSell { get; set; }
        public Double Value { get; set; }
        public int UserAgencyId { get; set; }

        public UserAgency UserAgency { get; set; }
        public Stock Stock { get; set; }

    }
}
