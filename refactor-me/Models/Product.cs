using System;
using System.ComponentModel.DataAnnotations;

namespace refactor_me.Models
{
    public class Product
    {
        public Guid ProductId { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public decimal Price { get; set; }

        public decimal DeliveryPrice { get; set; }
    }


}