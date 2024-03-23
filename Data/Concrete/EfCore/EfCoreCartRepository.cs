using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Abstract;
using Entity;
using Microsoft.EntityFrameworkCore;

namespace Data.Concrete.EfCore
{
    public class EfCoreCartRepository : EfCoreGenericRepository<Cart>, ICartRepository
    {

         public EfCoreCartRepository(ShopContext context) : base(context)
        {
        }

        private ShopContext ShopContext
        {
            get { return context as ShopContext; }
        }

        public async Task<Cart> GetByUserIdAsync(string userId)
        {
            return await ShopContext.Carts
                .Include(i => i.CartItems)
                .ThenInclude(i => i.Product)
                .ThenInclude(i => i.Image)
                .FirstOrDefaultAsync(i => i.UserId == userId);
        }

            public async Task DeleteFromCartAsync(int cartId, int productId)
            {
                var cmd = @"delete from CartItems where CartId=@p0 and ProductId=@p1";
                await ShopContext.Database.ExecuteSqlRawAsync(cmd, cartId, productId);
            }

        public async Task ClearCartAsync(int cartId)
        {
            var cmd = @"delete from CartItems where CartId=@p0";
            await ShopContext.Database.ExecuteSqlRawAsync(cmd, cartId);
        }

        public void ClearCart(int cartId)
        {
            var cmd = @"delete from CartItems where CartId=@p0";
            ShopContext.Database.ExecuteSqlRaw(cmd, cartId);
        }

        public Cart GetByUserId(string userId)
        {
            return ShopContext.Carts
                .Include(i => i.CartItems)
                .ThenInclude(i => i.Product)
                .ThenInclude(i => i.Image)
                .FirstOrDefault(i => i.UserId == userId);
        }

        public override void Update(Cart entity)
        {
            ShopContext.Carts.Update(entity);
        }
    }
}
