using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Entity;
using KokoMija.Entity;

namespace WebUi.Models
{
      public class ProductModel
    {
        public int ProductId { get; set; }  
        // [Display(Name="Name",Prompt="Enter product name")]
        // [Required(ErrorMessage ="Product ismini yazınız ve bu zorunludur !")]
        // [StringLength(70,MinimumLength =5,ErrorMessage ="İsim alanı 5-70 karakter arası olmalıdır ")]
        public string ProductName {get; set;} 
        // [Required(ErrorMessage ="Url ismini yazınız ve bu zorunludur !")]  
         public string Url { get; set; }  
        [Required(ErrorMessage ="Fiyat belirleyiniz ve bu zorunludur !")]    
        [Range(1,9999999999,ErrorMessage ="Fiyat - değer alamaz (1-9999999999)")]
        public double? Price { get; set; } 
        [Required(ErrorMessage ="Description yazınız ve bu zorunludur !")]
        public string Description { get; set; }    
        public bool IsApproved { get; set; }
        public bool IsHome { get; set; }
        [Required(ErrorMessage ="Evet ve ya Hayır !")]
        public bool IsInDiscount { get; set; }
        [Display(Name = "Discount Rate")]
        [Range(1,99,ErrorMessage ="indirim tutarı 99 üstü ve 1 altı olamaz")]
        public int? DiscountRate { get; set; }

        public List<Category> SelectedCategories { get; set; } 
        public List<Category> AllCategories { get; set; }
            
        public double? Quatations { get; set; }

        public List<Image> SelectedImages{get;set;} 
        public List<Image> AllImages{get;set;}
    }
}