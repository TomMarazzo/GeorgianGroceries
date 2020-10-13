using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GeorgianGroceries.Models
{
    public class Product
    {
        public int ProductId { get; set; } //PK
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
       
        public string Photo { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:c}")]
        [Range(0.01,999999)]
        public double Price { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        public List<Cart> Carts { get; set; }
        [Display(Name ="Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
