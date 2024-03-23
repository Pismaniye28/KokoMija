using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity;
using KokoMija.Entity;

namespace Bussines.Abstract
{
    public interface IRatingService : IValidator<Rating>
    {
        List<Rating> GetRatingsForProduct(int productId);
        double GetAverageRatingForProduct(int productId);
        Rating GetRatingByProductAndUser(int productId, string userId);
            Task<List<Rating>> GetRatingsForProductAsync(int productId);
            Task<double?> GetAverageRatingForProductAsync(int productId);
           Task<Rating> GetRatingByProductAndUserAsync(int productId, string userId);
           Task SubmitRatingAsync(int productId, string userId, int rating);
           Task<int> GetRatingCount(int productId);
        Task<List<Product>> GetMostFavoritedProductsAsync();
        Task<List<Product>> GetMostPositivelyRatedProductsAsync();
        Task<List<Product>> GetRatedRecommentProduct(int proId);
        Task<List<Product>> GetFavRecommentProduct(int proId);
        Task<List<Product>> GetProductsRatedByUserAsync(string userId);
    }
}