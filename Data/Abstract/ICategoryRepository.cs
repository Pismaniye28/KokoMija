using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KokoMija.Entity;

namespace Data.Abstract
{
    public interface ICategoryRepository: IRepository<Category>
    {
        List<Category> GetPopularCategories();
        Category GetByIdWithProducts(int categoryId);

        void DeleteFromCategory(int productId,int categoryId);
    }
}