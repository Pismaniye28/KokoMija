using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Entity;
using KokoMija.Entity;

namespace WebUi.Models
{
    public class CategoryModel
    {
        public int CategoryId { get; set; }
        [Required(ErrorMessage ="Name ismini yazınız ve bu zorunludur !(Katagorinin isim alanı zorunludur)")]
        [StringLength(100,MinimumLength =2,ErrorMessage ="2-100 karakter sayısına uymalıdır")]
        public string Name { get; set; }
         [Required(ErrorMessage ="Url ismini yazınız ve bu zorunludur !(Katagorinin Url alanı zorunludur)")]
         [StringLength(100,MinimumLength =2,ErrorMessage ="2-100 karakter sayısına uymalıdır")]
        public string Url { get; set; }

        public List<Product> Products { get; set; }

        public List<ProductImage> Images{get;set;}
    }
}