using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace EStore.Models
{
    public class Wishlist
    {
        [Required]
        public int WishlistId { get; set; }
        [Required]
        public int ChosenProductId { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsComplete { get; set; }
        public List<ProductWishlist> ProductWishlist { get; set; }
        public string ApplicationUserId { get; set; }
        [Display(Name = "User")]
        public ApplicationUser ApplicationUser { get; set; }
    }
}
