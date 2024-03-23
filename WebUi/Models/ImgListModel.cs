using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity;

namespace WebUi.Models
{
    public class ImgListModel
    {
        public List<Image> Images {get;set;}

        public List<ProductImage> productImages {get; set;}
    }
}