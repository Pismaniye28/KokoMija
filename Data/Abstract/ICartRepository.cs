using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity;

namespace Data.Abstract
{
    public interface ICartRepository : IRepository<Cart>
    {
        Task<Cart> GetByUserIdAsync(string userId);
        Task DeleteFromCartAsync(int cartId, int productId); 
        Task ClearCartAsync(int cartId);
        Cart GetByUserId(string userId);
        void ClearCart(int cartId);
    }
}
