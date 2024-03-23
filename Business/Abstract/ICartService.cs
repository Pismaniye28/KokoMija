using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity;

namespace Bussines.Abstract
{
    public interface ICartService
    {
        void InitializeCart(string userId);

        Cart GetCartByUserId(string userId);
        Task AddToCartAsync(string userId, int productId, int quantity, string color, string size,string imageurl); // Updated method signature
        Task ClearCartAsync(int cartId);
        Task<Cart> GetCartByUserIdAsync(string userId);
        Task DeleteFromCartAsync(string userId, int productId);
    }
}
