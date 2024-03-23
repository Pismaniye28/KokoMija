using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KokoMija.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configuration
{
    public class CommentConfiguration : IEntityTypeConfiguration<ProductComment>
    {
        public void Configure(EntityTypeBuilder<ProductComment> builder)
        {
             builder.HasKey(m=>m.Id);
             builder.Property(m=>m.ProductId).IsRequired();
             builder.Property(m=>m.Content).IsRequired();
             builder.Property(m=>m.UserId).IsRequired();
             builder.Property(m=>m.CreatedAt).HasDefaultValueSql("getdate()");
        }
    }
}