using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KokoMija.Entity;



namespace Entity
{
    public class Rating
    {
     public int Id { get; set; }
     public int Stars { get; set; }
     public int ProductId { get; set; }
     public Product Product { get; set; }
     public DateTime DateTime { get; set; }
     public string UserId { get; set; }
    }
}