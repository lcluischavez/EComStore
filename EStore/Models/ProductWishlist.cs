using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EStore.Models
{
    public class ProductWishlist
    {
        [Key]
        public int ProductWishlistId { get; set; }
        public int ProductId { get; set; }
        public int WishlistId { get; set; }
        public Wishlist Wishlist { get; set; }
        public Product Product { get; set; }
    }
}