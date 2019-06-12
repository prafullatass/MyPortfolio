using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyPortfolio.Models
{
    public class Agency
    {
        [Key]
        public int AgencyId { get; set; }
        [Required]
        public int CountryId { get; set; }
        [Required]
        [StringLength(30)]
        [Display(Name = "Agency")]
        public string Name { get; set; }
        public Country Country { get; set; }
        public virtual ICollection<UserAgency> UserAgencies { get; set; }
    }
}
