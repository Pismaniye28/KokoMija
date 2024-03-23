using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KokoMija.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configuration
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(m=>m.ProductId);
            builder.Property(m=>m.ProductName).IsRequired().HasMaxLength(150);
            // builder.Property(m=>m.DateAdded).HasDefaultValueSql("date('now')"); SQLİTE
            builder.Property(m=>m.DateAdded).HasDefaultValueSql("getdate()"); //SSMS

           

        }
    }
}