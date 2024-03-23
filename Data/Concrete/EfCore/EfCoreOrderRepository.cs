using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Abstract;
using KokoMija.Entity;
using Microsoft.EntityFrameworkCore;

namespace Data.Concrete.EfCore
{
    public class EfCoreOrderRepository : EfCoreGenericRepository<Order>, IOrderRepository
    {
           public EfCoreOrderRepository(ShopContext context):base(context)
        {
            
        }
        private ShopContext ShopContext{
            get{return context as ShopContext;}
        }

        public async Task<List<Order>> GetAllOrders()
        {
            var orders = await ShopContext.Orders.Include(i=>i.OrderItems).ThenInclude(i=>i.Product).ToListAsync();
            return orders;
        }

        public async Task<List<Order>> GetCompletedOrders()
        {
                var orders = ShopContext.Orders
                .Where(o => o.OrderState == EnumOrderState.completed)
                .Include(o=>o.OrderItems)
                .ThenInclude(o=>o.Product)
                .ToListAsync();
                return await orders;
        }
              public  int GetNewlyOrders()
                {
                        var orders = ShopContext.Orders
                        .Where(o => o.OrderState == EnumOrderState.waiting)
                        .Count();
                        return orders;
                }

        public List<Order> GetOrders(string userId)
        {
           

                var orders = ShopContext.Orders
                                    .Include(i=>i.OrderItems)
                                    .ThenInclude(i=>i.Product)
                                    .ThenInclude(i=>i.Image)
                                    .AsQueryable();

                if(!string.IsNullOrEmpty(userId))
                {
                    orders = orders.Where(i=>i.UserId ==userId);
                }

                return orders.ToList();
            
        }

        public int GetpacingOrders()
        {
            var orders = ShopContext.Orders
                        .Where(o => o.OrderState == EnumOrderState.packing)
                        .Count();
                        return orders;
        }
    }
}