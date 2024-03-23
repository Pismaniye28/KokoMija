using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KokoMija.Entity;

namespace Data.Abstract
{
    public interface IOrderRepository: IRepository<Order>
    {
        List<Order> GetOrders(string userId);
       Task< List<Order>> GetCompletedOrders();
       Task<List<Order>> GetAllOrders();
       int GetNewlyOrders();
       int GetpacingOrders();
    }
}