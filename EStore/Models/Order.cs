using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace EStore.Models
{
    public class Order
    {
        [Required]
        public int OrderId { get; set; }
        [Required]
        public int ChosenProductId { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsComplete { get; set; }
        public string ApplicationUserId { get; set; }
        [Display(Name = "User")]
        public ApplicationUser ApplicationUser { get; set; }
    }
}