using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyPortfolio.Models
{
    public class Sector
    {
        [Key]
        public int SectorId { get; set; }
        [Required]
        [StringLength(30)]
        [Display(Name = "Sector Name")]
        public string Name { get; set; }

        public virtual ICollection<Stock> Stocks { get; set; }
        [NotMapped]
        [Display(Name = "Total Investment")]
        public double TotalValue { get; set; }
    }
}
