﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeorgianGroceries.Models
{
    public class Cart
    {
        public int CartId { get; set; } //PK

        public int ProductId { get; set; }

        public DateTime DateCreated { get; set; }

        public string CustomerId { get; set; }

        public int Quantity { get; set; }

        public double Price { get; set; }

        public Product Product { get; set; }

    }
}
