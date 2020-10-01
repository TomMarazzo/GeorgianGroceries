using System;
using System.Collections.Generic;
using System.Text;
using GeorgianGroceries.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GeorgianGroceries.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        //Define our MODEL CLASSES so our Controller(s) can access the Models
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Product> Products { get; set; }

        //OVERRIDE the Model Creating Meathod
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //Define Relationships and Keys

                //Product and Category
            builder.Entity<Product>()
                    .HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .HasConstraintName("FK_Products_CategoryID");
            //Product and OrderDetail
            builder.Entity<OrderDetail>()
                   .HasOne(p => p.Product)
                   .WithMany(c => c.OrderDetails)
                   .HasForeignKey(p => p.ProductId)
                   .HasConstraintName("FK_OrderDetails_ProductID");
            //Product and Cart
            builder.Entity<Cart>()
                   .HasOne(p => p.Product)
                   .WithMany(c => c.Carts)
                   .HasForeignKey(p => p.ProductId)
                   .HasConstraintName("FK_Carts_ProductID");
            //OrderDetail and Order
            builder.Entity<OrderDetail>()
                   .HasOne(p => p.Order)
                   .WithMany(c => c.OrderDetails)
                   .HasForeignKey(p => p.OrderId)
                   .HasConstraintName("FK_OrderDetails_ProductID");

        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
