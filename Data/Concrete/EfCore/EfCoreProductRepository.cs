using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Abstract;
using Entity;
using KokoMija.Entity;
using Microsoft.EntityFrameworkCore;

namespace Data.Concrete.EfCore
{  public class EfCoreProductRepository :
        EfCoreGenericRepository<Product>, IProductRepository
    {
        public EfCoreProductRepository(ShopContext context):base(context)
        {
            
        }
        private ShopContext ShopContext{
            get{return context as ShopContext;}
        }

        


        public List<Product> GetAllWithPage(int page, int PageSize)
        {
           
           
                var products = ShopContext
                    .Products
                    .Where(i=>i.IsApproved)
                    .Include(i=>i.ProductImages)
                    .ThenInclude(i=>i.Image)
                    .AsQueryable();
                 return products.Skip((page-1)*PageSize).Take(PageSize).ToList();
            
        }

        
        public Product GetByIdWithCategories(int id)
        {
          
                return ShopContext.Products
                                .Where(i=>i.ProductId == id)
                                .Include(i=>i.ProductCategories)
                                .ThenInclude(i=>i.Category)
                                .Include(i=>i.ProductImages)
                                .ThenInclude(i=>i.Image)
                                .FirstOrDefault();
            
        }

        public int GetCountByCategory(string category)
        {
          
                var products = ShopContext.Products.Where(i=>i.IsApproved).AsQueryable();

                if(!string.IsNullOrEmpty(category))
                {
                    products = products
                                    .Include(i=>i.ProductCategories)
                                    .ThenInclude(i=>i.Category)
                                    .Where(i=>i.ProductCategories.Any(a=>a.Category.Url == category));
                }

                return products.Count();
            
        }
        public List<Product> GetHomePageProducts()
        {
           
                return ShopContext.Products
                    .Where(i=>i.IsApproved && i.IsHome)
                    .Include(i=>i.ProductImages)
                    .ThenInclude(i=>i.Image)
                    .ToList();
            
        }
        public Product GetProductDetails(string url)
        {
            
                return ShopContext.Products
                                .Where(i=>i.Url==url&&i.IsApproved)
                                .Include(i=>i.ProductCategories)
                                .ThenInclude(i=>i.Category)
                                .Include(i=>i.ProductImages)
                                .ThenInclude(i=>i.Image)
                                .FirstOrDefault();

            
        }
        public List<Product> GetProductsByCategory(string name,int page,int pageSize)
        {
           
                var products = ShopContext
                    .Products
                    .Where(i=>i.IsApproved)
                    .Include(i=>i.ProductImages)
                    .ThenInclude(i=>i.Image)
                    .AsQueryable();

                if(!string.IsNullOrEmpty(name))
                {
                    products = products
                                    .Include(i=>i.ProductCategories)
                                    .ThenInclude(i=>i.Category)
                                    .Where(i=>i.ProductCategories.Any(a=>a.Category.Url == name));
                }

                return products.Skip((page-1)*pageSize).Take(pageSize).ToList();
            
        }
        public List<Product> GetSearchResult(string searchString)
        {
            
                var products = ShopContext
                    .Products
                    .Where(i=>i.IsApproved && (i.ProductName.ToLower().Contains(searchString.ToLower()) || i.Description.ToLower().Contains(searchString.ToLower())))
                    .Include(i=>i.ProductImages)
                    .ThenInclude(i=>i.Image)
                    .AsQueryable();

                      return products.ToList();
            
        }

        public Product ProductImagesGetById(int id)
        {
           
                return ShopContext.Products
                                .Where(i=>i.ProductId == id).Include(i=>i.ProductImages).ThenInclude(i=>i.Image)
                                .FirstOrDefault();
            
        }

        public void Update(Product entity, int[] categoryIds , int[] selectedImages)
        {
           
                var product = ShopContext.Products
                                    .Include(i=>i.ProductCategories)
                                    .Include(i=>i.ProductImages)
                                    .FirstOrDefault(i=>i.ProductId==entity.ProductId);


                if(product!=null)
                {
                    product.ProductName = entity.ProductName;
                    product.Price = entity.Price;
                    product.Description=entity.Description;
                    product.Url =entity.Url;
                    product.IsApproved=entity.IsApproved;
                    product.IsHome=entity.IsHome;
                    product.DiscountRate=entity.DiscountRate;
                    product.IsInDiscount=entity.IsInDiscount;
                    product.Quatation=entity.Quatation;
                    
                    product.ProductImages= selectedImages.Select(imgId=>new ProductImage(){
                        ProductId=entity.ProductId,
                        ImageId= imgId
                    }).ToList();

                    product.ProductCategories = categoryIds.Select(catid=>new ProductCategory()
                    {
                        ProductId=entity.ProductId,
                        CategoryId = catid
                    }).ToList();            
                   
                }

            

        }
         public override async Task< List<Product>> GetAll()
        {
            
                return await ShopContext.Set<Product>().Include(i=>i.ProductImages).ThenInclude(i=>i.Image).ToListAsync();
            
        }

       public async Task<List<Product>> HomePageRecomment(int productId)
        {
            return await ShopContext.Products
                .Where(i => i.ProductId != productId && i.IsApproved && i.IsHome)
                .Include(i => i.ProductImages)
                .ThenInclude(i => i.Image)
                .ToListAsync();
        }

        

    }
}