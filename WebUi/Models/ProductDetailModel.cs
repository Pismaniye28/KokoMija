using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity;
using KokoMija.Entity;

namespace WebUi.Models
{
      public class ProductDetailModel
    {
        public Product Product { get; set; }
        public List<Category> Categories { get; set; }
        public List<Product> HomePageRecomment { get; set; }
        public List<Product> MostFavProducts { get; set; }
        public List<Product> MostRatedProducts { get; set; }
        public List<ProductComment> Comments{get;set;}
        public double AverageRating { get; set; }
        public int RatingCount { get; set; }
        public bool IsFavorite { get; set; }
    }
}