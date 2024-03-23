using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Entity;

namespace WebUi.Models
{
    public class ImgEditModel
    {
        
        [DataType(DataType.Text)]
        public string colorName { get; set; }
        
        public string colorCode { get; set; }
        public string imageUrl { get; set; }
        [Required]
        public int imageId { get; set; }

        public bool ClearColors { get; set; }

        public int Quatation { get; set; }

        public bool xsmall { get; set; }

        public bool Small { get; set; }

        public bool Medium { get; set; }

        public bool Large { get; set; }

        public bool xlarge { get; set; }

        public List<ProductImage> productimages { get; set; }
        

    }
}