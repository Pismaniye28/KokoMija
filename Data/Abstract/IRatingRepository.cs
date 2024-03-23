using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity;
using KokoMija.Entity;

namespace Data.Abstract
{
    public interface IRatingRepository : IRepository<Rating>
    {
       List<Rating> GetRatingsForProduct(int productId);
       double GetAverageRatingForProduct(int productId);
       Rating GetRatingByProductAndUser(int productId, string userId);

        Task<List<Rating>> GetRatingsForProductAsync(int productId);
        Task<double?> GetAverageRatingForProductAsync(int productId);
        Task<Rating> GetRatingByProductAndUserAsync(int productId, string userId);
        Task AddAsync(Rating rating);
        Task<int> GetRatingCountForProduct(int productId);
        Task<List<Product>> GetMostFavoritedProductsAsync();
        Task<List<Product>> GetMostPositivelyRatedProductsAsync();
        Task<List<Product>> GetRatedRecommentProduct(int proId);
        Task<List<Product>> GetFavRecommentProduct(int proId);
        Task<List<Product>> GetProductsRatedByUserAsync(string userId);
    }
}