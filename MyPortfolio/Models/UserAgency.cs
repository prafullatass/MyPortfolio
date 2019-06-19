using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyPortfolio.Models
{
    public class UserAgency
    {
        [Key]
        [Display(Name = "Agency")]
        public int UserAgencyId { get; set; }
        [Required]
        public int AgencyId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        [Display(Name = "Account No. ")]
        public int AccountNo { get; set; }
        public DateTime OpeningDate { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public Agency Agency { get; set; }
        public ApplicationUser User { get; set; }
    }
}
