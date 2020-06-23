using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Paintings.Models.OrderViewModels
{
    public class OrderDetailViewModel
    {

        public Order Order { get; set; }

        public IEnumerable<OrderLineItem> LineItems { get; set; }
    }
}