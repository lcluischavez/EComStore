using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EStore.Models;

namespace EStore.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public bool IsAdmin { get; set; }
        public List<Product> Products { get; set; }
        public virtual List<Order> Orders { get; set; }


    }
}