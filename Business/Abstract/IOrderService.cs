using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KokoMija.Entity;

namespace Bussines.Abstract
{
    public interface IOrderService:IValidator<Order>
    {
        void Create(Order entity);
        List<Order> GetOrders(string userId);
        Task< List<Order>> GetCompletedOrders();
        Task<List<Order>> GetAllOrders();
        int GetNewlyOrders();
        int GetpacingOrders();
    }
}