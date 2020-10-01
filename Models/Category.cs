﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GeorgianGroceries.Models
{
    public class Category
    {       
        public int CategoryId { get; set; } //Primary Key
        [Required]
        public string Name { get; set; }

        public List<Product> Products { get; set; }
        
    }
}
