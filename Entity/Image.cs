using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Entity
{
    public class Image
    {
        public int ImageId { get; set; }
        
        public String ImageUrl{get; set;}
        #nullable enable
        public String? ColorName{get;set;}
        
        public String? ColorCode{get;set;}
        #nullable disable
        public List<ProductImage> ProductImages { get; set; }
        public bool SizeSmall { get; set; }
        public bool SizeMedium { get; set; }
        public bool SizeLarge { get; set; }
        public bool SizeXSmall { get; set; }
        public bool SizeXLarge { get; set; }
        public int Quatation{get;set;}
        public DateTime DateAdded { get; set; }
    }
}