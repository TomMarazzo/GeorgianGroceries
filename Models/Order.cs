﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeorgianGroceries.Models
{
    public class Order
    {
        public int OrderId { get; set; } //PK
        public DateTime OrderDate { get; set; }
        public string CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public string email { get; set; }
        public float Total { get; set; }
        //Child reference 
        public List<OrderDetail> OrderDetails { get; set; }
    }
}

