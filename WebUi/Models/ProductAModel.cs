using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Abstract;
using KokoMija.Entity;

namespace WebUi.Models
{
    public class ProductAModel
    {
        public Product Product {get; set;}
        public bool IsFavorite { get; set; } 
    }
}