using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KokoMija.Entity;

namespace Data.Abstract
{
     public interface IProductRepository: IRepository<Product>
    {
        Task< List<Product>> HomePageRecomment(int id);
        Product ProductImagesGetById(int id);
        List<Product> GetAllWithPage(int page,int PageSize);
       Product GetProductDetails(string url);
       Product GetByIdWithCategories(int id);
       List<Product> GetProductsByCategory(string name,int page,int pageSize);
       List<Product> GetSearchResult(string searchString);
       List<Product> GetHomePageProducts();
       
       int GetCountByCategory(string category);
        
        void Update(Product entity, int[] categoryIds,int[] ImageId);

        
    }
}