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
    public class FavoriteManager : IFavoriteService
    {
         private readonly IUnitOfWork _unitOfWork;

        public FavoriteManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task AddFavoriteAsync(FavoriteProduct favorite)
        {
            await _unitOfWork.Favorites.AddFavoriteAsync(favorite); // Using 'Favorite' instead of 'Favorites'
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteFavoriteAsync(string userId, int productId)
        {
            await _unitOfWork.Favorites.DeleteFavoriteAsync( userId,  productId); // Using 'Favorite' instead of 'Favorites'
            await _unitOfWork.SaveAsync();
        }

        public async Task<List<Product>> GetFavoriteProductsAsync(string userId, bool orderByDateDescending = true)
        {
            return await _unitOfWork.Favorites.GetFavoriteProductsAsync(userId, orderByDateDescending); // Using 'Favorite' instead of 'Favorites'
        }

        public async Task<List<Product>> GetFavoriteProductsSortedByDateAsync(string userId, bool descending = true)
        {
            return await _unitOfWork.Favorites.GetFavoriteProductsSortedByDateAsync(userId, descending); // Using 'Favorite' instead of 'Favorites'
        }
        public async Task<bool> IsFavoriteAsync(string userId, int productId)
        {
            return await _unitOfWork.Favorites.IsFavoriteAsync(userId,productId);
        }
    }
}