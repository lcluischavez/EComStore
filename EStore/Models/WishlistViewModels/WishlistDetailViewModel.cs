using System;
using EStore.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EStore.Models.WishlistViewModels
{
    public class WishlistDetailViewModel
    {

        public Wishlist Wishlist { get; set; }

        public IEnumerable<WishlistLineItem> LineItems { get; set; }
    }
}