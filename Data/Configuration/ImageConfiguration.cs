using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configuration
{
    public class ImageConfiguration : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
             builder.HasKey(m=>m.ImageId);
             builder.Property(m=>m.ImageUrl).IsRequired();
             builder.Property(m=>m.DateAdded).HasDefaultValueSql("getdate()");
        }
    }
}