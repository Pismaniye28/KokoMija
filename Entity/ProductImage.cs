using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KokoMija.Entity;

namespace Entity
{
    public class ProductImage
    {
        public int ImageId { get; set; }
        public Image Image { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}