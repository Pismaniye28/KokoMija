using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Data.Abstract;

namespace Data.Concrete.EfCore
{
    public class UnitOfWork: IUnitOfWork
    {
        private readonly ShopContext _context;
        public UnitOfWork(ShopContext context)
        {
            _context = context;
        }
        
        private EfCoreCartRepository _cartRepository;
        private EfCoreCategoryRepository _categoryRepository;
        private EfCoreOrderRepository _orderRepository;
        private EfCoreProductRepository _productRepository;
        private EfCoreCouserRepository _courseRepository;
        private EfCorePhotoRepository _photoRepository;
        private EfCoreFavoriteRepository _favoriteRepository;
        private EfCoreRatingRepository _ratingRepository;
        private EfCoreCommentRepository _commentRepository;

        public ICartRepository Carts => 
            _cartRepository = _cartRepository ?? new EfCoreCartRepository(_context);

        public ICategoryRepository Categories => 
            _categoryRepository = _categoryRepository ?? new EfCoreCategoryRepository(_context);             

        public IOrderRepository Orders => 
            _orderRepository = _orderRepository ?? new EfCoreOrderRepository(_context);

        public IProductRepository Products => 
            _productRepository = _productRepository ?? new EfCoreProductRepository(_context);

        public ICourserRepository Slider =>
            _courseRepository = _courseRepository??new EfCoreCouserRepository(_context);

        public IPhotoRepository Photo => 
            _photoRepository = _photoRepository??new EfCorePhotoRepository(_context);

        public IRatingRepository Ratings => 
            _ratingRepository = _ratingRepository??new EfCoreRatingRepository(_context);

        public IFavoriteRepository Favorites =>
           _favoriteRepository = _favoriteRepository?? new EfCoreFavoriteRepository(_context);   

        public ICommentRepository Comment =>
           _commentRepository = _commentRepository?? new EfCoreCommentRepository(_context);                

        public void Dispose()
        {
            _context.Dispose();
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public async Task<int> SaveAsync()
        {
           return await _context.SaveChangesAsync();
        }
        public async Task BeginTransactionAsync()
        {
            await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }



    }
}