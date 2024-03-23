using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity;
using KokoMija.Entity;

namespace Data.Abstract
{
    public interface IFavoriteRepository:IRepository<FavoriteProduct>
    {
        Task AddFavoriteAsync(FavoriteProduct favorite);
         Task DeleteFavoriteAsync(string userId, int productId);
        Task<List<Product>> GetFavoriteProductsAsync(string userId, bool orderByDateDescending = true);
        Task<List<Product>> GetFavoriteProductsSortedByDateAsync(string userId, bool descending = true);
        Task<bool> IsFavoriteAsync(string userId, int productId);
    }
}