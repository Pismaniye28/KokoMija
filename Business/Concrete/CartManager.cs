using System;
using System.Linq;
using System.Threading.Tasks;
using Bussines.Abstract;
using Data.Abstract;
using Entity;

namespace Bussines.Concrete
{
    public class CartManager : ICartService
    {
        private readonly IUnitOfWork _unitofwork;

        public CartManager(IUnitOfWork unitofwork,IProductService productService)
        {
            _unitofwork = unitofwork;
        }

        public async Task AddToCartAsync(string userId, int productId, int quantity, string color, string size,string imageurl)
        {
            var cart = GetCartByUserId(userId);
            if (cart != null)
            {
                // Check if the product is already in the cart
                var index = cart.CartItems.FindIndex(i => i.ProductId == productId && i.Color == color && i.Size == size);
                var product = await _unitofwork.Products.GetById(productId);

                if (index < 0)
                {
                    cart.CartItems.Add(new CartItem
                    {
                        ProductId = productId,
                        Quantity = quantity,
                        CartId = cart.Id,
                        Color = color,
                        Size = size,
                        ImageUrl = imageurl 
                    });
                }
                else
                {
                    cart.CartItems[index].Quantity += quantity;
                }

                _unitofwork.Carts.Update(cart);
                await _unitofwork.SaveAsync();
            }
        }

        public Cart GetCartByUserId(string userId)
        {
            return _unitofwork.Carts.GetByUserId(userId);
        }

        public void InitializeCart(string userId)
        {
            _unitofwork.Carts.Create(new Cart()
            {
                UserId = userId
            });
            _unitofwork.Save();
        }

        public void ClearCart(int cartId)
        {
            _unitofwork.Carts.ClearCart(cartId);
            _unitofwork.Save();
        }

        public async Task ClearCartAsync(int cartId)
        {
            _unitofwork.Carts.ClearCart(cartId);
            await _unitofwork.SaveAsync();
        }

        public async Task<Cart> GetCartByUserIdAsync(string userId)
        {
            return await Task.FromResult(GetCartByUserId(userId));
        }

        public async Task DeleteFromCartAsync(string userId, int productId)
        {
            var cart = GetCartByUserId(userId);
            if (cart != null)
            {
                var cartItem = cart.CartItems.FirstOrDefault(item => item.ProductId == productId);

                if (cartItem != null)
                {
                    // Remove the cart item with the specified productId
                    cart.CartItems.Remove(cartItem);
                    _unitofwork.Carts.Update(cart);
                    await _unitofwork.SaveAsync();
                }
            }
        }

    }
}
