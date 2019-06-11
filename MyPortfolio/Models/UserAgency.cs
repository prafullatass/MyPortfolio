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
        [StringLength(30)]
        public int UserAgencyId { get; set; }
        [Required]
        public int AgencyId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public int AccountNo { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public Agency Agency { get; set; }
        public ApplicationUser User { get; set; }
    }
}
