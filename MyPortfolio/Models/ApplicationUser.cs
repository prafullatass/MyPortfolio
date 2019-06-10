using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyPortfolio.Models
{
    public class ApplicationUser :  IdentityUser
    {
        public ApplicationUser()
        {

        }
        [Key]
        public int UserId { get; set; }
        [Required]
        [StringLength(30)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(30)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [StringLength(55)]
        public string StreetAddress { get; set; }
        public virtual ICollection<UserAgency>  UserAgencies { get; set; }
    }
}
