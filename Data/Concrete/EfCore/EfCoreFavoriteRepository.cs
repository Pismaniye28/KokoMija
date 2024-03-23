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
    public class EfCoreFavoriteRepository : EfCoreGenericRepository<FavoriteProduct> , IFavoriteRepository
    {
        public EfCoreFavoriteRepository(DbContext ctx) : base(ctx)
        {
        }
        private ShopContext ShopContext{
            get{return context as ShopContext;}
        }
         // Add a favorite product
        public async Task AddFavoriteAsync(FavoriteProduct favorite)
        {
            await context.Set<FavoriteProduct>().AddAsync(favorite);
        }

        public async Task DeleteFavoriteAsync(string userId, int productId)
        {
            var favoriteToDelete = await context.Set<FavoriteProduct>()
                .FirstOrDefaultAsync(fp => fp.UserId == userId && fp.ProductId == productId);

            if (favoriteToDelete != null)
            {
                context.Set<FavoriteProduct>().Remove(favoriteToDelete);
            }
        }


        public async Task<List<Product>> GetFavoriteProductsAsync(string userId)
        {
            var favoriteProductIds = await ShopContext.FavoriteProduct
                .Where(fp => fp.UserId == userId)
                .OrderByDescending(fp => fp.DateTime)
                .Select(fp => fp.ProductId)
                .ToListAsync();

            var products = await ShopContext.Products
                .Where(p => favoriteProductIds.Contains(p.ProductId))
                .Include(i => i.ProductImages)
                .ThenInclude(i => i.Image)
                .ToListAsync();

            return products;
        }


        public async Task<List<Product>> GetFavoriteProductsAsync(string userId, bool orderByDateDescending = true)
        {
            IQueryable<FavoriteProduct> query = context.Set<FavoriteProduct>()
                .Where(fp => fp.UserId == userId);

            if (orderByDateDescending)
            {
                query = query.OrderByDescending(fp => fp.DateTime);
            }
            else
            {
                query = query.OrderBy(fp => fp.DateTime);
            }

            var favoriteProducts = await query.ToListAsync();

            // Get the product IDs from favorite products
            var productIds = favoriteProducts.Select(fp => fp.ProductId).ToList();

            // Retrieve the products based on the product IDs
            var products = await context.Set<Product>()
                .Where(p => productIds.Contains(p.ProductId))
                .Include(i => i.ProductImages)
                .ThenInclude(i => i.Image)
                .ToListAsync();

            return products;
        }

            public async Task<List<Product>> GetFavoriteProductsSortedByDateAsync(string userId, bool descending = true)
            {
                IQueryable<FavoriteProduct> query = context.Set<FavoriteProduct>()
                    .Where(fp => fp.UserId == userId);

                if (descending)
                {
                    query = query.OrderByDescending(fp => fp.DateTime);
                }
                else
                {
                    query = query.OrderBy(fp => fp.DateTime);
                }

                var favoriteProducts = await query.ToListAsync();

                // Get the product IDs from favorite products
                var productIds = favoriteProducts.Select(fp => fp.ProductId).ToList();

                // Retrieve the products based on the product IDs
                var products = await context.Set<Product>()
                    .Where(p => productIds.Contains(p.ProductId))
                    .Include(i => i.ProductImages)
                    .ThenInclude(i => i.Image)
                    .ToListAsync();

                return products;
            }

        public async Task<bool> IsFavoriteAsync(string userId, int productId)
        {
            return await context.Set<FavoriteProduct>()
                .AnyAsync(fp => fp.UserId == userId && fp.ProductId == productId);
        }

    }
}