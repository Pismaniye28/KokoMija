using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Abstract;
using Entity;
using KokoMija.Entity;
using Microsoft.EntityFrameworkCore;

namespace Data.Concrete.EfCore
{
    public class EfCoreRatingRepository
    : EfCoreGenericRepository<Rating>, IRatingRepository
    {
        public EfCoreRatingRepository(DbContext ctx) : base(ctx)
        {
        }
        private ShopContext ShopContext{
            get{return context as ShopContext;}
        }
            // You can add additional methods related to rating here if needed
            // For example, a method to get ratings for a specific product
        public List<Rating> GetRatingsForProduct(int productId)
        {
            return ShopContext.Ratings
                .Where(r => r.ProductId == productId)
                .ToList();
        }
         public double GetAverageRatingForProduct(int productId)
            {
                var ratings = GetRatingsForProduct(productId);
                if (ratings.Any())
                {
                    return ratings.Average(r => r.Stars);
                }
                return 0; // No ratings yet, return 0 as the average
            }
            public Rating GetRatingByProductAndUser(int productId, string userId)
            {
                return ShopContext.Ratings
                    .FirstOrDefault(r => r.ProductId == productId && r.UserId == userId);
            }
                public async Task<List<Rating>> GetRatingsForProductAsync(int productId)
                {
                    return await ShopContext.Ratings
                        .Where(r => r.ProductId == productId)
                        .ToListAsync();
                }

                public async Task<double?> GetAverageRatingForProductAsync(int productId)
                {
                      var averageRating = await ShopContext.Ratings
                            .Where(r => r.ProductId == productId)
                            .AverageAsync(r => (double?)r.Stars);

                        // If there are no ratings, the AverageAsync method returns null
                        // You can round the result if it is not null
                        if (averageRating.HasValue)
                        {
                            return Math.Round(averageRating.Value, 2);
                        }

                        return null;
                }

                public async Task<Rating> GetRatingByProductAndUserAsync(int productId, string userId)
                {
                    return await ShopContext.Ratings
                        .SingleOrDefaultAsync(r => r.ProductId == productId && r.UserId == userId);
                }
                    public async Task AddAsync(Rating rating)
                    {
                        await ShopContext.Ratings.AddAsync(rating);
                    }

                public async Task<int> GetRatingCountForProduct(int productId)
                {
                    return await ShopContext.Ratings.CountAsync(r => r.ProductId == productId);
                }
                public async Task<List<Product>> GetMostFavoritedProductsAsync()
                {
                    var productIds = await ShopContext.FavoriteProduct
                        .GroupBy(r => r.ProductId)
                        .OrderByDescending(group => group.Count())
                        .Select(group => group.Key)
                        .ToListAsync();

                    var products = await ShopContext.Products
                        .Where(p => productIds.Contains(p.ProductId)&& p.IsApproved)
                        .Include(i => i.ProductImages)
                        .ThenInclude(i => i.Image)
                        .ToListAsync();

                    // Reverse the order of products on the client side
                    return products.AsEnumerable().Reverse().ToList();
                }

                public async Task<List<Product>> GetMostPositivelyRatedProductsAsync()
                {
                    var productIds = await ShopContext.Ratings
                        .GroupBy(r => r.ProductId)
                        .OrderByDescending(group => group.Average(r => r.Stars))
                        .Select(group => group.Key)
                        .ToListAsync();

                    var products = await ShopContext.Products
                        .Where(p => productIds.Contains(p.ProductId)&&p.IsApproved)
                        .Include(i => i.ProductImages)
                        .ThenInclude(i => i.Image)
                        .ToListAsync();

                    // Reverse the order of products on the client side
                    return products.AsEnumerable().ToList();
                }

                public async Task<List<Product>> GetRatedRecommentProduct(int proId){
                        var productIds = await ShopContext.Ratings
                        .GroupBy(r => r.ProductId)
                        .OrderByDescending(group => group.Average(r => r.Stars))
                        .Select(group => group.Key)
                        .ToListAsync();
                        var products = await ShopContext.Products
                        .Where(p => productIds.Contains(p.ProductId)&& p.ProductId != proId && p.IsApproved)
                        .Include(i => i.ProductImages)
                        .ThenInclude(i => i.Image)
                        .ToListAsync();
                        return products.AsEnumerable().Reverse().ToList();
                }
                public async Task<List<Product>> GetFavRecommentProduct(int proId){
                        var productIds = await ShopContext.FavoriteProduct
                            .GroupBy(r => r.ProductId)
                            .OrderByDescending(group => group.Count())
                            .Select(group => group.Key)
                            .ToListAsync();

                        var products = await ShopContext.Products
                        .Where(p => productIds.Contains(p.ProductId)&& p.ProductId != proId && p.IsApproved)
                        .Include(i => i.ProductImages)
                        .ThenInclude(i => i.Image)
                        .ToListAsync();
                        return products.AsEnumerable().Reverse().ToList();
                }


                public async Task<List<Product>> GetProductsRatedByUserAsync(string userId)
                {
                    var productIds = await ShopContext.Ratings
                        .Where(r => r.UserId == userId)
                        .Select(r => r.ProductId)
                        .ToListAsync();

                    var products = await ShopContext.Products
                        .Where(p => productIds.Contains(p.ProductId))
                        .Include(p => p.ProductImages)
                        .ThenInclude(pi => pi.Image)
                        .ToListAsync();

                    return products;
                }



                
    }
}