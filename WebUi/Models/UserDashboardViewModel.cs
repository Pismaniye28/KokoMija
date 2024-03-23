using Entity;
using KokoMija.Entity;

namespace WebUi.Models
{
    public class UserDashboardViewModel
    {
        public IEnumerable<Product> FavoriteProducts { get; set; }
        public IEnumerable<Product> RatedProducts { get; set; }
        public IEnumerable<ProductComment> UserComments { get; set; }
    }
}