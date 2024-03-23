using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KokoMija.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configuration
{
    public class CourserConfiguration : IEntityTypeConfiguration<Courser>
    {
        public void Configure(EntityTypeBuilder<Courser> builder)
        {
            builder.HasKey(m=>m.CourserId);
            builder.Property(m=>m.CourserImgUrl).IsRequired();
             builder.Property(m=>m.DateAdded).HasDefaultValueSql("getdate()");
        }
    }
}