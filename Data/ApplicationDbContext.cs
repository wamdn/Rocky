#nullable disable
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Rocky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public DbSet<Category> Category { get; set; }
        public DbSet<ApplicationType> ApplicationType { get; set; }
        public DbSet<Product> Product { get; set; }

        public ApplicationDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }       

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity => {
                entity
                    .Property(e => e.Price)
                    .HasPrecision(10, 2);

                /*
                entity
                    .HasOne(p => p.Category)
                    .WithMany()
                    .HasForeignKey(p => p.CategoryId)
                    .HasConstraintName("FK_Product_Category_CategoryId");*/     // constraint name = FK_<dependent>_<principal>_<FK>
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
