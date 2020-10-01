using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeorgianGroceries.Models
{
    public class Product
    {
        public int ProductId { get; set; } //PK

        public  string Name { get; set; }
        public int Description { get; set; }
        public int Photo { get; set; }
        public float Price { get; set; }
        public Category Category { get; set; }

        public List<OrderDetail> OrderDetails { get; set; }

        public List<Cart> Carts { get; set; }

        public Category CategoryId { get; set; }
    }
}
