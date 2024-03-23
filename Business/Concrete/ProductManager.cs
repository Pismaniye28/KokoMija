using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bussines.Abstract;
using Data.Abstract;
using KokoMija.Entity;

namespace Bussines.Concrete
{
       public class ProductManager : IProductService
    {
        private readonly IUnitOfWork _unitofwork;
        public ProductManager( IUnitOfWork unitofwork)
        {
            _unitofwork = unitofwork;
        }
        
        

        public bool Create(Product entity)
        {

            if (Validation(entity))
            {
                 // iş kuralları uygula
                 
            _unitofwork.Products.Create(entity);
            _unitofwork.Save();
            return true ;
            }
            return false;
           
        }

        public void Delete(Product entity)
        {
            if (Validation(entity))
            {
                    // iş kuralları
                _unitofwork.Products.Delete(entity);
                _unitofwork.Save();
                
            }

        }

        public async Task< List<Product>> GetAll()
        {            
            return await _unitofwork.Products.GetAll();
        }

        public async Task<Product> GetById(int id)
        {
            return await _unitofwork.Products.GetById(id);
        }

        public Product GetByIdWithCategories(int id)
        {
            return  _unitofwork.Products.GetByIdWithCategories(id);
        }

        public int GetCountByCategory(string category)
        {
            return  _unitofwork.Products.GetCountByCategory(category);
        }

        public List<Product> GetHomePageProducts()
        {
           return  _unitofwork.Products.GetHomePageProducts();
        }

        public Product GetProductDetails(string url)
        {
            return  _unitofwork.Products.GetProductDetails(url);
        }

        public List<Product> GetProductsByCategory(string name,int page,int pageSize)
        {
            return  _unitofwork.Products.GetProductsByCategory(name,page,pageSize);
        }

        public List<Product> GetSearchResult(string searchString)
        {
           return  _unitofwork.Products.GetSearchResult(searchString);
        }

        public void Update(Product entity)
        {
            
             _unitofwork.Products.Update(entity);
             _unitofwork.Save();
        }

        public bool Update(Product entity, int[] categoryIds,int[] ImageId)
        {
            if(Validation(entity))
            {
                if(categoryIds.Length==0)
                {
                    ErrorMessage += "Ürün için en az bir kategori seçmelisiniz.";
                    return false;
                }
                  _unitofwork.Products.Update(entity,categoryIds,ImageId);
                  _unitofwork.Save();
                return true;
            }
            return false;          
        }

        public string ErrorMessage { get; set; }

        public bool Validation(Product entity)
        {
            var isValid = true;
            
            if (string.IsNullOrEmpty(entity.ProductName))
            {
                ErrorMessage += "ürün ismi girmelisiniz.\n";
                isValid = false;
            }

            if (entity.Price<0)
            {
                ErrorMessage += "ürünün fiyatı 0'dan küçük ve ya negatif olamaz";
                 isValid = false;
            }



            return isValid;


        }

        public List<Product> GetAllWithPage(int page, int PageSize)
        {
            return  _unitofwork.Products.GetAllWithPage(page,PageSize);
        }

        public Product ProductImagesGetById(int id)
        {
            return  _unitofwork.Products.ProductImagesGetById(id);
        }

    

        public async Task<Product> CreateAsync(Product entity)
        {
            
            await _unitofwork.Products.CreateAsync(entity);
            await _unitofwork.SaveAsync();
            return entity;
        }

        public async Task UpdateAsync(Product entityToUpdate, Product entity)
        {
            entityToUpdate.ProductName = entity.ProductName;
            entityToUpdate.Price=entity.Price;
            entityToUpdate.Description=entity.Description;
            entityToUpdate.IsInDiscount=entity.IsInDiscount;
            entityToUpdate.IsApproved=entity.IsApproved;
            entityToUpdate.IsHome=entity.IsHome;
            entityToUpdate.DiscountRate=entity.DiscountRate;
            entityToUpdate.Quatation=entity.Quatation;
            entityToUpdate.Url=entity.Url;
            
            await _unitofwork.SaveAsync();
            
        }

        public async Task DeleteAsync(Product entity)
        {
            _unitofwork.Products.Delete(entity);
            await _unitofwork.SaveAsync();
            
        }

        public async Task< List<Product>> HomePageRecomment(int id)
        {
            return await _unitofwork.Products.HomePageRecomment(id);
        }
    }
}