using System.ComponentModel.DataAnnotations;
using Entity;

namespace KokoMija.Entity
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        public string ProductName { get; set; }

        public double? Price { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public bool IsApproved { get; set; }

        public bool IsHome { get; set; }

        public bool IsInDiscount { get; set; }

        public int? DiscountRate { get; set; }

        public double? Quatation { get; set; }

        public DateTime DateAdded { get; set; }

        public double? AverageRating { get; set; }

        public int RatingCount { get; set; }
        public bool? IsFavorite { get; set; } 
        //Stripe Att
        #nullable enable
        public string? StripeProductId { get; set; }
        public string? StripePriceId { get; set; }
        public string? StripeDiscountedPriceId { get; set; }
        #nullable disable
        // Navigation Properties
        public List<ProductCategory> ProductCategories { get; set; }

        public List<ProductComment> Comments { get; set; }

        public Category Category { get; set; }

        public List<ProductImage> ProductImages { get; set; }

        public Image Image { get; set; }
    }

    
}