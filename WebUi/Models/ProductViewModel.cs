using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity;
using KokoMija.Entity;

namespace WebUi.Models
{
      public class PageInfo
    {
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public string CurrentCategory { get; set; }

        public int TotalPages()
        {
             return (int)Math.Ceiling((decimal)TotalItems/ItemsPerPage);
        }
    }

    public class ProductListViewModel
    {
        public PageInfo PageInfo { get; set; }
        public List<Product> Products { get; set; }
        public List<Courser> Courser{get;set;}
        public List<Category> Categories{get;set;}
        public List<Product> MostFav { get; set; }
        public List<Product> MostRated { get; set; }
        public bool IsFavorite { get; set; }
    }   

}