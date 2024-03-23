using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bussines.Abstract;
using Data.Abstract;
using Entity;
using KokoMija.Entity;

namespace Bussines.Concrete
{
    public class RatingManager : IRatingService
    {
         private readonly IUnitOfWork _unitofwork;
          public RatingManager( IUnitOfWork unitofwork)
        {
            _unitofwork = unitofwork;
        }
        public string ErrorMessage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public double GetAverageRatingForProduct(int productId)
        {
            return _unitofwork.Ratings.GetAverageRatingForProduct(productId);
        }

        public Rating GetRatingByProductAndUser(int productId, string userId)
        {
            return _unitofwork.Ratings.GetRatingByProductAndUser(productId,userId);
        }

        public List<Rating> GetRatingsForProduct(int productId)
        {
            return _unitofwork.Ratings.GetRatingsForProduct(productId);
        }

            public async Task<List<Rating>> GetRatingsForProductAsync(int productId)
            {
                return await _unitofwork.Ratings.GetRatingsForProductAsync(productId);
            }

            public async Task<double?> GetAverageRatingForProductAsync(int productId)
            {
                return await _unitofwork.Ratings.GetAverageRatingForProductAsync(productId);
            }

            public async Task<Rating> GetRatingByProductAndUserAsync(int productId, string userId)
            {
                return await _unitofwork.Ratings.GetRatingByProductAndUserAsync(productId, userId);
            }

            //----------------
                public async Task SubmitRatingAsync(int productId, string userId, int rating)
                {
                    // Check if the user has already rated the product, and update the existing rating if applicable
                    Rating existingRating = await _unitofwork.Ratings.GetRatingByProductAndUserAsync(productId, userId);
                    if (existingRating != null)
                    {
                        existingRating.Stars = rating;
                    }
                    else
                    {
                        // Create a new rating entity if the user hasn't rated the product before
                        existingRating = new Rating
                        {
                            ProductId = productId,
                            UserId = userId,
                            Stars = rating,
                            DateTime = DateTime.UtcNow
                        };
                        await _unitofwork.Ratings.AddAsync(existingRating);
                    }

                    await _unitofwork.SaveAsync();
                }

        public bool Validation(Rating entity)
        {
            return true;
        }

        public async Task<int> GetRatingCount(int productId)
        {
            // Use your data layer or repository to query the database and get the count of ratings for the specified product
            return await _unitofwork.Ratings.GetRatingCountForProduct(productId);
        }

        public async Task<List<Product>> GetMostFavoritedProductsAsync()
        {
            return await _unitofwork.Ratings.GetMostFavoritedProductsAsync();
        }

        public async Task<List<Product>> GetMostPositivelyRatedProductsAsync()
        {
            return await _unitofwork.Ratings.GetMostPositivelyRatedProductsAsync();
        }

                public async Task<List<Product>> GetRatedRecommentProduct(int proId)
        {
            return await _unitofwork.Ratings.GetRatedRecommentProduct(proId);
        }
                public async Task<List<Product>> GetFavRecommentProduct(int proId)
        {
            return await _unitofwork.Ratings.GetFavRecommentProduct(proId);
        }
                public async Task<List<Product>> GetProductsRatedByUserAsync(string userId)
                {
                    return await _unitofwork.Ratings.GetProductsRatedByUserAsync(userId);
                }

    }
}