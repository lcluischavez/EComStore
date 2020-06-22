using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace EStore.Models
{
    public class PaymentType
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string PaymentName { get; set; }
        public int OrderId { get; set; }
        public string ApplicationUserId { get; set; }
        [Display(Name = "User")]
        public ApplicationUser ApplicationUser { get; set; }
    }
}