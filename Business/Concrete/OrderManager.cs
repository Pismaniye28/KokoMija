using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bussines.Abstract;
using Data.Abstract;
using KokoMija.Entity;

namespace Bussines.Concrete
{
    public class OrderManager : IOrderService
    {        
          private readonly IUnitOfWork _unitofwork;

        public string ErrorMessage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public OrderManager( IUnitOfWork unitofwork)
        {
            _unitofwork = unitofwork;
        }
        
        public void Create(Order entity)
        {
            _unitofwork.Orders.Create(entity);
            _unitofwork.Save();
        }

        public async Task<List<Order>> GetCompletedOrders()
        {
            return await _unitofwork.Orders.GetCompletedOrders();
        }


        public List<Order> GetOrders(string userId)
        {
            return  _unitofwork.Orders.GetOrders(userId);
        }

        public int GetNewlyOrders()
        {
            return _unitofwork.Orders.GetNewlyOrders();
        }
         public int GetpacingOrders()
        {
            return _unitofwork.Orders.GetpacingOrders();
        }

        public Task<List<Order>> GetAllOrders()
        {
            return _unitofwork.Orders.GetAllOrders();
        }

        public bool Validation(Order entity)
        {
            throw new NotImplementedException();
        }
    }
}