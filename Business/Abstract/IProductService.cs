using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KokoMija.Entity;

namespace Bussines.Abstract
{
       public interface IProductService:IValidator<Product>
    {
        Task< List<Product>> HomePageRecomment(int id);
        Product ProductImagesGetById(int id);
        Task<Product> GetById(int id);
        Product GetByIdWithCategories(int id);
        Product GetProductDetails(string url);

        List<Product> GetAllWithPage(int page, int PageSize);
        List<Product> GetProductsByCategory(string name,int page,int pageSize);
        int GetCountByCategory(string category);
        
        
        List<Product> GetHomePageProducts();
        List<Product> GetSearchResult(string searchString);
        Task< List<Product>> GetAll();
        bool Create(Product entity);
        Task<Product> CreateAsync(Product entity);
        void Update(Product entity);
        Task UpdateAsync(Product entityToUpdate,Product entity);
        void Delete(Product entity);
        Task DeleteAsync(Product entity);
         
        bool Update(Product entity, int[] categoryIds , int[] ImageId);
    }
}