using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace EStore.Models
{
    public class Product
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public string Quantity { get; set; }
        [Required]
        public string Cost { get; set; }
        public bool IsSold { get; set; }

        [Display(Name = "Image Url")]
        public string ImagePath { get; set; }

        public string ApplicationUserId { get; set; }
        [Display(Name = "User")]
        public ApplicationUser ApplicationUser { get; set; }
    }
}