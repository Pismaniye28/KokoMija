using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Entity;

namespace WebUi.Models
{
    public class ImgCreateModel
    {
        #nullable enable
        [DataType(DataType.Text)]
        public string? colorName { get; set; }
        
        public string? colorCode { get; set; }

         public bool ClearColors { get; set; }
        
    }
}