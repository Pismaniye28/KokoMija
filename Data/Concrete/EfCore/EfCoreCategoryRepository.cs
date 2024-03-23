using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Abstract;
using KokoMija.Entity;
using Microsoft.EntityFrameworkCore;

namespace Data.Concrete.EfCore
{
    public class EfCoreCategoryRepository : EfCoreGenericRepository<Category>, ICategoryRepository
    {
        public EfCoreCategoryRepository(ShopContext context):base(context)
        {
            
        }
        private ShopContext ShopContext{
            get{return context as ShopContext;}
        }

     public void DeleteFromCategory(int productId, int categoryId)
        {
          
                var cmd = "delete from productcategory where ProductId=@p0 and CategoryId=@p1";
                ShopContext.Database.ExecuteSqlRaw(cmd,productId,categoryId);
            
        }

        public Category GetByIdWithProducts(int categoryId)
        {
          
                return ShopContext.Categories
                            .Where(i=>i.CategoryId==categoryId)
                            .Include(i=>i.ProductCategories)
                            .ThenInclude(i=>i.Product)
                            .ThenInclude(i=>i.ProductImages)
                            .ThenInclude(i=>i.Image)
                            .FirstOrDefault();
            
        }

        public List<Category> GetPopularCategories()
        {
            throw new NotImplementedException();
        }
    }
}