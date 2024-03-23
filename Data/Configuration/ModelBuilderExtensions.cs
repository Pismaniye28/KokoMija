using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity;
using KokoMija.Entity;
using Microsoft.EntityFrameworkCore;

namespace Data.Configuration
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder builder)
        {
            builder.Entity<Category>().HasData(  
            new Category(){CategoryId=1,Name="Telefon",Url="telefon"},
            new Category(){CategoryId=2,Name="Bilgisayar", Url="bilgisayar"},
            new Category(){CategoryId=3,Name="Elektronik", Url="elektronik"},
            new Category(){CategoryId=4,Name="Discount %", Url="discount"});

             builder.Entity<Product>().HasData(
            new Product(){ProductId=1,ProductName="Samsung S5",Url="samsung-s5",Price=2000,Description="iyi telefon", IsApproved=true ,IsInDiscount=false },
            new Product(){ProductId=2,ProductName="Samsung S6",Url="samsung-s6",Price=3000,Description="iyi telefon", IsApproved=true ,IsInDiscount=false },
            new Product(){ProductId=3,ProductName="Samsung S7",Url="samsung-s7",Price=4000,Description="iyi telefon", IsApproved=true ,IsInDiscount=false},
            new Product(){ProductId=4,ProductName="Samsung S8",Url="samsung-s8",Price=5000,Description="iyi telefon", IsApproved=true,IsInDiscount=false},
            new Product(){ProductId=5,ProductName="Samsung S9",Url="samsung-s9",Price=6000,Description="iyi telefon", IsApproved=true,IsInDiscount=true}
            );

                 builder.Entity<ProductCategory>().HasData(           
            new ProductCategory(){ProductId=1,CategoryId=1},
            new ProductCategory(){ProductId=2,CategoryId=2},
            new ProductCategory(){ProductId=3,CategoryId=3},
            new ProductCategory(){ProductId=4,CategoryId=4} );

            builder.Entity<Image>().HasData(            
            new Image(){ImageId=1,ImageUrl="1.jpg",ColorName="Black",ColorCode="#ffff"},
            new Image(){ImageId=2,ImageUrl="2.jpg",ColorName="Black",ColorCode="#ffff"},
            new Image(){ImageId=3,ImageUrl="3.jpg",ColorName="Black",ColorCode="#ffff"},
            new Image(){ImageId=4,ImageUrl="4.jpg",ColorName="Black",ColorCode="#ffff"});

              builder.Entity<ProductImage>().HasData( 
            new ProductImage(){ProductId=1,ImageId=1},
            new ProductImage(){ProductId=2,ImageId=2},
            new ProductImage(){ProductId=3,ImageId=3},
            new ProductImage(){ProductId=4,ImageId=4});

              builder.Entity<Courser>().HasData(           
            new Courser(){CourserId=1,CourserImgUrl="slider1.png"},
            new Courser(){CourserId=2,CourserImgUrl="slider2.png"},
            new Courser(){CourserId=3,CourserImgUrl="slider3.png"});
       
        }
    }
}